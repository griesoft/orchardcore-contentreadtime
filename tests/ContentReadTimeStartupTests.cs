using Griesoft.OrchardCore.ContentReadTime;
using Griesoft.OrchardCore.ContentReadTime.Services;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Griesoft.OrchardCore.ContentReadTime.Tests;

public class StartupTests
{
    [Fact]
    public void ConfigureServices_RegistersReadTimeCalculator()
    {
        var services = new ServiceCollection();
        var startup = new Startup();

        startup.ConfigureServices(services);

        Assert.Contains(services, s =>
            s.ServiceType == typeof(IContentReadTimeCalculator) &&
            s.ImplementationType == typeof(ContentReadTimeCalculator) &&
            s.Lifetime == ServiceLifetime.Scoped);
    }
}
