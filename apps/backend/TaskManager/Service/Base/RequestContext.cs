using System;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using TaskManager.Models.Entities;
using TaskManager.Repository;
using TaskManager.Utils.i18n;

namespace TaskManager.Service.Base
{
    public interface IRequestContext
    {
        /// <summary>
        /// Id do usuário autenticado
        /// </summary>
        Guid UserId { get; }

        /// <summary>
        /// Usuário autenticado
        /// </summary>
        User User { get; }
    }

    public sealed class RequestContext(IHttpContextAccessor httpContextAccessor, IResourceStringLocalizer localizer, IServiceProvider provider) : IRequestContext
    {
        private User _user;

        /// <summary>
        /// Indica se o usuário está autenticado
        /// </summary>
        public bool IsAuthenticated =>
            httpContextAccessor
                .HttpContext?
                .User
                .Identity?
                .IsAuthenticated ??
            throw new ApplicationException(localizer.GetString(LocalizationDictionary.UserNotFound));

        /// <summary>
        /// Id do usuário autenticado
        /// </summary>
        public Guid UserId =>
            httpContextAccessor
                .HttpContext?
                .User
                .GetUserId() ??
            throw new ApplicationException(localizer.GetString(LocalizationDictionary.UserNotFound));

        /// <summary>
        /// Indica se o usuário está autenticado
        /// </summary>
        public User User => _user ??= provider.GetService<IUserRepository>().Get(UserId);
    }
}