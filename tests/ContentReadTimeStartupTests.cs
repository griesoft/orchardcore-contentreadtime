using Griesoft.OrchardCore.ContentReadTime;
using Griesoft.OrchardCore.ContentReadTime.GraphQL;
using Griesoft.OrchardCore.ContentReadTime.Models;
using Griesoft.OrchardCore.ContentReadTime.Services;
using GraphQL.Types;
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

    [Fact]
    public void ConfigureServices_RegistersGraphQLObjectType()
    {
        // Arrange
        var services = new ServiceCollection();
        var startup = new Startup();

        // Act
        startup.ConfigureServices(services);

        // Assert
        // The AddObjectGraphType extension registers the type as transient
        Assert.Contains(services, s =>
            s.ImplementationType == typeof(ContentReadTimeQueryObjectType) &&
            s.Lifetime == ServiceLifetime.Transient);
    }

    [Fact]
    public void ConfigureServices_RegistersCorrectGraphQLTypeForContentPart()
    {
        // Arrange
        var services = new ServiceCollection();
        var startup = new Startup();

        // Act
        startup.ConfigureServices(services);
        var serviceProvider = services.BuildServiceProvider();

        // Assert
        var graphqlType = serviceProvider.GetService<ContentReadTimeQueryObjectType>();
        Assert.NotNull(graphqlType);
        Assert.IsAssignableFrom<ObjectGraphType<ContentReadTimePart>>(graphqlType);
    }
}
