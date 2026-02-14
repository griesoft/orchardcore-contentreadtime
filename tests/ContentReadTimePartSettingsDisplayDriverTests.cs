using System.Reflection;
using System.Text.Json;
using System.Text.Json.Nodes;
using Griesoft.OrchardCore.ContentReadTime.Drivers;
using Griesoft.OrchardCore.ContentReadTime.Models;
using Griesoft.OrchardCore.ContentReadTime.ViewModels;
using Microsoft.AspNetCore.Mvc.Rendering;
using Moq;
using OrchardCore.ContentManagement.Metadata.Builders;
using OrchardCore.ContentManagement.Metadata.Models;
using OrchardCore.DisplayManagement;
using OrchardCore.DisplayManagement.ModelBinding;
using OrchardCore.DisplayManagement.Zones;
using Xunit;

namespace Griesoft.OrchardCore.ContentReadTime.Tests;

public class ContentReadTimePartSettingsDisplayDriverTests
{
    [Fact]
    public void Edit_WithDefaultSettings_ReturnsNonNullResult()
    {
        var driver = new ContentReadTimePartSettingsDisplayDriver();
        var typePartDef = CreateTypePartDefinition(new ContentReadTimePartSettings());

        var result = driver.Edit(typePartDef, null!);

        Assert.NotNull(result);
    }

    [Fact]
    public void Edit_WithFieldSource_ReturnsNonNullResult()
    {
        var driver = new ContentReadTimePartSettingsDisplayDriver();
        var settings = new ContentReadTimePartSettings
        {
            ContentPartName = "BlogPost",
            ContentFieldName = "MainContent",
            WordsPerMinute = 300
        };
        var typePartDef = CreateTypePartDefinition(settings);

        var result = driver.Edit(typePartDef, null!);

        Assert.NotNull(result);
    }

    [Fact]
    public void Edit_Initializer_WithBodyPartSource_SetsSelectedSourceWithoutField()
    {
        var driver = new ContentReadTimePartSettingsDisplayDriver();
        var settings = new ContentReadTimePartSettings
        {
            ContentPartName = "HtmlBodyPart",
            ContentFieldName = "",
            WordsPerMinute = 250
        };
        var typePartDef = CreateTypePartDefinition(settings);

        var result = driver.Edit(typePartDef, null!);
        var model = new ContentReadTimePartSettingsViewModel();
        InvokeShapeInitializer(result, model);

        Assert.Equal(250, model.WordsPerMinute);
        Assert.Equal("HtmlBodyPart:", model.SelectedSource);
        Assert.NotNull(model.AvailableSources);
    }

    [Fact]
    public void Edit_Initializer_WithFieldSource_SetsSelectedSourceWithField()
    {
        var driver = new ContentReadTimePartSettingsDisplayDriver();
        var settings = new ContentReadTimePartSettings
        {
            ContentPartName = "BlogPost",
            ContentFieldName = "MainContent",
            WordsPerMinute = 300
        };
        var typePartDef = CreateTypePartDefinition(settings);

        var result = driver.Edit(typePartDef, null!);
        var model = new ContentReadTimePartSettingsViewModel();
        InvokeShapeInitializer(result, model);

        Assert.Equal(300, model.WordsPerMinute);
        Assert.Equal("BlogPost:MainContent", model.SelectedSource);
    }

    [Fact]
    public async Task UpdateAsync_DefaultModel_AppliesSettings()
    {
        var driver = new ContentReadTimePartSettingsDisplayDriver();
        var typePartDef = CreateTypePartDefinition(new ContentReadTimePartSettings());
        var context = CreateUpdateContext(typePartDef);

        var result = await driver.UpdateAsync(typePartDef, context);

        Assert.NotNull(result);
    }

