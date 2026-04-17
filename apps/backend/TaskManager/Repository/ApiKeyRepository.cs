using TaskManager.Models.Entities;
using TaskManager.Repository.Base;
using TaskManager.Context;

namespace TaskManager.Repository
{
    public interface IApiKeyRepository : IBaseRepository<ApiKey>
    {
    }

    public class ApiKeyRepository : BaseRepository<ApiKey>, IApiKeyRepository
    {
        public ApiKeyRepository(TaskManagerContext context) : base(context)
        {
        }
    }
}
