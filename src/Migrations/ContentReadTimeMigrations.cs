using Griesoft.OrchardCore.ContentReadTime.Models;
using OrchardCore.ContentManagement.Metadata;
using OrchardCore.ContentManagement.Metadata.Settings;
using OrchardCore.Data.Migration;

namespace Griesoft.OrchardCore.ContentReadTime.Migrations;

/// <summary>
/// Data migration that registers the <see cref="ReadTimePart"/> content part definition.
/// </summary>
public sealed class ContentReadTimeMigrations : DataMigration
{
    private readonly IContentDefinitionManager _contentDefinitionManager;

    /// <summary>
    /// Initializes a new instance of the <see cref="ContentReadTimeMigrations"/> class.
    /// </summary>
    public ContentReadTimeMigrations(IContentDefinitionManager contentDefinitionManager)
    {
        _contentDefinitionManager = contentDefinitionManager;
    }

    /// <summary>
    /// Creates the <see cref="ContentReadTimePart"/> part definition as an attachable part.
    /// </summary>
    public async Task<int> CreateAsync()
    {
        await _contentDefinitionManager.AlterPartDefinitionAsync(nameof(ContentReadTimePart), part => part
            .Attachable()
            .WithDescription("Calculates and stores the estimated reading time for a content item."));

        return 1;
    }
}
