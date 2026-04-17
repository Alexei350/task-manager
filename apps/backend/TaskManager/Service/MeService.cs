using System.Threading.Tasks;
using TaskManager.Models.Base;
using TaskManager.Repository;
using TaskManager.Service.Interfaces;
using System;
using TaskManager.Models.Request.User;
using TaskManager.Models.Return;
using TaskManager.Service.Base;
using Microsoft.Extensions.DependencyInjection;
using TaskManager.Utils.Extensions;
using TaskManager.Utils.i18n;

namespace TaskManager.Service
{
    public class MeService(IServiceProvider provider) : BaseService(provider), IMeService
    {
        private readonly IUserRepository _repository = provider.GetService<IUserRepository>();

        /// <summary>
        /// Retorna os dados do usuário autenticado
        /// </summary>
        /// <returns></returns>
        public ReturnData<UserReturn> Get()
        {
            return new ReturnData<UserReturn>
            {
                Success = true,
                Data = new UserReturn
                {
                    Id = Context.User.Id,
                    Name = Context.User.Name,
                    Email = Context.User.Email
                }
            };
        }

        /// <summary>
        /// Edita os dados do usuário autenticado
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<ReturnData> Update(UpdateUserModel model)
        {
            if (
                model == default ||
                model.Name.IsNullOrEmpty()
            )
            {
                return ReturnData.ReturnError(Localizer.GetString(LocalizationDictionary.IncompleteData));
            }

            var user = await _repository
                .GetForUpdateAsync(Context.UserId);

            if (user == default)
                return ReturnData.ReturnError(Localizer.GetString(LocalizationDictionary.RecordNotFound));

            if (user.Deleted)
                return ReturnData.ReturnError(Localizer.GetString(LocalizationDictionary.RecordIsDeleted));

            user.Name = model.Name;

            _repository.Update(user);

            var success = await _repository.SaveChangesAsync();

            return success
                ? ReturnData.ReturnSuccess(Localizer.GetString(LocalizationDictionary.SuccessWhenUpdating))
                : ReturnData.ReturnError(Localizer.GetString(LocalizationDictionary.ErrorWhenUpdating));
        }
    }
}
