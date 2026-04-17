using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using TaskManager.Models.Base;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using TaskManager.Repository;
using TaskManager.Service.Interfaces;
using System.Collections.Generic;
using System.Security.Cryptography;
using Google.Apis.Auth;
using TaskManager.Models.Entities;
using TaskManager.Models.Enums;
using TaskManager.Service.Base;
using TaskManager.Models.Request.Authentication;
using TaskManager.Models.Return;
using TaskManager.Utils.Extensions;
using TaskManager.Utils.i18n;
using TaskManager.Utils.Security;

namespace TaskManager.Service
{
    public class AuthenticationService(
        IConfiguration config,
        IResourceStringLocalizer localizer, 
        IUserRepository userRepository,
        IApiKeyService apiKeyService
    ) : IAuthenticationService
    {
        /// <summary>
        /// Gera o Token de acesso e o Refresh Token
        /// </summary>
        /// <param name="model">Dados do login</param>
        /// <returns></returns>
        public async Task<ReturnData<TokenReturn>> GenerateToken(LoginModel model)
        {
            if (
                model == default ||
                model.Email.IsNullOrEmpty() ||
                model.Password.IsNullOrEmpty()
            )
            {
                return ReturnData<TokenReturn>.ReturnError(localizer.GetString(LocalizationDictionary.IncompleteData));
            }

            var user = await userRepository
                .GetByEmailAsync(model.Email);

            if (user == default || user.Deleted)
                return ReturnData<TokenReturn>.ReturnError(localizer.GetString(LocalizationDictionary.UserNotFound));

            var isValid = Hashing.ValidatePassword(model.Password, user.Password, user.Salt);

            if (!isValid)
                return ReturnData<TokenReturn>.ReturnError(localizer.GetString(LocalizationDictionary.IncorrectPassword));

            var refreshToken = GenerateRefreshToken();

            user.RefreshToken = refreshToken;
            user.LastAccess = DateTime.Now;

            await userRepository.SaveChangesAsync();

            var claims = new Claim[] 
            {
                new(type: ClaimTypes.Email, value: user.Email),
                new(type: ClaimTypes.Role, value: user.Role.ToString()),
                new(type: "UserId", value: user.Id.ToString()),
            };

            return GenerateToken(claims, refreshToken, user);
        }

        /// <summary>
        /// Gera um novo Token através do Token expirado e do Refresh Token
        /// </summary>
        /// <param name="model">Dados para geração do Token</param>
        /// <returns></returns>
        public async Task<ReturnData<TokenReturn>> RefreshToken(RefreshTokenModel model)
        {
            var principal = GetPrincipalFromExpiredToken(model.Token);

            if (!principal.Success || principal.Data == null)
            {
                return new ReturnData<TokenReturn>
                {
                    Success = false,
                    Messages = principal.Messages ?? [ new ReturnMessage(ReturnMessageTypeEnum.Error, localizer.GetString(LocalizationDictionary.TokenValidationError)) ]
                };
            }
            
            var userId = principal.Data.GetUserId();

            if (!userId.HasValue)
                return ReturnData<TokenReturn>.ReturnError(localizer.GetString(LocalizationDictionary.UserNotFound));

            var user = await userRepository.GetForUpdateAsync(userId.Value);

            if (user == default || user.Deleted)
                return ReturnData<TokenReturn>.ReturnError(localizer.GetString(LocalizationDictionary.UserNotFound));

            if (user.RefreshToken != model.RefreshToken)
                return ReturnData<TokenReturn>.ReturnError(localizer.GetString(LocalizationDictionary.InvalidRefreshToken));

            var newRefreshToken = GenerateRefreshToken();

            user.RefreshToken = newRefreshToken;
            user.LastAccess = DateTime.Now;

            await userRepository.SaveChangesAsync();

            return GenerateToken(principal.Data.Claims, newRefreshToken, user);
        }
        
        /// <summary>
        /// Faz login no sistema a partir de um login do Google no front
        /// </summary>
        /// <param name="model"></param>
        /// <returns>Token de acesso e refresh token</returns>
        public async Task<ReturnData<TokenReturn>> GoogleLogin(GoogleLoginModel model)
        {
            try
            {
                var payload = await GoogleJsonWebSignature.ValidateAsync(model.Token, new GoogleJsonWebSignature.ValidationSettings
                {
                    Audience = [config["GoogleClientId"]]
                });
                
                if (!payload.EmailVerified)
                    return ReturnData<TokenReturn>.ReturnError(localizer.GetString(LocalizationDictionary.EmailNotVerified));
                
                var user = await userRepository.GetByEmailAsync(payload.Email);
                
                var refreshToken = GenerateRefreshToken();
                
                if (user == default)
                {
                    var randomPassword = Convert.ToBase64String(RandomNumberGenerator.GetBytes(16));
                    var (hash, salt) = Hashing.HashPassword(randomPassword);
                    
                    user = new User
                    {
                        Name = payload.Name,
                        Email = payload.Email,
                        Password = hash,
                        Salt = salt,
                        Role = UserRoleEnum.Default,
                        RefreshToken = refreshToken,
                        GoogleUserId = payload.Subject,
                        LastAccess = DateTime.Now
                    };
                    
                    await userRepository.CreateAsync(user);
                }
                else
                {
                    user.RefreshToken = refreshToken;
                    user.LastAccess = DateTime.Now;
                    
                    userRepository.Update(user);
                }
                
                await userRepository.SaveChangesAsync();
                
                var claims = new Claim[] 
                {
                    new(type: ClaimTypes.Email, value: user.Email),
                    new(type: ClaimTypes.Role, value: user.Role.ToString()),
                    new(type: "UserId", value: user.Id.ToString()),
                };
                
                return GenerateToken(claims, refreshToken, user);
            }
            catch (InvalidJwtException)
            {
                return ReturnData<TokenReturn>.ReturnError(localizer.GetString(LocalizationDictionary.InvalidToken));
            }
            catch
            {
                return ReturnData<TokenReturn>.ReturnError(localizer.GetString(LocalizationDictionary.AuthenticationError));
            }
        }

        /// <summary>
        /// Gera um Token JWT a partir de uma API Key válida
        /// </summary>
        /// <param name="apiKey">API Key para autenticação</param>
        /// <returns>Token de acesso e refresh token</returns>
        public async Task<ReturnData<TokenReturn>> ApiKeyLogin(string apiKey)
        {
            if (string.IsNullOrWhiteSpace(apiKey))
            {
                return ReturnData<TokenReturn>.ReturnError(localizer.GetString(LocalizationDictionary.IncompleteData));
            }

            var validationResult = await apiKeyService.Validate(apiKey);

            if (!validationResult.Success || validationResult.Data == null)
            {
                return ReturnData<TokenReturn>.ReturnError(localizer.GetString(LocalizationDictionary.ApiKeyInvalid));
            }

            var user = validationResult.Data;

            var refreshToken = GenerateRefreshToken();

            user.RefreshToken = refreshToken;
            user.LastAccess = DateTime.Now;

            await userRepository.SaveChangesAsync();

            var claims = new Claim[] 
            {
                new(type: ClaimTypes.Email, value: user.Email),
                new(type: ClaimTypes.Role, value: user.Role.ToString()),
                new(type: "UserId", value: user.Id.ToString()),
            };

            return GenerateToken(claims, refreshToken, user);
        }
        
        #region Métodos Privados

        /// <summary>
        /// Gera um novo Refresh Token aleatório
        /// </summary>
        /// <returns></returns>
        private static string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            
            return Convert.ToBase64String(randomNumber);
        }

