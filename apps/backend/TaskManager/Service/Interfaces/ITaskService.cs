using System;
using System.Threading.Tasks;
using TaskManager.Models.Base;
using TaskManager.Models.Request.Base;
using TaskManager.Models.Request.Task;
using TaskManager.Models.Return;
using TaskManager.Service.Interfaces.Base;

namespace TaskManager.Service.Interfaces
{
    public interface ITaskService : IBaseService
    {
        /// <summary>
        /// Adiciona uma tarefa
        /// </summary>
        /// <param name="model">Dados do usuário</param>
        /// <returns></returns>
        Task<ReturnData<TaskReturn>> Create(CreateTaskModel model);

        /// <summary>
        /// Edita uma tarefa
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<ReturnData> Update(UpdateTaskModel model);

        /// <summary>
        /// Deleta uma tarefa
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<ReturnData> Delete(Guid id);

        /// <summary>
        /// Retorna os dados de uma tarefa
        /// </summary>
        /// <returns></returns>
        Task<ReturnData<TaskReturn>> Get(Guid id);

        /// <summary>
        /// Retorna uma lista paginada de tarefas
        /// </summary>
        /// <returns></returns>
        Task<ReturnDataPaged<TaskReturn>> GetPaged(GetPagedModel model);
    }
}