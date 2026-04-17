using TaskManager.Context;
using TaskManager.Models.Entities;
using TaskManager.Repository.Base;

namespace TaskManager.Repository
{
    public interface ITaskRepository : IBaseRepository<Task>
    {

    }

    public class TaskRepository(TaskManagerContext context) : BaseRepository<Task>(context), ITaskRepository
    {

    }
}