    [Fact]
    public async Task UpdateAsync_NullSelectedSource_DefaultsPartName()
    {
        var driver = new ContentReadTimePartSettingsDisplayDriver();
        var typePartDef = CreateTypePartDefinition(new ContentReadTimePartSettings());
        var context = CreateUpdateContext(typePartDef, configure: m =>
        {
            m.SelectedSource = null!;
            m.WordsPerMinute = 0;
        });

        var result = await driver.UpdateAsync(typePartDef, context);

        Assert.NotNull(result);
    }

    [Fact]
    public async Task UpdateAsync_EmptyPartName_DefaultsToHtmlBodyPart()
    {
        var driver = new ContentReadTimePartSettingsDisplayDriver();
        var typePartDef = CreateTypePartDefinition(new ContentReadTimePartSettings());
        var context = CreateUpdateContext(typePartDef, configure: m =>
        {
            m.SelectedSource = ":SomeField";
            m.WordsPerMinute = 100;
        });

        var result = await driver.UpdateAsync(typePartDef, context);

        Assert.NotNull(result);
    }

    [Fact]
    public async Task UpdateAsync_SourceWithoutColon_FieldNameIsEmpty()
    {
        var driver = new ContentReadTimePartSettingsDisplayDriver();
        var typePartDef = CreateTypePartDefinition(new ContentReadTimePartSettings());
        var context = CreateUpdateContext(typePartDef, configure: m =>
        {
            m.SelectedSource = "HtmlBodyPart";
            m.WordsPerMinute = 200;
        });

        var result = await driver.UpdateAsync(typePartDef, context);

        Assert.NotNull(result);
    }

    [Fact]
    public async Task UpdateAsync_NegativeWordsPerMinute_DefaultsTo200()
    {
        var driver = new ContentReadTimePartSettingsDisplayDriver();
        var typePartDef = CreateTypePartDefinition(new ContentReadTimePartSettings());
        var context = CreateUpdateContext(typePartDef, configure: m =>
        {
            m.SelectedSource = "HtmlBodyPart:";
            m.WordsPerMinute = -5;
        });

        var result = await driver.UpdateAsync(typePartDef, context);

        Assert.NotNull(result);
    }

    [Fact]
    public void BuildAvailableSources_SkipsReadTimePart()
    {
        var readTimePartDef = new ContentPartDefinition(nameof(ContentReadTimePart));
        var readTimeTypePartDef = new ContentTypePartDefinition(
            nameof(ContentReadTimePart), readTimePartDef, new JsonObject());

        var typeDef = new ContentTypeDefinition(
            "BlogPost", "Blog Post", [readTimeTypePartDef], new JsonObject());

        var sources = InvokeBuildAvailableSources(typeDef);

        Assert.Empty(sources);
    }

    [Fact]
    public void BuildAvailableSources_AddsHtmlBodyPart()
    {
        var htmlBodyPartDef = new ContentPartDefinition("HtmlBodyPart");
        var htmlBodyTypePartDef = new ContentTypePartDefinition(
            "HtmlBodyPart", htmlBodyPartDef, new JsonObject());

        var typeDef = new ContentTypeDefinition(
            "BlogPost", "Blog Post", [htmlBodyTypePartDef], new JsonObject());

        var sources = InvokeBuildAvailableSources(typeDef);

        Assert.Single(sources);
        Assert.Equal("HtmlBodyPart:", sources[0].Value);
        Assert.Equal("HtmlBodyPart", sources[0].Text);
    }

    [Fact]
    public void BuildAvailableSources_AddsMarkdownBodyPart()
    {
        var markdownPartDef = new ContentPartDefinition("MarkdownBodyPart");
        var markdownTypePartDef = new ContentTypePartDefinition(
            "MarkdownBodyPart", markdownPartDef, new JsonObject());

        var typeDef = new ContentTypeDefinition(
            "BlogPost", "Blog Post", [markdownTypePartDef], new JsonObject());

        var sources = InvokeBuildAvailableSources(typeDef);

        Assert.Single(sources);
        Assert.Equal("MarkdownBodyPart:", sources[0].Value);
    }

