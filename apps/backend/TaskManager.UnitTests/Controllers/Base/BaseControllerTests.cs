using FakeItEasy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using TaskManager.Controllers.Base;
using TaskManager.Models.Base;
using TaskManager.Utils.i18n;

namespace TaskManager.UnitTests.Controllers.Base;

public class TestController : BaseController
{
    public TestController(IServiceProvider provider) : base(provider)
    {
    }

    public async Task<IActionResult> TestDefaultHandlerAsync(Func<Task<IActionResult>> action)
    {
        return await DefaultHandlerAsync(action);
    }

    public IActionResult TestDefaultHandler(Func<IActionResult> action)
    {
        return DefaultHandler(action);
    }
}

public class BaseControllerTests
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IResourceStringLocalizer _localizer;
    private readonly ILogger<BaseController> _logger;
    private readonly TestController _controller;

    public BaseControllerTests()
    {
        _serviceProvider = A.Fake<IServiceProvider>();
        _localizer = A.Fake<IResourceStringLocalizer>();
        _logger = A.Fake<ILogger<BaseController>>();

        A.CallTo(() => _serviceProvider.GetService(typeof(IResourceStringLocalizer))).Returns(_localizer);
        A.CallTo(() => _serviceProvider.GetService(typeof(ILogger<BaseController>))).Returns(_logger);

        _controller = new TestController(_serviceProvider);
    }

    [Fact]
    public async Task DefaultHandlerAsync_ShouldReturnResult_WhenActionSucceeds()
    {
        var expectedResult = new OkResult();
        var result = await _controller.TestDefaultHandlerAsync(async () => expectedResult);

        Assert.Same(expectedResult, result);
    }

    [Fact]
    public async Task DefaultHandlerAsync_ShouldReturnBadRequest_WhenActionThrowsException()
    {
        var exception = new Exception("Test exception");
        A.CallTo(() => _localizer.GetString(LocalizationDictionary.UnexpectedError)).Returns("Unexpected error");

        var result = await _controller.TestDefaultHandlerAsync(async () => throw exception);

        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        var returnData = Assert.IsType<ReturnData>(badRequestResult.Value);
        
        Assert.False(returnData.Success);
    }

    [Fact]
    public void DefaultHandler_ShouldReturnResult_WhenActionSucceeds()
    {
        var expectedResult = new OkResult();
        var result = _controller.TestDefaultHandler(() => expectedResult);

        Assert.Same(expectedResult, result);
    }

    [Fact]
    public void DefaultHandler_ShouldReturnBadRequest_WhenActionThrowsException()
    {
        var exception = new Exception("Test exception");
        A.CallTo(() => _localizer.GetString(LocalizationDictionary.UnexpectedError)).Returns("Unexpected error");

        var result = _controller.TestDefaultHandler(() => throw exception);

        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        var returnData = Assert.IsType<ReturnData>(badRequestResult.Value);
        Assert.False(returnData.Success);
    }
}
