using System.Threading.Tasks;
using TaskManager.Service.Interfaces.Base;
using TaskManager.Models.Base;
using TaskManager.Models.Request.User;
using TaskManager.Models.Return;

namespace TaskManager.Service.Interfaces
{
    public interface IMeService : IBaseService
    {
        /// <summary>
        /// Retorna os dados do usuário autenticado
        /// </summary>
        /// <returns></returns>
        ReturnData<UserReturn> Get();

        /// <summary>
        /// Edita os dados do usuário autenticado
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<ReturnData> Update(UpdateUserModel model);
    }
}
