using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using TaskManager.Models.Base;
using TaskManager.Models.Base.Query;
using TaskManager.Models.Enums;
using TaskManager.Models.Request.Base;
using TaskManager.Models.Request.Task;
using TaskManager.Models.Return;
using TaskManager.Repository;
using TaskManager.Service.Base;
using TaskManager.Service.Interfaces;
using TaskManager.Utils.i18n;

namespace TaskManager.Service
{
    public class TaskService(IServiceProvider provider) : BaseService(provider), ITaskService
    {
        private readonly ITaskRepository _repository = provider.GetService<ITaskRepository>();

        /// <summary>
        /// Adiciona uma tarefa
        /// </summary>
        /// <param name="model">Dados da tarefa</param>
        /// <returns></returns>
        public async Task<ReturnData<TaskReturn>> Create(CreateTaskModel model)
        {
            if (
                model == default ||
                model.Status == default ||
                string.IsNullOrWhiteSpace(model.Description)
            )
            {
                return ReturnData<TaskReturn>.ReturnError(Localizer.GetString(LocalizationDictionary.IncompleteData));
            }
            
            if (!Enum.IsDefined(typeof(TaskStatusEnum), model.Status))
                return ReturnData<TaskReturn>.ReturnError(Localizer.GetString(LocalizationDictionary.InvalidStatus));

            var newTask = new Models.Entities.Task
            {
                UserId = Context.UserId,
                CreationDate = DateTime.Now,
                Status = model.Status,
                Description = model.Description,
                Observation = model.Observation,
                TimeSpent = model.TimeSpent,
                DueDate = model.DueDate
            };

            await _repository.CreateAsync(newTask);

            var success = await _repository.SaveChangesAsync();

            if (!success)
                return ReturnData<TaskReturn>.ReturnError(Localizer.GetString(LocalizationDictionary.ErrorWhenCreating));

            return new ReturnData<TaskReturn>
            {
                Success = true,
                Messages =
                [
                    new(ReturnMessageTypeEnum.Success, Localizer.GetString(LocalizationDictionary.SuccessWhenCreating))
                ],
                Data = new TaskReturn
                {
                    Id = newTask.Id,
                    Status = newTask.Status,
                    Description = newTask.Description,
                    Observation = newTask.Observation,
                    CreationDate = newTask.CreationDate,
                    TimeSpent = newTask.TimeSpent,
                    DueDate = newTask.DueDate,
                    CompletedDate = newTask.CompletedDate
                }
            };
        }

        /// <summary>
        /// Edita uma tarefa
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<ReturnData> Update(UpdateTaskModel model)
        {
            if (
                model == default ||
                model.Id == Guid.Empty ||
                model.Status == default ||
                string.IsNullOrWhiteSpace(model.Description)
            )
            {
                return ReturnData.ReturnError(Localizer.GetString(LocalizationDictionary.IncompleteData));
            }
            
            if (!Enum.IsDefined(typeof(TaskStatusEnum), model.Status))
                return ReturnData.ReturnError(Localizer.GetString(LocalizationDictionary.InvalidStatus));

            var domain = await _repository
                .GetForUpdateAsync(model.Id);

            if (domain == default)
                return ReturnData.ReturnError(Localizer.GetString(LocalizationDictionary.RecordNotFound));

            domain.Status = model.Status;
            domain.Description = model.Description;
            domain.Observation = model.Observation;
            domain.TimeSpent = model.TimeSpent;
            domain.DueDate = model.DueDate;

            if (model.Status == TaskStatusEnum.Finished)
                domain.CompletedDate = DateTime.Now;

            _repository.Update(domain);

            var success = await _repository.SaveChangesAsync();

            return success
                ? ReturnData.ReturnSuccess(Localizer.GetString(LocalizationDictionary.SuccessWhenUpdating))
                : ReturnData.ReturnError(Localizer.GetString(LocalizationDictionary.ErrorWhenUpdating));
        }

        /// <summary>
        /// Deleta uma tarefa
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<ReturnData> Delete(Guid id)
        {
            if (id == Guid.Empty)
                return ReturnData.ReturnError(Localizer.GetString(LocalizationDictionary.IncompleteData));

            var domain = await _repository.GetAsync(id);

            if (domain == default)
                return ReturnData.ReturnError(Localizer.GetString(LocalizationDictionary.RecordNotFound));

            _repository.Delete(domain);

            var success = await _repository.SaveChangesAsync();

            return success
                ? ReturnData.ReturnSuccess(Localizer.GetString(LocalizationDictionary.SuccessWhenDeleting))
                : ReturnData.ReturnError(Localizer.GetString(LocalizationDictionary.ErrorWhenDeleting));
        }
        
        /// <summary>
        /// Retorna os dados de uma tarefa
        /// </summary>
        /// <returns></returns>
        public async Task<ReturnData<TaskReturn>> Get(Guid id)
        {
            if (id == Guid.Empty)
                return ReturnData<TaskReturn>.ReturnError(Localizer.GetString(LocalizationDictionary.IncompleteData));

            var domain = await _repository.GetAsync(id);

            if (domain == default)
                return ReturnData<TaskReturn>.ReturnError(Localizer.GetString(LocalizationDictionary.RecordNotFound));

            return new ReturnData<TaskReturn>
            {
                Success = true,
                Data = new TaskReturn
                {
                    Id = domain.Id,
                    Status = domain.Status,
                    Description = domain.Description,
                    Observation = domain.Observation,
                    CreationDate = domain.CreationDate,
                    TimeSpent = domain.TimeSpent,
                    DueDate = domain.DueDate,
                    CompletedDate = domain.CompletedDate
                }
            };
        }
        
        /// <summary>
        /// Retorna os dados de uma tarefa
        /// </summary>
        /// <returns></returns>
        public async Task<ReturnDataPaged<TaskReturn>> GetPaged(GetPagedModel model)
        {
            
            var filter = new FilterBy<Models.Entities.Task>(model.Filter)
                .AddExpression(x => x.UserId == Context.UserId);

            return await _repository
                .GetPagedAsync(
                    filter: filter,
                    select: s => new TaskReturn
                    {
                        Id = s.Id,
                        Status = s.Status,
                        Description = s.Description,
                        Observation = s.Observation,
                        CreationDate = s.CreationDate,
                        TimeSpent = s.TimeSpent,
                        DueDate = s.DueDate,
                        CompletedDate = s.CompletedDate
                    },
                    orderBy: x => x.CreationDate,
                    page: model.Page,
                    pageSize: model.PageSize
                );
        }
    }
}