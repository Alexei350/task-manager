using System.Threading.Tasks;
using TaskManager.Models.Base;
using TaskManager.Models.Entities;
using TaskManager.Repository;
using TaskManager.Service.Interfaces;
using TaskManager.Models.Enums;
using System;
using TaskManager.Models.Request.User;
using TaskManager.Service.Base;
using Microsoft.Extensions.DependencyInjection;
using TaskManager.Utils.Extensions;
using TaskManager.Utils.i18n;
using TaskManager.Utils.Security;

namespace TaskManager.Service
{
    public class UserService(IServiceProvider provider) : BaseService(provider), IUserService
    {
        private readonly IUserRepository _repository = provider.GetService<IUserRepository>();

        /// <summary>
        /// Adiciona um usuário
        /// </summary>
        /// <param name="model">Dados do usuário</param>
        /// <returns></returns>
        public async Task<ReturnData> Create(CreateUserModel model)
        {
            if (
                model == default ||
                (model.Password.IsNullOrEmpty() && model.GoogleUserId.IsNullOrEmpty()) ||
                model.Name.IsNullOrEmpty() ||
                model.Email.IsNullOrEmpty()
            )
            {
                return ReturnData.ReturnError(Localizer.GetString(LocalizationDictionary.IncompleteData));
            }

            if (!model.Email.IsValidEmail())
                return ReturnData.ReturnError(Localizer.GetString(LocalizationDictionary.InvalidEmail));

            var domain = await _repository
                .GetByEmailAsync(model.Email);

            if (domain != default)
                return ReturnData.ReturnWarning(Localizer.GetString(LocalizationDictionary.DuplicatedUserEmail));

            var hash = (string)null;
            var salt = (string)null;

            if (!model.Password.IsNullOrEmpty())
                (hash, salt) = Hashing.HashPassword(model.Password);

            await _repository.CreateAsync(new User
            {
                Name = model.Name,
                Email = model.Email,
                Password = hash,
                Salt = salt,
                Role = UserRoleEnum.Default
            });

            var success = await _repository.SaveChangesAsync();

            return success
                ? ReturnData.ReturnSuccess(Localizer.GetString(LocalizationDictionary.SuccessWhenCreating))
                : ReturnData.ReturnError(Localizer.GetString(LocalizationDictionary.ErrorWhenCreating));
        }
    }
}