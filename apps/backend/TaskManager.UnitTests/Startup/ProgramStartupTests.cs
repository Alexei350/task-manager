using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TaskManager.Context;
using TaskManager.Service.Interfaces;
using TaskManager.Utils.i18n;
using StartupClass = TaskManager.Startup;

namespace TaskManager.UnitTests.Startup;

public class ProgramStartupTests
{
    [Fact]
    public void CreateHostBuilder_ShouldConfigureStartup()
    {
        // Ensure required configuration keys exist so the host can build without throwing
        Environment.SetEnvironmentVariable("DB_USER", "user");
        Environment.SetEnvironmentVariable("DB_PASS", "pass");
        Environment.SetEnvironmentVariable("DB_HOST", "localhost");
        Environment.SetEnvironmentVariable("DB_NAME", "db");
        Environment.SetEnvironmentVariable("DB_PORT", "5432");
        Environment.SetEnvironmentVariable("Jwt:Issuer", "issuer");
        Environment.SetEnvironmentVariable("Jwt:Audience", "audience");
        Environment.SetEnvironmentVariable("Jwt:Key", "SuperSecretKeyForTests12345");
        Environment.SetEnvironmentVariable("DefaultCulture", "en-US");

        var builder = Program.CreateHostBuilder(Array.Empty<string>());

        Assert.NotNull(builder);
        using var host = builder.Build();
        Assert.NotNull(host.Services);
    }

    [Fact]
    public void ConfigureServices_ShouldRegisterCoreDependencies()
    {
        var settings = new Dictionary<string, string?>
        {
            {"DB_USER", "user"},
            {"DB_PASS", "pass"},
            {"DB_HOST", "localhost"},
            {"DB_NAME", "db"},
            {"DB_PORT", "5432"},
            {"Jwt:Issuer", "issuer"},
            {"Jwt:Audience", "audience"},
            {"Jwt:Key", "SuperSecretKeyForTests12345"},
            {"DefaultCulture", "en-US"}
        };

        IConfiguration configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(settings)
            .Build();

        var services = new ServiceCollection();
        services.AddSingleton(configuration);

        var startup = new StartupClass(configuration);

        startup.ConfigureServices(services);

        var provider = services.BuildServiceProvider();

        Assert.NotNull(provider.GetService<IAuthenticationService>());
        Assert.NotNull(provider.GetService<IUserService>());
        Assert.NotNull(provider.GetService<IMeService>());
        Assert.NotNull(provider.GetService<ITaskService>());
        Assert.NotNull(provider.GetService<IResourceStringLocalizer>());
        Assert.NotNull(provider.GetService<TaskManagerContext>());
    }
}
