using Griesoft.OrchardCore.ContentReadTime.Drivers;
using Griesoft.OrchardCore.ContentReadTime.GraphQL;
using Griesoft.OrchardCore.ContentReadTime.Handlers;
using Griesoft.OrchardCore.ContentReadTime.Migrations;
using Griesoft.OrchardCore.ContentReadTime.Models;
using Griesoft.OrchardCore.ContentReadTime.Services;
using Microsoft.Extensions.DependencyInjection;
using OrchardCore.Apis;
using OrchardCore.ContentManagement;
using OrchardCore.ContentTypes.Editors;
using OrchardCore.Data.Migration;
using OrchardCore.Modules;

namespace Griesoft.OrchardCore.ContentReadTime;

/// <summary>
/// Registers services and content parts for the Content Read Time module.
/// </summary>
public sealed class Startup : StartupBase
{
    /// <inheritdoc />
    public override void ConfigureServices(IServiceCollection services)
    {
        services.AddContentPart<ContentReadTimePart>()
            .AddHandler<ContentReadTimePartHandler>();

        services.AddScoped<IContentTypePartDefinitionDisplayDriver, ContentReadTimePartSettingsDisplayDriver>();
        services.AddDataMigration<ContentReadTimeMigrations>();
        services.AddScoped<IContentReadTimeCalculator, ContentReadTimeCalculator>();
    }
}

/// <summary>
/// Registers GraphQL types for the Content Read Time module.
/// </summary>
[RequireFeatures("OrchardCore.Apis.GraphQL")]
public sealed class GraphQLStartup : StartupBase
{
    /// <inheritdoc />
    public override void ConfigureServices(IServiceCollection services)
    {
        services.AddObjectGraphType<ContentReadTimePart, ContentReadTimePartQueryObjectType>();
    }
}
