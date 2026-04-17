using FakeItEasy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using TaskManager.Controllers;
using TaskManager.Controllers.Base;
using TaskManager.Models.Base;
using TaskManager.Models.Request.Authentication;
using TaskManager.Models.Return;
using TaskManager.Service.Interfaces;
using TaskManager.Utils.i18n;

namespace TaskManager.UnitTests.Controllers;

public class AuthenticationControllerTests
{
    private readonly AuthenticationController _controller;
    private readonly IAuthenticationService _service;
    private readonly IServiceProvider _serviceProvider;
    private readonly IResourceStringLocalizer _localizer;
    private readonly ILogger<BaseController> _logger;

    public AuthenticationControllerTests()
    {
        _service = A.Fake<IAuthenticationService>();
        _serviceProvider = A.Fake<IServiceProvider>();
        _localizer = A.Fake<IResourceStringLocalizer>();
        _logger = A.Fake<ILogger<BaseController>>();

        A.CallTo(() => _serviceProvider.GetService(typeof(IAuthenticationService))).Returns(_service);
        A.CallTo(() => _serviceProvider.GetService(typeof(IResourceStringLocalizer))).Returns(_localizer);
        A.CallTo(() => _serviceProvider.GetService(typeof(ILogger<BaseController>))).Returns(_logger);

        _controller = new AuthenticationController(_serviceProvider);
    }

    [Fact]
    public async Task GenerateToken_ShouldReturnOk_WhenServiceReturnsSuccess()
    {
        // Arrange
        var model = new LoginModel();
        var returnData = new ReturnData<TokenReturn> { Success = true };
        A.CallTo(() => _service.GenerateToken(model)).Returns(returnData);

        // Act
        var result = await _controller.GenerateToken(model);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(returnData, okResult.Value);
    }

    [Fact]
    public async Task GenerateToken_ShouldReturnUnauthorized_WhenServiceReturnsFailure()
    {
        // Arrange
        var model = new LoginModel();
        var returnData = new ReturnData<TokenReturn> { Success = false };
        A.CallTo(() => _service.GenerateToken(model)).Returns(returnData);

        // Act
        var result = await _controller.GenerateToken(model);

        // Assert
        var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result);
        Assert.Equal(returnData, unauthorizedResult.Value);
    }

    [Fact]
    public async Task RefreshToken_ShouldReturnOk_WhenServiceReturnsSuccess()
    {
        // Arrange
        var model = new RefreshTokenModel();
        var returnData = new ReturnData<TokenReturn> { Success = true };
        A.CallTo(() => _service.RefreshToken(model)).Returns(returnData);

        // Act
        var result = await _controller.RefreshToken(model);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(returnData, okResult.Value);
    }

    [Fact]
    public async Task RefreshToken_ShouldReturnUnauthorized_WhenServiceReturnsFailure()
    {
        // Arrange
        var model = new RefreshTokenModel();
        var returnData = new ReturnData<TokenReturn> { Success = false };
        A.CallTo(() => _service.RefreshToken(model)).Returns(returnData);

        // Act
        var result = await _controller.RefreshToken(model);

        // Assert
        var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result);
        Assert.Equal(returnData, unauthorizedResult.Value);
    }

    [Fact]
    public async Task GoogleLogin_ShouldReturnOk_WhenServiceReturnsSuccess()
    {
        // Arrange
        var model = new GoogleLoginModel();
        var returnData = new ReturnData<TokenReturn> { Success = true };
        A.CallTo(() => _service.GoogleLogin(model)).Returns(returnData);

        // Act
        var result = await _controller.GoogleLogin(model);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(returnData, okResult.Value);
    }

    [Fact]
    public async Task GoogleLogin_ShouldReturnUnauthorized_WhenServiceReturnsFailure()
    {
        // Arrange
        var model = new GoogleLoginModel();
        var returnData = new ReturnData<TokenReturn> { Success = false };
        A.CallTo(() => _service.GoogleLogin(model)).Returns(returnData);

        // Act
        var result = await _controller.GoogleLogin(model);

        // Assert
        var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result);
        Assert.Equal(returnData, unauthorizedResult.Value);
    }
}
