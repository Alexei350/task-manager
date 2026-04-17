using System.Threading.Tasks;
using TaskManager.Context;
using TaskManager.Models.Entities;
using TaskManager.Repository.Base;
using Microsoft.EntityFrameworkCore;

namespace TaskManager.Repository
{
    public interface IUserRepository : IBaseRepository<User>
    {
        /// <summary>
        /// Busca o usuário pelo Email
        /// </summary>
        /// <param name="email">Email a ser consultado</param>
        /// <returns></returns>
        Task<User> GetByEmailAsync(string email);
    }

    public class UserRepository(TaskManagerContext context) : BaseRepository<User>(context), IUserRepository
    {
        /// <summary>
        /// Busca o usuário pelo Email
        /// </summary>
        /// <param name="email">Email a ser consultado</param>
        /// <returns></returns>
        public async Task<User> GetByEmailAsync(string email)
        {
            return await Context.Users
                .FirstOrDefaultAsync(x => x.Email == email);
        }
    }
}