    [Fact]
    public void BuildAvailableSources_AddsTextBearingFields()
    {
        var htmlFieldDef = new ContentFieldDefinition("HtmlField");
        var htmlFieldPartField = new ContentPartFieldDefinition(htmlFieldDef, "Body", new JsonObject());

        var textFieldDef = new ContentFieldDefinition("TextField");
        var textFieldPartField = new ContentPartFieldDefinition(textFieldDef, "Summary", new JsonObject());

        var markdownFieldDef = new ContentFieldDefinition("MarkdownField");
        var markdownFieldPartField = new ContentPartFieldDefinition(markdownFieldDef, "Notes", new JsonObject());

        var partDef = new ContentPartDefinition("CustomPart",
            [htmlFieldPartField, textFieldPartField, markdownFieldPartField], new JsonObject());
        var typePartDef = new ContentTypePartDefinition("CustomPart", partDef, new JsonObject());

        var typeDef = new ContentTypeDefinition(
            "BlogPost", "Blog Post", [typePartDef], new JsonObject());

        var sources = InvokeBuildAvailableSources(typeDef);

        Assert.Equal(3, sources.Count);
        Assert.Equal("CustomPart:Body", sources[0].Value);
        Assert.Equal("CustomPart:Summary", sources[1].Value);
        Assert.Equal("CustomPart:Notes", sources[2].Value);
    }

    [Fact]
    public void BuildAvailableSources_SkipsNonTextBearingFields()
    {
        var imageFieldDef = new ContentFieldDefinition("MediaField");
        var imageFieldPartField = new ContentPartFieldDefinition(imageFieldDef, "Image", new JsonObject());

        var partDef = new ContentPartDefinition("CustomPart",
            [imageFieldPartField], new JsonObject());
        var typePartDef = new ContentTypePartDefinition("CustomPart", partDef, new JsonObject());

        var typeDef = new ContentTypeDefinition(
            "BlogPost", "Blog Post", [typePartDef], new JsonObject());

        var sources = InvokeBuildAvailableSources(typeDef);

        Assert.Empty(sources);
    }

    [Fact]
    public void BuildAvailableSources_CombinesBodyPartsAndFields()
    {
        var htmlBodyPartDef = new ContentPartDefinition("HtmlBodyPart");
        var htmlBodyTypePartDef = new ContentTypePartDefinition(
            "HtmlBodyPart", htmlBodyPartDef, new JsonObject());

        var htmlFieldDef = new ContentFieldDefinition("HtmlField");
        var htmlFieldPartField = new ContentPartFieldDefinition(htmlFieldDef, "Body", new JsonObject());
        var customPartDef = new ContentPartDefinition("CustomPart",
            [htmlFieldPartField], new JsonObject());
        var customTypePartDef = new ContentTypePartDefinition("CustomPart", customPartDef, new JsonObject());

        var readTimePartDef = new ContentPartDefinition(nameof(ContentReadTimePart));
        var readTimeTypePartDef = new ContentTypePartDefinition(
            nameof(ContentReadTimePart), readTimePartDef, new JsonObject());

        var typeDef = new ContentTypeDefinition(
            "BlogPost", "Blog Post",
            [htmlBodyTypePartDef, customTypePartDef, readTimeTypePartDef],
            new JsonObject());

        var sources = InvokeBuildAvailableSources(typeDef);

        Assert.Equal(2, sources.Count);
        Assert.Equal("HtmlBodyPart:", sources[0].Value);
        Assert.Equal("CustomPart:Body", sources[1].Value);
    }

