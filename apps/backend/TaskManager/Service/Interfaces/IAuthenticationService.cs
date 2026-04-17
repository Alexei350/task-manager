using System.Threading.Tasks;
using TaskManager.Models.Base;
using TaskManager.Models.Request.Authentication;
using TaskManager.Models.Return;

namespace TaskManager.Service.Interfaces
{
    public interface IAuthenticationService
    {
        /// <summary>
        /// Gera o Token de acesso e o Refresh Token
        /// </summary>
        /// <param name="model">Dados do login</param>
        /// <returns></returns>
        Task<ReturnData<TokenReturn>> GenerateToken(LoginModel model);
        
        /// <summary>
        /// Gera um novo Token através do Token expirado e do Refresh Token
        /// </summary>
        /// <param name="model">Dados para geração do Token</param>
        /// <returns></returns>
        Task<ReturnData<TokenReturn>> RefreshToken(RefreshTokenModel model);

        /// <summary>
        /// Faz login no sistema a partir de um login do Google no front
        /// </summary>
        /// <param name="model"></param>
        /// <returns>Token de acesso e refresh token</returns>
        Task<ReturnData<TokenReturn>> GoogleLogin(GoogleLoginModel model);
        
        /// <summary>
        /// Gera um Token JWT a partir de uma API Key válida
        /// </summary>
        /// <param name="apiKey">API Key para autenticação</param>
        /// <returns>Token de acesso e refresh token</returns>
        Task<ReturnData<TokenReturn>> ApiKeyLogin(string apiKey);
    }
}