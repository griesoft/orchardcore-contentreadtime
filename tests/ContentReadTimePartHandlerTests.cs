using System.Text.Json;
using System.Text.Json.Nodes;
using Griesoft.OrchardCore.ContentReadTime.Handlers;
using Griesoft.OrchardCore.ContentReadTime.Models;
using Griesoft.OrchardCore.ContentReadTime.Services;
using Moq;
using OrchardCore.ContentManagement;
using OrchardCore.ContentManagement.Handlers;
using OrchardCore.ContentManagement.Metadata;
using OrchardCore.ContentManagement.Metadata.Models;
using Xunit;

namespace Griesoft.OrchardCore.ContentReadTime.Tests;

public class ContentReadTimePartHandlerTests
{
    private readonly Mock<IContentDefinitionManager> _definitionManagerMock = new();
    private readonly Mock<IContentReadTimeCalculator> _calculatorMock = new();
    private readonly ContentReadTimePartHandler _handler;

    public ContentReadTimePartHandlerTests()
    {
        _handler = new ContentReadTimePartHandler(
            _definitionManagerMock.Object,
            _calculatorMock.Object);
    }

    [Fact]
    public async Task PublishingAsync_NullTypeDefinition_ReturnsWithoutCalculating()
    {
        var contentItem = CreateContentItem("BlogPost");
        var part = CreatePart(contentItem);
        var context = new PublishContentContext(contentItem, contentItem);

        _definitionManagerMock
            .Setup(m => m.GetTypeDefinitionAsync("BlogPost"))
            .ReturnsAsync((ContentTypeDefinition?)null);

        await _handler.PublishingAsync(context, part);

        Assert.Equal(0, part.Minutes);
        _calculatorMock.Verify(c => c.Calculate(It.IsAny<string?>(), It.IsAny<int>()), Times.Never);
    }

    [Fact]
    public async Task PublishingAsync_NoReadTimePartInDefinition_ReturnsWithoutCalculating()
    {
        var contentItem = CreateContentItem("BlogPost");
        var part = CreatePart(contentItem);
        var context = new PublishContentContext(contentItem, contentItem);

        var otherPartDef = new ContentPartDefinition("SomePart");
        var otherTypePartDef = new ContentTypePartDefinition("SomePart", otherPartDef, new JsonObject());
        var typeDef = new ContentTypeDefinition("BlogPost", "Blog Post", [otherTypePartDef], new JsonObject());

        _definitionManagerMock
            .Setup(m => m.GetTypeDefinitionAsync("BlogPost"))
            .ReturnsAsync(typeDef);

        await _handler.PublishingAsync(context, part);

        Assert.Equal(0, part.Minutes);
        _calculatorMock.Verify(c => c.Calculate(It.IsAny<string?>(), It.IsAny<int>()), Times.Never);
    }

    [Fact]
    public async Task PublishingAsync_WithHtmlBodyPart_CalculatesMinutes()
    {
        var settings = new ContentReadTimePartSettings
        {
            ContentPartName = "HtmlBodyPart",
            ContentFieldName = "",
            WordsPerMinute = 200
        };

        var contentItem = CreateContentItem("BlogPost");
        SetContentData(contentItem, "HtmlBodyPart", new JsonObject { ["Html"] = "<p>Hello world</p>" });

        var part = CreatePart(contentItem);
        var context = new PublishContentContext(contentItem, contentItem);

        SetupTypeDefinition("BlogPost", settings);
        _calculatorMock
            .Setup(c => c.Calculate("<p>Hello world</p>", 200))
            .Returns(1);

        await _handler.PublishingAsync(context, part);

        Assert.Equal(1, part.Minutes);
    }

    [Fact]
    public async Task PublishingAsync_WithMarkdownContent_ExtractsMarkdownText()
    {
        var settings = new ContentReadTimePartSettings
        {
            ContentPartName = "MarkdownBodyPart",
            ContentFieldName = "",
            WordsPerMinute = 150
        };

        var contentItem = CreateContentItem("BlogPost");
        SetContentData(contentItem, "MarkdownBodyPart", new JsonObject { ["Markdown"] = "# Hello" });

        var part = CreatePart(contentItem);
        var context = new PublishContentContext(contentItem, contentItem);

        SetupTypeDefinition("BlogPost", settings);
        _calculatorMock
            .Setup(c => c.Calculate("# Hello", 150))
            .Returns(1);

        await _handler.PublishingAsync(context, part);

        Assert.Equal(1, part.Minutes);
    }

