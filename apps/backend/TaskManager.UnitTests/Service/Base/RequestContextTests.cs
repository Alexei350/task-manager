using System.Security.Claims;
using FakeItEasy;
using Microsoft.AspNetCore.Http;
using TaskManager.Service.Base;
using TaskManager.Utils.i18n;

namespace TaskManager.UnitTests.Service.Base;

public class RequestContextTests
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IResourceStringLocalizer _localizer;
    private readonly IServiceProvider _serviceProvider;
    private readonly RequestContext _requestContext;

    public RequestContextTests()
    {
        _httpContextAccessor = A.Fake<IHttpContextAccessor>();
        _localizer = A.Fake<IResourceStringLocalizer>();
        _serviceProvider = A.Fake<IServiceProvider>();

        _requestContext = new RequestContext(_httpContextAccessor, _localizer, _serviceProvider);
    }

    [Fact]
    public void IsAuthenticated_ShouldReturnTrue_WhenUserIsAuthenticated()
    {
        var context = new DefaultHttpContext();
        var claims = new List<Claim> { new Claim(ClaimTypes.Name, "test") };
        var identity = new ClaimsIdentity(claims, "TestAuthType");
        context.User = new ClaimsPrincipal(identity);
        
        A.CallTo(() => _httpContextAccessor.HttpContext).Returns(context);

        Assert.True(_requestContext.IsAuthenticated);
    }

    [Fact]
    public void IsAuthenticated_ShouldReturnFalse_WhenUserIsNotAuthenticated()
    {
        var context = new DefaultHttpContext();
        context.User = new ClaimsPrincipal(new ClaimsIdentity()); // Not authenticated
        
        A.CallTo(() => _httpContextAccessor.HttpContext).Returns(context);

        Assert.False(_requestContext.IsAuthenticated);
    }

    [Fact]
    public void IsAuthenticated_ShouldThrowException_WhenHttpContextIsNull()
    {
        A.CallTo(() => _httpContextAccessor.HttpContext).Returns(null);
        A.CallTo(() => _localizer.GetString(LocalizationDictionary.UserNotFound)).Returns("User not found");

        Assert.Throws<ApplicationException>(() => _requestContext.IsAuthenticated);
    }

    [Fact]
    public void UserId_ShouldReturnUserId_WhenUserIsAuthenticated()
    {
        var userId = Guid.NewGuid();
        var context = new DefaultHttpContext();
        var claims = new List<Claim> { new Claim("UserId", userId.ToString()) };
        var identity = new ClaimsIdentity(claims, "TestAuthType");
        context.User = new ClaimsPrincipal(identity);
        
        A.CallTo(() => _httpContextAccessor.HttpContext).Returns(context);

        Assert.Equal(userId, _requestContext.UserId);
    }

    [Fact]
    public void UserId_ShouldThrowException_WhenUserIdClaimIsMissing()
    {
        var context = new DefaultHttpContext();
        var claims = new List<Claim>(); // No UserId claim
        var identity = new ClaimsIdentity(claims, "TestAuthType");
        context.User = new ClaimsPrincipal(identity);
        
        A.CallTo(() => _httpContextAccessor.HttpContext).Returns(context);
        A.CallTo(() => _localizer.GetString(LocalizationDictionary.UserNotFound)).Returns("User not found");

        Assert.Throws<ApplicationException>(() => _requestContext.UserId);
    }
}
