using FakeItEasy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using TaskManager.Controllers;
using TaskManager.Controllers.Base;
using TaskManager.Models.Base;
using TaskManager.Models.Request.Base;
using TaskManager.Models.Request.Task;
using TaskManager.Models.Return;
using TaskManager.Service.Interfaces;
using TaskManager.Utils.i18n;

namespace TaskManager.UnitTests.Controllers;

public class TaskControllerTests
{
    private readonly TaskController _controller;
    private readonly ITaskService _service;
    private readonly IServiceProvider _serviceProvider;
    private readonly IResourceStringLocalizer _localizer;
    private readonly ILogger<BaseController> _logger;

    public TaskControllerTests()
    {
        _service = A.Fake<ITaskService>();
        _serviceProvider = A.Fake<IServiceProvider>();
        _localizer = A.Fake<IResourceStringLocalizer>();
        _logger = A.Fake<ILogger<BaseController>>();

        A.CallTo(() => _serviceProvider.GetService(typeof(ITaskService))).Returns(_service);
        A.CallTo(() => _serviceProvider.GetService(typeof(IResourceStringLocalizer))).Returns(_localizer);
        A.CallTo(() => _serviceProvider.GetService(typeof(ILogger<BaseController>))).Returns(_logger);

        _controller = new TaskController(_serviceProvider);
    }

    [Fact]
    public async Task Create_ShouldReturnOk_WhenServiceReturnsSuccess()
    {
        // Arrange
        var model = new CreateTaskModel();
        var returnData = new ReturnData<TaskReturn> { Success = true };
        A.CallTo(() => _service.Create(model)).Returns(returnData);

        // Act
        var result = await _controller.Create(model);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(returnData, okResult.Value);
    }

    [Fact]
    public async Task Create_ShouldReturnBadRequest_WhenServiceReturnsFailure()
    {
        // Arrange
        var model = new CreateTaskModel();
        var returnData = new ReturnData<TaskReturn> { Success = false };
        A.CallTo(() => _service.Create(model)).Returns(returnData);

        // Act
        var result = await _controller.Create(model);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal(returnData, badRequestResult.Value);
    }

    [Fact]
    public async Task Update_ShouldReturnOk_WhenServiceReturnsSuccess()
    {
        // Arrange
        var model = new UpdateTaskModel();
        var returnData = new ReturnData { Success = true };
        A.CallTo(() => _service.Update(model)).Returns(returnData);

        // Act
        var result = await _controller.Update(model);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(returnData, okResult.Value);
    }

    [Fact]
    public async Task Update_ShouldReturnBadRequest_WhenServiceReturnsFailure()
    {
        // Arrange
        var model = new UpdateTaskModel();
        var returnData = new ReturnData { Success = false };
        A.CallTo(() => _service.Update(model)).Returns(returnData);

        // Act
        var result = await _controller.Update(model);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal(returnData, badRequestResult.Value);
    }

    [Fact]
    public async Task Delete_ShouldReturnOk_WhenServiceReturnsSuccess()
    {
        // Arrange
        var id = Guid.NewGuid();
        var returnData = new ReturnData { Success = true };
        A.CallTo(() => _service.Delete(id)).Returns(returnData);

        // Act
        var result = await _controller.Delete(id);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(returnData, okResult.Value);
    }

    [Fact]
    public async Task Delete_ShouldReturnBadRequest_WhenServiceReturnsFailure()
    {
        // Arrange
        var id = Guid.NewGuid();
        var returnData = new ReturnData { Success = false };
        A.CallTo(() => _service.Delete(id)).Returns(returnData);

        // Act
        var result = await _controller.Delete(id);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal(returnData, badRequestResult.Value);
    }

    [Fact]
    public async Task Get_ById_ShouldReturnOk_WhenServiceReturnsSuccess()
    {
        // Arrange
        var id = Guid.NewGuid();
        var returnData = new ReturnData<TaskReturn> { Success = true };
        A.CallTo(() => _service.Get(id)).Returns(returnData);

        // Act
        var result = await _controller.Get(id);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(returnData, okResult.Value);
    }

    [Fact]
    public async Task Get_ById_ShouldReturnBadRequest_WhenServiceReturnsFailure()
    {
        // Arrange
        var id = Guid.NewGuid();
        var returnData = new ReturnData<TaskReturn> { Success = false };
        A.CallTo(() => _service.Get(id)).Returns(returnData);

        // Act
        var result = await _controller.Get(id);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal(returnData, badRequestResult.Value);
    }

    [Fact]
    public async Task Get_Paged_ShouldReturnOk_WhenServiceReturnsSuccess()
    {
        // Arrange
        var model = new GetPagedModel();
        var returnData = new ReturnDataPaged<TaskReturn> { Success = true };
        A.CallTo(() => _service.GetPaged(model)).Returns(returnData);

        // Act
        var result = await _controller.Get(model);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(returnData, okResult.Value);
    }

    [Fact]
    public async Task Get_Paged_ShouldReturnBadRequest_WhenServiceReturnsFailure()
    {
        // Arrange
        var model = new GetPagedModel();
        var returnData = new ReturnDataPaged<TaskReturn> { Success = false };
        A.CallTo(() => _service.GetPaged(model)).Returns(returnData);

        // Act
        var result = await _controller.Get(model);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal(returnData, badRequestResult.Value);
    }
}