    [Fact]
    public void BuildAvailableSources_TextBearingFieldDisplayFormat()
    {
        var htmlFieldDef = new ContentFieldDefinition("HtmlField");
        var htmlFieldPartField = new ContentPartFieldDefinition(htmlFieldDef, "Body", new JsonObject());

        var partDef = new ContentPartDefinition("BlogPost",
            [htmlFieldPartField], new JsonObject());
        var typePartDef = new ContentTypePartDefinition("BlogPost", partDef, new JsonObject());

        var typeDef = new ContentTypeDefinition(
            "BlogPost", "Blog Post", [typePartDef], new JsonObject());

        var sources = InvokeBuildAvailableSources(typeDef);

        Assert.Single(sources);
        Assert.Equal("BlogPost ? Body (HtmlField)", sources[0].Text);
        Assert.Equal("BlogPost:Body", sources[0].Value);
    }

    private static ContentTypePartDefinition CreateTypePartDefinition(ContentReadTimePartSettings settings)
    {
        var settingsJson = new JsonObject
        {
            [nameof(ContentReadTimePartSettings)] = JsonSerializer.SerializeToNode(settings)
        };
        var partDef = new ContentPartDefinition(nameof(ContentReadTimePart));
        var typePartDef = new ContentTypePartDefinition(
            nameof(ContentReadTimePart), partDef, settingsJson);

        _ = new ContentTypeDefinition("BlogPost", "Blog Post", [typePartDef], new JsonObject());

        return typePartDef;
    }

    private static global::OrchardCore.ContentTypes.Editors.UpdateTypePartEditorContext CreateUpdateContext(
        ContentTypePartDefinition typePartDef,
        Action<ContentReadTimePartSettingsViewModel>? configure = null)
    {
        var updaterMock = new Mock<IUpdateModel>();
        updaterMock
            .Setup(u => u.TryUpdateModelAsync(
                It.IsAny<ContentReadTimePartSettingsViewModel>(),
                It.IsAny<string>()))
            .Returns((ContentReadTimePartSettingsViewModel m, string _) =>
            {
                configure?.Invoke(m);
                return Task.FromResult(true);
            });

        var builderMock = new Mock<ContentTypePartDefinitionBuilder>(typePartDef);
        var shapeMock = new Mock<IShape>();
        var shapeFactoryMock = new Mock<IShapeFactory>();
        var layoutMock = new Mock<IZoneHolding>();

        return new global::OrchardCore.ContentTypes.Editors.UpdateTypePartEditorContext(
            builderMock.Object,
            shapeMock.Object,
            "",
            false,
            shapeFactoryMock.Object,
            layoutMock.Object,
            updaterMock.Object);
    }

    private static List<SelectListItem> InvokeBuildAvailableSources(ContentTypeDefinition contentTypeDefinition)
    {
        var method = typeof(ContentReadTimePartSettingsDisplayDriver)
            .GetMethod("BuildAvailableSources", BindingFlags.NonPublic | BindingFlags.Static);
        return (List<SelectListItem>)method!.Invoke(null, [contentTypeDefinition])!;
    }

    /// <summary>
    /// Walks the object graph of an IDisplayResult to find and invoke a stored
    /// Action<ContentReadTimePartSettingsViewModel> initializer set by Initialize<T>.
    /// </summary>
    private static void InvokeShapeInitializer(object result, ContentReadTimePartSettingsViewModel model)
    {
        var action = FindDelegate(result, []);
        Assert.NotNull(action);
        action!(model);
    }

    private static Action<ContentReadTimePartSettingsViewModel>? FindDelegate(object? obj, HashSet<object> visited)
    {
        if (obj is null || !visited.Add(obj))
            return null;

        var type = obj.GetType();
        if (type.IsPrimitive || obj is string || obj is Type)
            return null;

        var fields = type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        foreach (var field in fields)
        {
            object? value;
            try { value = field.GetValue(obj); }
            catch { continue; }

            if (value is Action<ContentReadTimePartSettingsViewModel> action)
                return action;

            var found = FindDelegate(value, visited);
            if (found is not null)
                return found;
        }

        return null;
    }
}