    [Fact]
    public async Task PublishingAsync_WithTextContent_ExtractsText()
    {
        var settings = new ContentReadTimePartSettings
        {
            ContentPartName = "TextPart",
            ContentFieldName = "",
            WordsPerMinute = 200
        };

        var contentItem = CreateContentItem("BlogPost");
        SetContentData(contentItem, "TextPart", new JsonObject { ["Text"] = "Plain text content" });

        var part = CreatePart(contentItem);
        var context = new PublishContentContext(contentItem, contentItem);

        SetupTypeDefinition("BlogPost", settings);
        _calculatorMock
            .Setup(c => c.Calculate("Plain text content", 200))
            .Returns(1);

        await _handler.PublishingAsync(context, part);

        Assert.Equal(1, part.Minutes);
    }

    [Fact]
    public async Task PublishingAsync_PartNodeMissing_CalculatesWithNull()
    {
        var settings = new ContentReadTimePartSettings
        {
            ContentPartName = "HtmlBodyPart",
            ContentFieldName = "",
            WordsPerMinute = 200
        };

        var contentItem = CreateContentItem("BlogPost");
        // Do not set HtmlBodyPart content

        var part = CreatePart(contentItem);
        var context = new PublishContentContext(contentItem, contentItem);

        SetupTypeDefinition("BlogPost", settings);
        _calculatorMock
            .Setup(c => c.Calculate(null, 200))
            .Returns(0);

        await _handler.PublishingAsync(context, part);

        Assert.Equal(0, part.Minutes);
    }

    [Fact]
    public async Task PublishingAsync_WithFieldSource_ExtractsFromField()
    {
        var settings = new ContentReadTimePartSettings
        {
            ContentPartName = "BlogPost",
            ContentFieldName = "MainContent",
            WordsPerMinute = 200
        };

        var fieldContent = new JsonObject
        {
            ["MainContent"] = new JsonObject { ["Html"] = "<p>Field text</p>" }
        };

        var contentItem = CreateContentItem("BlogPost");
        SetContentData(contentItem, "BlogPost", fieldContent);

        var part = CreatePart(contentItem);
        var context = new PublishContentContext(contentItem, contentItem);

        SetupTypeDefinition("BlogPost", settings);
        _calculatorMock
            .Setup(c => c.Calculate("<p>Field text</p>", 200))
            .Returns(1);

        await _handler.PublishingAsync(context, part);

        Assert.Equal(1, part.Minutes);
    }

    [Fact]
    public async Task PublishingAsync_WithFieldSource_FieldMissing_CalculatesWithNull()
    {
        var settings = new ContentReadTimePartSettings
        {
            ContentPartName = "BlogPost",
            ContentFieldName = "MissingField",
            WordsPerMinute = 200
        };

        var contentItem = CreateContentItem("BlogPost");
        SetContentData(contentItem, "BlogPost", new JsonObject());

        var part = CreatePart(contentItem);
        var context = new PublishContentContext(contentItem, contentItem);

        SetupTypeDefinition("BlogPost", settings);
        _calculatorMock
            .Setup(c => c.Calculate(null, 200))
            .Returns(0);

        await _handler.PublishingAsync(context, part);

        Assert.Equal(0, part.Minutes);
    }

    [Fact]
    public async Task PublishingAsync_NoMatchingTextProperty_CalculatesWithNull()
    {
        var settings = new ContentReadTimePartSettings
        {
            ContentPartName = "CustomPart",
            ContentFieldName = "",
            WordsPerMinute = 200
        };

        var contentItem = CreateContentItem("BlogPost");
        // Part node exists but has no Html, Markdown, or Text keys
        SetContentData(contentItem, "CustomPart", new JsonObject { ["Other"] = "value" });

        var part = CreatePart(contentItem);
        var context = new PublishContentContext(contentItem, contentItem);

        SetupTypeDefinition("BlogPost", settings);
        _calculatorMock
            .Setup(c => c.Calculate(null, 200))
            .Returns(0);

        await _handler.PublishingAsync(context, part);

        Assert.Equal(0, part.Minutes);
    }

