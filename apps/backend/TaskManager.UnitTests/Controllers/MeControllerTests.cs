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

public class MeControllerTests
{
    private readonly MeController _controller;
    private readonly IMeService _service;
    private readonly IServiceProvider _serviceProvider;
    private readonly IResourceStringLocalizer _localizer;
    private readonly ILogger<BaseController> _logger;

    public MeControllerTests()
    {
        _service = A.Fake<IMeService>();
        _serviceProvider = A.Fake<IServiceProvider>();
        _localizer = A.Fake<IResourceStringLocalizer>();
        _logger = A.Fake<ILogger<BaseController>>();

        A.CallTo(() => _serviceProvider.GetService(typeof(IMeService))).Returns(_service);
        A.CallTo(() => _serviceProvider.GetService(typeof(IResourceStringLocalizer))).Returns(_localizer);
        A.CallTo(() => _serviceProvider.GetService(typeof(ILogger<BaseController>))).Returns(_logger);

        _controller = new MeController(_serviceProvider);
    }

    [Fact]
    public void Get_ShouldReturnOk_WhenServiceReturnsSuccess()
    {
        // Arrange
        var returnData = new ReturnData<UserReturn> { Success = true };
        A.CallTo(() => _service.Get()).Returns(returnData);

        // Act
        var result = _controller.Get();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(returnData, okResult.Value);
    }

    [Fact]
    public void Get_ShouldReturnBadRequest_WhenServiceReturnsFailure()
    {
        // Arrange
        var returnData = new ReturnData<UserReturn> { Success = false };
        A.CallTo(() => _service.Get()).Returns(returnData);

        // Act
        var result = _controller.Get();

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal(returnData, badRequestResult.Value);
    }

    [Fact]
    public async Task Update_ShouldReturnOk_WhenServiceReturnsSuccess()
    {
        // Arrange
        var model = new UpdateUserModel();
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
        var model = new UpdateUserModel();
        var returnData = new ReturnData { Success = false };
        A.CallTo(() => _service.Update(model)).Returns(returnData);

        // Act
        var result = await _controller.Update(model);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal(returnData, badRequestResult.Value);
    }
}
