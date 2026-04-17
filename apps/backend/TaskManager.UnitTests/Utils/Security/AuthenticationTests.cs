using System.Security.Claims;
using FakeItEasy;
using Microsoft.Extensions.Configuration;
using TaskManager.Models.Entities;
using TaskManager.Utils.i18n;
using TaskManager.Utils.Security;

namespace TaskManager.UnitTests.Utils.Security;

public class AuthenticationTests
{
    private readonly Authentication _authentication;
    private readonly IResourceStringLocalizer _localizer;
    private readonly IConfiguration _config;

    public AuthenticationTests()
    {
        _localizer = A.Fake<IResourceStringLocalizer>();
        _config = A.Fake<IConfiguration>();

        A.CallTo(() => _config["Jwt:Issuer"]).Returns("TestIssuer");
        A.CallTo(() => _config["Jwt:Audience"]).Returns("TestAudience");
        A.CallTo(() => _config["Jwt:Key"]).Returns("ThisIsASecretKeyForTestingPurposesOnly12345");

        _authentication = new Authentication(_localizer, _config);
    }

    [Fact]
    public void GenerateRefreshToken_ShouldReturnString()
    {
        var token = Authentication.GenerateRefreshToken();
        Assert.NotNull(token);
        Assert.NotEmpty(token);
    }

    [Fact]
    public void GenerateToken_ShouldReturnTokenReturn()
    {
        var claims = new List<Claim> { new Claim(ClaimTypes.Name, "test") };
        var refreshToken = "refreshToken";
        var user = new User { Id = Guid.NewGuid(), Name = "Test User", Email = "test@test.com" };

        var result = _authentication.GenerateToken(claims, refreshToken, user);

        Assert.True(result.Success);
        Assert.NotNull(result.Data);
        Assert.Equal(refreshToken, result.Data.RefreshToken);
        Assert.NotNull(result.Data.Token);
    }

    [Fact]
    public void GetPrincipalFromExpiredToken_ShouldReturnPrincipal_WhenTokenIsValid()
    {
        // This is hard to test without generating a valid token first.
        // We can use GenerateToken to get a token and then validate it.
        
        var claims = new List<Claim> { new Claim(ClaimTypes.Name, "test") };
        var refreshToken = "refreshToken";
        var user = new User { Id = Guid.NewGuid(), Name = "Test User", Email = "test@test.com" };

        var tokenResult = _authentication.GenerateToken(claims, refreshToken, user);
        var token = tokenResult.Data.Token;

        var result = _authentication.GetPrincipalFromExpiredToken(token);

        Assert.True(result.Success);
        Assert.NotNull(result.Data);
    }

    [Fact]
    public void GetPrincipalFromExpiredToken_ShouldReturnError_WhenTokenIsInvalid()
    {
        var token = "invalidToken";
        A.CallTo(() => _localizer.GetString(LocalizationDictionary.TokenValidationError)).Returns("Error");

        var result = _authentication.GetPrincipalFromExpiredToken(token);

        Assert.False(result.Success);
    }
}
