using System;
using System.Threading.Tasks;
using TaskManager.Service.Interfaces;
using Microsoft.AspNetCore.Mvc;
using TaskManager.Models.Request.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;
using TaskManager.Controllers.Base;

namespace TaskManager.Controllers
{
    [ApiController]
    [AllowAnonymous]
    [Route("[controller]")]
    public class AuthenticationController(IServiceProvider provider) : BaseController(provider)
    {
        private readonly IAuthenticationService _authenticationService = provider.GetService<IAuthenticationService>();

        /// <summary>
        /// Gera o Token de acesso e o Refresh Token
        /// </summary>
        /// <param name="model">Dados do login</param>
        /// <returns></returns>
        [HttpPost("GenerateToken")]
        public async Task<IActionResult> GenerateToken(LoginModel model)
        {
            return await DefaultHandlerAsync(async () =>
            {
                var response = await _authenticationService.GenerateToken(model);

                if (!response.Success)
                    return Unauthorized(response);
                
                return Ok(response);
            });
        }

        /// <summary>
        /// Gera um novo Token através do Token expirado e do Refresh Token
        /// </summary>
        /// <param name="model">Dados para geração do Token</param>
        /// <returns></returns>
        [HttpPost("RefreshToken")]
        public async Task<IActionResult> RefreshToken(RefreshTokenModel model)
        {
            return await DefaultHandlerAsync(async () =>
            {
                var response = await _authenticationService.RefreshToken(model);

                if (!response.Success)
                    return Unauthorized(response);

                return Ok(response);
            });
        }
        
        /// <summary>
        /// Login com o Google
        /// </summary>
        /// <param name="googleToken">Token de autenticação gerado pelo Google</param>
        /// <returns>Token de acesso e refresh token</returns>
        [HttpPost("GoogleLogin")]
        public async Task<IActionResult> GoogleLogin(GoogleLoginModel googleToken)
        {
            return await DefaultHandlerAsync(async () =>
            {
                var response = await _authenticationService.GoogleLogin(googleToken);

                if (!response.Success)
                    return Unauthorized(response);

                return Ok(response);
            });
        }
        
        /// <summary>
        /// Gera um Token JWT usando uma API Key válida
        /// </summary>
        /// <param name="model">Modelo contendo a API Key</param>
        /// <returns>Token de acesso e refresh token</returns>
        [HttpPost("ApiKeyLogin")]
        public async Task<IActionResult> ApiKeyLogin(ApiKeyLoginModel model)
        {
            return await DefaultHandlerAsync(async () =>
            {
                var response = await _authenticationService.ApiKeyLogin(model.ApiKey);

                if (!response.Success)
                    return Unauthorized(response);

                return Ok(response);
            });
        }
    }
}