using Griesoft.OrchardCore.ContentReadTime.Migrations;
using Griesoft.OrchardCore.ContentReadTime.Models;
using Moq;
using OrchardCore.ContentManagement.Metadata;
using OrchardCore.ContentManagement.Metadata.Models;
using Xunit;

namespace Griesoft.OrchardCore.ContentReadTime.Tests;

public class ContentReadTimeMigrationsTests
{
    [Fact]
    public async Task CreateAsync_RegistersReadTimePart_ReturnsOne()
    {
        var definitionManagerMock = new Mock<IContentDefinitionManager>();

        // AlterPartDefinitionAsync is an extension method that calls GetPartDefinitionAsync
        // then AlterPartDefinitionAsync(ContentPartDefinition). Set up the underlying calls.
        definitionManagerMock
            .Setup(m => m.GetPartDefinitionAsync(nameof(ContentReadTimePart)))
            .ReturnsAsync(new ContentPartDefinition(nameof(ContentReadTimePart)));

        var migration = new ContentReadTimeMigrations(definitionManagerMock.Object);

        var result = await migration.CreateAsync();

        Assert.Equal(1, result);
    }
}
