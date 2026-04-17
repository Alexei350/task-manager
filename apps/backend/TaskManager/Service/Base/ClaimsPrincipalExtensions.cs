using System;
using System.Security.Claims;

namespace TaskManager.Service.Base
{
    public static class ClaimsPrincipalExtensions
    {
        /// <summary>
        /// Busca o Id do usuário autenticado através das Claims
        /// </summary>
        /// <param name="principal"></param>
        /// <returns></returns>
        /// <exception cref="ApplicationException"></exception>
        public static Guid? GetUserId(this ClaimsPrincipal principal)
        {
            var userId = principal?.FindFirstValue("UserId");

            return Guid.TryParse(userId, out var parsedUserId) ?
                parsedUserId :
                null;
        }
    }
}