        /// <summary>
        /// Gera um Token com base nas Claims e em um Refresh Token gerado
        /// </summary>
        /// <param name="claims"></param>
        /// <param name="refreshToken"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        private ReturnData<TokenReturn> GenerateToken(IEnumerable<Claim> claims, string refreshToken, User user)
        {            
            var issuer = config["Jwt:Issuer"];
            var audience = config["Jwt:Audience"];

            var signingCredentials = new SigningCredentials(GetKey(), SecurityAlgorithms.HmacSha256);
            var expiresIn = DateTime.Now.AddHours(8);

            var tokenOptions = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: expiresIn,
                signingCredentials: signingCredentials
            );

            var token = new JwtSecurityTokenHandler().WriteToken(tokenOptions);

            return new ReturnData<TokenReturn>
            {
                Success = true,
                Data = new TokenReturn
                {
                    Token = token,
                    RefreshToken = refreshToken,
                    User = new UserReturn
                    {
                        Id = user.Id,
                        Email = user.Email,
                        Name = user.Name,
                    }
                }
            };
        }

        /// <summary>
        /// Busca as informações do Token expirado
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        private ReturnData<ClaimsPrincipal> GetPrincipalFromExpiredToken(string token)
        {
            try
            {
                var tokenValidationParameters = new TokenValidationParameters
                {
                    ValidateAudience = false,
                    ValidateIssuer = false,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = GetKey(),
                    ValidateLifetime = false
                };

                var tokenHandler = new JwtSecurityTokenHandler();
                var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out var securityToken);

                if (securityToken is not JwtSecurityToken jwtSecurityToken || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                    return ReturnData<ClaimsPrincipal>.ReturnError(localizer.GetString(LocalizationDictionary.InvalidToken));
            
                return new ReturnData<ClaimsPrincipal>
                {
                    Success = true,
                    Data = principal
                };
            }
            catch
            {
                return ReturnData<ClaimsPrincipal>.ReturnError(localizer.GetString(LocalizationDictionary.TokenValidationError));
            }
        }

        /// <summary>
        /// Busca a chave do Token dos parâmetros da aplicação
        /// </summary>
        /// <returns></returns>
        private SymmetricSecurityKey GetKey() => new(Encoding.UTF8.GetBytes(config["Jwt:Key"] ?? string.Empty));

        #endregion
    }
}