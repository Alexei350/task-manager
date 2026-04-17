using FakeItEasy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using TaskManager.Controllers;
using TaskManager.Controllers.Base;
using TaskManager.Models.Base;
using TaskManager.Models.Request.User;
using TaskManager.Models.Return;
using TaskManager.Service.Interfaces;
using TaskManager.Utils.i18n;

namespace TaskManager.UnitTests.Controllers;

public class UserControllerTests
{
    private readonly UserController _controller;
    private readonly IUserService _service;
    private readonly IServiceProvider _serviceProvider;
    private readonly IResourceStringLocalizer _localizer;
    private readonly ILogger<BaseController> _logger;

    public UserControllerTests()
    {
        _service = A.Fake<IUserService>();
        _serviceProvider = A.Fake<IServiceProvider>();
        _localizer = A.Fake<IResourceStringLocalizer>();
        _logger = A.Fake<ILogger<BaseController>>();

        A.CallTo(() => _serviceProvider.GetService(typeof(IUserService))).Returns(_service);
        A.CallTo(() => _serviceProvider.GetService(typeof(IResourceStringLocalizer))).Returns(_localizer);
        A.CallTo(() => _serviceProvider.GetService(typeof(ILogger<BaseController>))).Returns(_logger);

        _controller = new UserController(_serviceProvider);
    }

    [Fact]
    public async Task Create_ShouldReturnOk_WhenServiceReturnsSuccess()
    {
        // Arrange
        var model = new CreateUserModel();
        var returnData = new ReturnData { Success = true };
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
        var model = new CreateUserModel();
        var returnData = new ReturnData { Success = false };
        A.CallTo(() => _service.Create(model)).Returns(returnData);

        // Act
        var result = await _controller.Create(model);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal(returnData, badRequestResult.Value);
    }
}