    [Fact]
    public async Task PublishingAsync_FieldWithMarkdown_ExtractsMarkdownText()
    {
        var settings = new ContentReadTimePartSettings
        {
            ContentPartName = "BlogPost",
            ContentFieldName = "Body",
            WordsPerMinute = 200
        };

        var fieldContent = new JsonObject
        {
            ["Body"] = new JsonObject { ["Markdown"] = "## Heading" }
        };

        var contentItem = CreateContentItem("BlogPost");
        SetContentData(contentItem, "BlogPost", fieldContent);

        var part = CreatePart(contentItem);
        var context = new PublishContentContext(contentItem, contentItem);

        SetupTypeDefinition("BlogPost", settings);
        _calculatorMock
            .Setup(c => c.Calculate("## Heading", 200))
            .Returns(1);

        await _handler.PublishingAsync(context, part);

        Assert.Equal(1, part.Minutes);
    }

    [Fact]
    public async Task PublishingAsync_FieldWithText_ExtractsTextValue()
    {
        var settings = new ContentReadTimePartSettings
        {
            ContentPartName = "BlogPost",
            ContentFieldName = "Summary",
            WordsPerMinute = 200
        };

        var fieldContent = new JsonObject
        {
            ["Summary"] = new JsonObject { ["Text"] = "Some summary text" }
        };

        var contentItem = CreateContentItem("BlogPost");
        SetContentData(contentItem, "BlogPost", fieldContent);

        var part = CreatePart(contentItem);
        var context = new PublishContentContext(contentItem, contentItem);

        SetupTypeDefinition("BlogPost", settings);
        _calculatorMock
            .Setup(c => c.Calculate("Some summary text", 200))
            .Returns(1);

        await _handler.PublishingAsync(context, part);

        Assert.Equal(1, part.Minutes);
    }

    [Fact]
    public async Task PublishingAsync_FieldWithNoTextProperty_CalculatesWithNull()
    {
        var settings = new ContentReadTimePartSettings
        {
            ContentPartName = "BlogPost",
            ContentFieldName = "Image",
            WordsPerMinute = 200
        };

        var fieldContent = new JsonObject
        {
            ["Image"] = new JsonObject { ["Url"] = "https://example.com/img.png" }
        };

        var contentItem = CreateContentItem("BlogPost");
        SetContentData(contentItem, "BlogPost", fieldContent);

        var part = CreatePart(contentItem);
        var context = new PublishContentContext(contentItem, contentItem);

        SetupTypeDefinition("BlogPost", settings);
        _calculatorMock
            .Setup(c => c.Calculate(null, 200))
            .Returns(0);

        await _handler.PublishingAsync(context, part);

        Assert.Equal(0, part.Minutes);
    }

    private static ContentItem CreateContentItem(string contentType)
    {
        var item = new ContentItem();
        item.ContentType = contentType;
        return item;
    }

    private static ContentReadTimePart CreatePart(ContentItem contentItem)
    {
        var part = new ContentReadTimePart();
        part.ContentItem = contentItem;
        return part;
    }

    private static void SetContentData(ContentItem contentItem, string partName, JsonObject partData)
    {
        contentItem.Content[partName] = partData;
    }

    private void SetupTypeDefinition(string contentType, ContentReadTimePartSettings settings)
    {
        var settingsJson = new JsonObject
        {
            [nameof(ContentReadTimePartSettings)] = JsonSerializer.SerializeToNode(settings)
        };
        var readTimePartDef = new ContentPartDefinition(nameof(ContentReadTimePart));
        var readTimeTypePartDef = new ContentTypePartDefinition(
            nameof(ContentReadTimePart), readTimePartDef, settingsJson);
        var typeDef = new ContentTypeDefinition(
            contentType, contentType, [readTimeTypePartDef], new JsonObject());

        _definitionManagerMock
            .Setup(m => m.GetTypeDefinitionAsync(contentType))
            .ReturnsAsync(typeDef);
    }
}
