using System.Security.Claims;
using TaskManager.Service.Base;

namespace TaskManager.UnitTests.Service.Base;

public class ClaimsPrincipalExtensionsTests
{
    [Fact]
    public void GetUserId_ShouldReturnUserId_WhenClaimExists()
    {
        var userId = Guid.NewGuid();
        var claims = new List<Claim>
        {
            new Claim("UserId", userId.ToString())
        };
        var identity = new ClaimsIdentity(claims);
        var principal = new ClaimsPrincipal(identity);

        var result = principal.GetUserId();

        Assert.Equal(userId, result);
    }

    [Fact]
    public void GetUserId_ShouldReturnNull_WhenClaimDoesNotExist()
    {
        var claims = new List<Claim>();
        var identity = new ClaimsIdentity(claims);
        var principal = new ClaimsPrincipal(identity);

        var result = principal.GetUserId();

        Assert.Null(result);
    }

    [Fact]
    public void GetUserId_ShouldReturnNull_WhenClaimIsNotGuid()
    {
        var claims = new List<Claim>
        {
            new Claim("UserId", "not-a-guid")
        };
        var identity = new ClaimsIdentity(claims);
        var principal = new ClaimsPrincipal(identity);

        var result = principal.GetUserId();

        Assert.Null(result);
    }

    [Fact]
    public void GetUserId_ShouldReturnNull_WhenPrincipalIsNull()
    {
        ClaimsPrincipal? principal = null;

        var result = principal.GetUserId();

        Assert.Null(result);
    }
}
