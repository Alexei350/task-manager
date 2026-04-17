using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using TaskManager.Models.Base;
using TaskManager.Models.Entities;
using TaskManager.Models.Return;
using TaskManager.Utils.i18n;

namespace TaskManager.Utils.Security;

public class Authentication(
    IResourceStringLocalizer localizer,
    IConfiguration config
)
{
    /// <summary>
    /// Gera um novo Refresh Token aleatório
    /// </summary>
    /// <returns></returns>
    public static string GenerateRefreshToken()
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
    public ReturnData<TokenReturn> GenerateToken(IEnumerable<Claim> claims, string refreshToken, User user)
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
    public ReturnData<ClaimsPrincipal> GetPrincipalFromExpiredToken(string token)
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
}