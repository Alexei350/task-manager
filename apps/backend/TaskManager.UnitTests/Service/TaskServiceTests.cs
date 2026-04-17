using FakeItEasy;
using TaskManager.Models.Base;
using TaskManager.Models.Enums;
using TaskManager.Models.Request.Base;
using TaskManager.Models.Request.Task;
using TaskManager.Models.Return;
using TaskManager.Repository;
using TaskManager.Service;
using TaskManager.Service.Base;
using TaskManager.Utils.i18n;
using Task = System.Threading.Tasks.Task;
using TaskEntity = TaskManager.Models.Entities.Task;

namespace TaskManager.UnitTests.Service;

public class TaskServiceTests
{
    private readonly TaskService _service;
    private readonly IResourceStringLocalizer _localizer;
    private readonly ITaskRepository _taskRepository;
    private readonly IServiceProvider _serviceProvider;
    private readonly IRequestContext _requestContext;

    public TaskServiceTests()
    {
        //Dependências
        _localizer = A.Fake<IResourceStringLocalizer>();
        _taskRepository = A.Fake<ITaskRepository>();
        _requestContext = A.Fake<IRequestContext>();
        _serviceProvider = A.Fake<IServiceProvider>();

        A.CallTo(() => _serviceProvider.GetService(typeof(IResourceStringLocalizer)))
            .Returns(_localizer);
        A.CallTo(() => _serviceProvider.GetService(typeof(ITaskRepository)))
            .Returns(_taskRepository);
        A.CallTo(() => _serviceProvider.GetService(typeof(IRequestContext)))
            .Returns(_requestContext);

        var userId = Guid.NewGuid();
        A.CallTo(() => _requestContext.UserId).Returns(userId);

        //SUT
        _service = new TaskService(_serviceProvider);
    }

    #region Create Tests

