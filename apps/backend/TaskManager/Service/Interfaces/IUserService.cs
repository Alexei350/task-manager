using System.Threading.Tasks;
using TaskManager.Service.Interfaces.Base;
using TaskManager.Models.Base;
using TaskManager.Models.Request.User;

namespace TaskManager.Service.Interfaces
{
    public interface IUserService : IBaseService
    {
        /// <summary>
        /// Adiciona um usuário
        /// </summary>
        /// <param name="model">Dados do usuário</param>
        /// <returns></returns>
        Task<ReturnData> Create(CreateUserModel model);
    }
}