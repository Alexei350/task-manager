using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System.Globalization;
using System.Resources;
using TaskManager.Utils.i18n;

namespace TaskManager.UnitTests.Utils.i18n;

public class ResourceStringLocalizerTests
{
    [Fact]
    public void GetString_ShouldHandleMissingKey_AndSetDefaultCulture()
    {
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                {"DefaultCulture", "en-US"}
            })
            .Build();

        var httpContextAccessor = new HttpContextAccessor
        {
            HttpContext = new DefaultHttpContext()
        };

        var localizer = new ResourceStringLocalizer(config, httpContextAccessor);

        Assert.Throws<MissingManifestResourceException>(() => localizer.GetString("nonexistent_key"));
        Assert.Equal("en-US", CultureInfo.CurrentCulture.Name);
    }

    [Fact]
    public void SetCulture_ShouldRespectAcceptLanguageHeader()
    {
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                {"DefaultCulture", "pt-BR"}
            })
            .Build();

        var context = new DefaultHttpContext();
        context.Request.Headers.Append("Accept-Language", "es-ES");

        var httpContextAccessor = new HttpContextAccessor
        {
            HttpContext = context
        };

        var localizer = new ResourceStringLocalizer(config, httpContextAccessor);

        Assert.Throws<MissingManifestResourceException>(() => localizer.GetString("any"));

        Assert.Equal("es-ES", CultureInfo.CurrentCulture.Name);
    }
}