    [Fact]
    public async Task Create_WithValidData_ShouldReturnSuccess()
    {
        //Parametrização
        var model = new CreateTaskModel
        {
            Description = "Test task",
            Status = TaskStatusEnum.Pending,
            Observation = "Test observation",
            TimeSpent = TimeSpan.FromMinutes(120),
            DueDate = DateTime.Now.AddDays(7)
        };

        var successMessage = "Criado com sucesso";
        A.CallTo(() => _localizer.GetString(LocalizationDictionary.SuccessWhenCreating))
            .Returns(successMessage);
        A.CallTo(() => _taskRepository.SaveChangesAsync())
            .Returns(true);

        //Ação
        var response = await _service.Create(model);

        //Validações
        Assert.NotNull(response);
        Assert.True(response.Success);
        Assert.Single(response.Messages);
        Assert.Equal(successMessage, response.Messages[0].Message);
        Assert.NotNull(response.Data);
        
        var taskReturn = response.Data;
        Assert.NotNull(taskReturn);
        Assert.Equal(model.Description, taskReturn.Description);
        Assert.Equal(model.Status, taskReturn.Status);
        Assert.Equal(model.Observation, taskReturn.Observation);
        Assert.Equal(model.TimeSpent, taskReturn.TimeSpent);
        Assert.Equal(model.DueDate, taskReturn.DueDate);

        A.CallTo(() => _taskRepository.CreateAsync(A<TaskEntity>._)).MustHaveHappenedOnceExactly();
        A.CallTo(() => _taskRepository.SaveChangesAsync()).MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task Create_WithNullModel_ShouldReturnError()
    {
        //Parametrização
        var errorMessage = "Dados incompletos";
        A.CallTo(() => _localizer.GetString(LocalizationDictionary.IncompleteData))
            .Returns(errorMessage);

        //Ação
        var response = await _service.Create(null);

        //Validações
        Assert.NotNull(response);
        Assert.False(response.Success);
        Assert.Single(response.Messages);
        Assert.Equal(errorMessage, response.Messages[0].Message);

        A.CallTo(() => _taskRepository.CreateAsync(A<TaskEntity>._)).MustNotHaveHappened();
    }

    [Fact]
    public async Task Create_WithEmptyDescription_ShouldReturnError()
    {
        //Parametrização
        var model = new CreateTaskModel
        {
            Description = "",
            Status = TaskStatusEnum.Pending
        };

        var errorMessage = "Dados incompletos";
        A.CallTo(() => _localizer.GetString(LocalizationDictionary.IncompleteData))
            .Returns(errorMessage);

        //Ação
        var response = await _service.Create(model);

        //Validações
        Assert.NotNull(response);
        Assert.False(response.Success);
        Assert.Single(response.Messages);
        Assert.Equal(errorMessage, response.Messages[0].Message);

        A.CallTo(() => _taskRepository.CreateAsync(A<TaskEntity>._)).MustNotHaveHappened();
    }

    [Fact]
    public async Task Create_WithInvalidStatus_ShouldReturnError()
    {
        //Parametrização
        var model = new CreateTaskModel
        {
            Description = "Test task",
            Status = (TaskStatusEnum)999
        };

        var errorMessage = "Status inválido";
        A.CallTo(() => _localizer.GetString(LocalizationDictionary.InvalidStatus))
            .Returns(errorMessage);

        //Ação
        var response = await _service.Create(model);

        //Validações
        Assert.NotNull(response);
        Assert.False(response.Success);
        Assert.Single(response.Messages);
        Assert.Equal(errorMessage, response.Messages[0].Message);

        A.CallTo(() => _taskRepository.CreateAsync(A<TaskEntity>._)).MustNotHaveHappened();
    }

    [Fact]
    public async Task Create_WhenSaveFails_ShouldReturnError()
    {
        //Parametrização
        var model = new CreateTaskModel
        {
            Description = "Test task",
            Status = TaskStatusEnum.Pending
        };

        var errorMessage = "Erro ao criar";
        A.CallTo(() => _localizer.GetString(LocalizationDictionary.ErrorWhenCreating))
            .Returns(errorMessage);
        A.CallTo(() => _taskRepository.SaveChangesAsync())
            .Returns(false);

        //Ação
        var response = await _service.Create(model);

        //Validações
        Assert.NotNull(response);
        Assert.False(response.Success);
        Assert.Single(response.Messages);
        Assert.Equal(errorMessage, response.Messages[0].Message);

        A.CallTo(() => _taskRepository.CreateAsync(A<TaskEntity>._)).MustHaveHappenedOnceExactly();
    }

    #endregion

    #region Update Tests

    [Fact]
    public async Task Update_WithValidData_ShouldReturnSuccess()
    {
        //Parametrização
        var taskId = Guid.NewGuid();
        var model = new UpdateTaskModel
        {
            Id = taskId,
            Description = "Updated task",
            Status = TaskStatusEnum.InProgress,
            Observation = "Updated observation",
            TimeSpent = TimeSpan.FromMinutes(240),
            DueDate = DateTime.Now.AddDays(5)
        };

        var existingTask = new TaskEntity
        {
            Id = taskId,
            Description = "Old description",
            Status = TaskStatusEnum.Pending
        };

        var successMessage = "Atualizado com sucesso";
        A.CallTo(() => _localizer.GetString(LocalizationDictionary.SuccessWhenUpdating))
            .Returns(successMessage);
        A.CallTo(() => _taskRepository.GetForUpdateAsync(taskId))
            .Returns(existingTask);
        A.CallTo(() => _taskRepository.SaveChangesAsync())
            .Returns(true);

        //Ação
        var response = await _service.Update(model);

        //Validações
        Assert.NotNull(response);
        Assert.True(response.Success);
        Assert.Single(response.Messages);
        Assert.Equal(successMessage, response.Messages[0].Message);

        A.CallTo(() => _taskRepository.Update(A<TaskEntity>.That.Matches(t => 
            t.Description == model.Description && 
            t.Status == model.Status)))
            .MustHaveHappenedOnceExactly();
        A.CallTo(() => _taskRepository.SaveChangesAsync()).MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task Update_WithNullModel_ShouldReturnError()
    {
        //Parametrização
        var errorMessage = "Dados incompletos";
        A.CallTo(() => _localizer.GetString(LocalizationDictionary.IncompleteData))
            .Returns(errorMessage);

        //Ação
        var response = await _service.Update(null);

        //Validações
        Assert.NotNull(response);
        Assert.False(response.Success);
        Assert.Single(response.Messages);
        Assert.Equal(errorMessage, response.Messages[0].Message);

        A.CallTo(() => _taskRepository.Update(A<TaskEntity>._)).MustNotHaveHappened();
    }

    [Fact]
    public async Task Update_WithEmptyId_ShouldReturnError()
    {
        //Parametrização
        var model = new UpdateTaskModel
        {
            Id = Guid.Empty,
            Description = "Test task",
            Status = TaskStatusEnum.Pending
        };

        var errorMessage = "Dados incompletos";
        A.CallTo(() => _localizer.GetString(LocalizationDictionary.IncompleteData))
            .Returns(errorMessage);

        //Ação
        var response = await _service.Update(model);

        //Validações
        Assert.NotNull(response);
        Assert.False(response.Success);
        Assert.Single(response.Messages);
        Assert.Equal(errorMessage, response.Messages[0].Message);

        A.CallTo(() => _taskRepository.Update(A<TaskEntity>._)).MustNotHaveHappened();
    }

    [Fact]
    public async Task Update_WithInvalidStatus_ShouldReturnError()
    {
        //Parametrização
        var model = new UpdateTaskModel
        {
            Id = Guid.NewGuid(),
            Description = "Test task",
            Status = (TaskStatusEnum)999
        };

        var errorMessage = "Status inválido";
        A.CallTo(() => _localizer.GetString(LocalizationDictionary.InvalidStatus))
            .Returns(errorMessage);

        //Ação
        var response = await _service.Update(model);

        //Validações
        Assert.NotNull(response);
        Assert.False(response.Success);
        Assert.Single(response.Messages);
        Assert.Equal(errorMessage, response.Messages[0].Message);

        A.CallTo(() => _taskRepository.Update(A<TaskEntity>._)).MustNotHaveHappened();
    }

    [Fact]
    public async Task Update_WithNonExistentTask_ShouldReturnError()
    {
        //Parametrização
        var taskId = Guid.NewGuid();
        var model = new UpdateTaskModel
        {
            Id = taskId,
            Description = "Test task",
            Status = TaskStatusEnum.Pending
        };

        var errorMessage = "Registro não encontrado";
        A.CallTo(() => _localizer.GetString(LocalizationDictionary.RecordNotFound))
            .Returns(errorMessage);
        A.CallTo(() => _taskRepository.GetForUpdateAsync(taskId))
            .Returns(Task.FromResult<TaskEntity>(null!));

        //Ação
        var response = await _service.Update(model);

        //Validações
        Assert.NotNull(response);
        Assert.False(response.Success);
        Assert.Single(response.Messages);
        Assert.Equal(errorMessage, response.Messages[0].Message);

        A.CallTo(() => _taskRepository.Update(A<TaskEntity>._)).MustNotHaveHappened();
    }

    [Fact]
    public async Task Update_WithFinishedStatus_ShouldSetCompletedDate()
    {
        //Parametrização
        var taskId = Guid.NewGuid();
        var model = new UpdateTaskModel
        {
            Id = taskId,
            Description = "Completed task",
            Status = TaskStatusEnum.Finished
        };

        var existingTask = new TaskEntity
        {
            Id = taskId,
            Description = "Test task",
            Status = TaskStatusEnum.InProgress,
            CompletedDate = null
        };

        var successMessage = "Atualizado com sucesso";
        A.CallTo(() => _localizer.GetString(LocalizationDictionary.SuccessWhenUpdating))
            .Returns(successMessage);
        A.CallTo(() => _taskRepository.GetForUpdateAsync(taskId))
            .Returns(existingTask);
        A.CallTo(() => _taskRepository.SaveChangesAsync())
            .Returns(true);

        //Ação
        var response = await _service.Update(model);

        //Validações
        Assert.NotNull(response);
        Assert.True(response.Success);

        A.CallTo(() => _taskRepository.Update(A<TaskEntity>.That.Matches(t => 
            t.Status == TaskStatusEnum.Finished && 
            t.CompletedDate.HasValue)))
            .MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task Update_WhenSaveFails_ShouldReturnError()
    {
        //Parametrização
        var taskId = Guid.NewGuid();
        var model = new UpdateTaskModel
        {
            Id = taskId,
            Description = "Test task",
            Status = TaskStatusEnum.Pending
        };

        var existingTask = new TaskEntity
        {
            Id = taskId,
            Description = "Old description",
            Status = TaskStatusEnum.Pending
        };

        var errorMessage = "Erro ao atualizar";
        A.CallTo(() => _localizer.GetString(LocalizationDictionary.ErrorWhenUpdating))
            .Returns(errorMessage);
        A.CallTo(() => _taskRepository.GetForUpdateAsync(taskId))
            .Returns(existingTask);
        A.CallTo(() => _taskRepository.SaveChangesAsync())
            .Returns(false);

        //Ação
        var response = await _service.Update(model);

        //Validações
        Assert.NotNull(response);
        Assert.False(response.Success);
        Assert.Single(response.Messages);
        Assert.Equal(errorMessage, response.Messages[0].Message);
    }

    #endregion

    #region Delete Tests

    [Fact]
    public async Task Delete_WithValidId_ShouldReturnSuccess()
    {
        //Parametrização
        var taskId = Guid.NewGuid();
        var existingTask = new TaskEntity
        {
            Id = taskId,
            Description = "Test task",
            Status = TaskStatusEnum.Pending
        };

        var successMessage = "Deletado com sucesso";
        A.CallTo(() => _localizer.GetString(LocalizationDictionary.SuccessWhenDeleting))
            .Returns(successMessage);
        A.CallTo(() => _taskRepository.GetAsync(taskId))
            .Returns(existingTask);
        A.CallTo(() => _taskRepository.SaveChangesAsync())
            .Returns(true);

        //Ação
        var response = await _service.Delete(taskId);

        //Validações
        Assert.NotNull(response);
        Assert.True(response.Success);
        Assert.Single(response.Messages);
        Assert.Equal(successMessage, response.Messages[0].Message);

        A.CallTo(() => _taskRepository.Delete(existingTask)).MustHaveHappenedOnceExactly();
        A.CallTo(() => _taskRepository.SaveChangesAsync()).MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task Delete_WithEmptyId_ShouldReturnError()
    {
        //Parametrização
        var errorMessage = "Dados incompletos";
        A.CallTo(() => _localizer.GetString(LocalizationDictionary.IncompleteData))
            .Returns(errorMessage);

        //Ação
        var response = await _service.Delete(Guid.Empty);

        //Validações
        Assert.NotNull(response);
        Assert.False(response.Success);
        Assert.Single(response.Messages);
        Assert.Equal(errorMessage, response.Messages[0].Message);

        A.CallTo(() => _taskRepository.Delete(A<TaskEntity>._)).MustNotHaveHappened();
    }

    [Fact]
    public async Task Delete_WithNonExistentTask_ShouldReturnError()
    {
        //Parametrização
        var taskId = Guid.NewGuid();
        var errorMessage = "Registro não encontrado";
        A.CallTo(() => _localizer.GetString(LocalizationDictionary.RecordNotFound))
            .Returns(errorMessage);
        A.CallTo(() => _taskRepository.GetAsync(taskId))
            .Returns(Task.FromResult<TaskEntity>(null!));

        //Ação
        var response = await _service.Delete(taskId);

        //Validações
        Assert.NotNull(response);
        Assert.False(response.Success);
        Assert.Single(response.Messages);
        Assert.Equal(errorMessage, response.Messages[0].Message);

        A.CallTo(() => _taskRepository.Delete(A<TaskEntity>._)).MustNotHaveHappened();
    }

    [Fact]
    public async Task Delete_WhenSaveFails_ShouldReturnError()
    {
        //Parametrização
        var taskId = Guid.NewGuid();
        var existingTask = new TaskEntity
        {
            Id = taskId,
            Description = "Test task"
        };

        var errorMessage = "Erro ao deletar";
        A.CallTo(() => _localizer.GetString(LocalizationDictionary.ErrorWhenDeleting))
            .Returns(errorMessage);
        A.CallTo(() => _taskRepository.GetAsync(taskId))
            .Returns(existingTask);
        A.CallTo(() => _taskRepository.SaveChangesAsync())
            .Returns(false);

        //Ação
        var response = await _service.Delete(taskId);

        //Validações
        Assert.NotNull(response);
        Assert.False(response.Success);
        Assert.Single(response.Messages);
        Assert.Equal(errorMessage, response.Messages[0].Message);
    }

    #endregion

    #region Get Tests

    [Fact]
    public async Task Get_WithValidId_ShouldReturnTaskData()
    {
        //Parametrização
        var taskId = Guid.NewGuid();
        var existingTask = new TaskEntity
        {
            Id = taskId,
            Description = "Test task",
            Status = TaskStatusEnum.Pending,
            Observation = "Test observation",
            CreationDate = DateTime.Now,
            TimeSpent = TimeSpan.FromMinutes(120),
            DueDate = DateTime.Now.AddDays(7)
        };

        A.CallTo(() => _taskRepository.GetAsync(taskId))
            .Returns(existingTask);

        //Ação
        var response = await _service.Get(taskId);

        //Validações
        Assert.NotNull(response);
        Assert.True(response.Success);
        Assert.NotNull(response.Data);
        Assert.Equal(existingTask.Id, response.Data.Id);
        Assert.Equal(existingTask.Description, response.Data.Description);
        Assert.Equal(existingTask.Status, response.Data.Status);
        Assert.Equal(existingTask.Observation, response.Data.Observation);
    }

    [Fact]
    public async Task Get_WithEmptyId_ShouldReturnError()
    {
        //Parametrização
        var errorMessage = "Dados incompletos";
        A.CallTo(() => _localizer.GetString(LocalizationDictionary.IncompleteData))
            .Returns(errorMessage);

        //Ação
        var response = await _service.Get(Guid.Empty);

        //Validações
        Assert.NotNull(response);
        Assert.False(response.Success);
        Assert.Single(response.Messages);
        Assert.Equal(errorMessage, response.Messages[0].Message);

        A.CallTo(() => _taskRepository.GetAsync(A<Guid>._)).MustNotHaveHappened();
    }

    [Fact]
    public async Task Get_WithNonExistentTask_ShouldReturnError()
    {
        //Parametrização
        var taskId = Guid.NewGuid();
        var errorMessage = "Registro não encontrado";
        A.CallTo(() => _localizer.GetString(LocalizationDictionary.RecordNotFound))
            .Returns(errorMessage);
        A.CallTo(() => _taskRepository.GetAsync(taskId))
            .Returns(Task.FromResult<TaskEntity>(null!));

        //Ação
        var response = await _service.Get(taskId);

        //Validações
        Assert.NotNull(response);
        Assert.False(response.Success);
        Assert.Single(response.Messages);
        Assert.Equal(errorMessage, response.Messages[0].Message);
    }

    #endregion

    #region GetPaged Tests

    [Fact]
    public async Task GetPaged_WithValidModel_ShouldReturnPagedData()
    {
        //Parametrização
        var model = new GetPagedModel
        {
            Page = 1,
            PageSize = 10,
            Filter = "test"
        };

        var pagedResult = new ReturnDataPaged<TaskReturn>
        {
            Success = true,
            Data = new List<TaskReturn>
            {
                new TaskReturn
                {
                    Id = Guid.NewGuid(),
                    Description = "Task 1",
                    Status = TaskStatusEnum.Pending
                },
                new TaskReturn
                {
                    Id = Guid.NewGuid(),
                    Description = "Task 2",
                    Status = TaskStatusEnum.InProgress
                }
            },
            TotalPages = 1,
            TotalRecords = 2
        };

        A.CallTo(() => _taskRepository.GetPagedAsync(
                A<Models.Base.Query.FilterBy<TaskEntity>>._,
                A<System.Linq.Expressions.Expression<Func<TaskEntity, TaskReturn>>>._,
                A<System.Linq.Expressions.Expression<Func<TaskEntity, object>>>._,
                A<int>._,
                A<int>._))
            .Returns(pagedResult);

        //Ação
        var response = await _service.GetPaged(model);

        //Validações
        Assert.NotNull(response);
        Assert.True(response.Success);
        Assert.NotNull(response.Data);
        Assert.Equal(2, response.Data.Count);
        Assert.Equal(2, response.TotalRecords);

        A.CallTo(() => _taskRepository.GetPagedAsync(
            A<Models.Base.Query.FilterBy<TaskEntity>>._,
            A<System.Linq.Expressions.Expression<Func<TaskEntity, TaskReturn>>>._,
            A<System.Linq.Expressions.Expression<Func<TaskEntity, object>>>._,
            A<int>._,
            A<int>._)).MustHaveHappenedOnceExactly();
    }

    #endregion
}
