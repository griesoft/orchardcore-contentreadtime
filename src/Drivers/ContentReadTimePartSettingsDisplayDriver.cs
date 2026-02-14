using Griesoft.OrchardCore.ContentReadTime.Models;
using Griesoft.OrchardCore.ContentReadTime.ViewModels;
using Microsoft.AspNetCore.Mvc.Rendering;
using OrchardCore.ContentManagement.Metadata.Models;
using OrchardCore.ContentTypes.Editors;
using OrchardCore.DisplayManagement.Handlers;
using OrchardCore.DisplayManagement.Views;

namespace Griesoft.OrchardCore.ContentReadTime.Drivers;

/// <summary>
/// Display driver that provides the settings editor for <see cref="ContentReadTimePart"/>.
/// </summary>
public sealed class ContentReadTimePartSettingsDisplayDriver : ContentTypePartDefinitionDisplayDriver<ContentReadTimePart>
{
    private static readonly HashSet<string> TextBearingParts = ["HtmlBodyPart", "MarkdownBodyPart"];
    private static readonly HashSet<string> TextBearingFieldTypes = ["HtmlField", "TextField", "MarkdownField"];

    /// <inheritdoc />
    public override IDisplayResult Edit(ContentTypePartDefinition contentTypePartDefinition, BuildEditorContext context)
    {
        return Initialize<ContentReadTimePartSettingsViewModel>($"{nameof(ContentReadTimePartSettings)}_Edit", model =>
        {
            var settings = contentTypePartDefinition.GetSettings<ContentReadTimePartSettings>();
            model.WordsPerMinute = settings.WordsPerMinute;

            model.SelectedSource = string.IsNullOrEmpty(settings.ContentFieldName)
                ? $"{settings.ContentPartName}:"
                : $"{settings.ContentPartName}:{settings.ContentFieldName}";

            model.AvailableSources = BuildAvailableSources(contentTypePartDefinition.ContentTypeDefinition);
        }).Location("Content:5");
    }

    /// <inheritdoc />
    public override async Task<IDisplayResult> UpdateAsync(ContentTypePartDefinition contentTypePartDefinition, UpdateTypePartEditorContext context)
    {
        var model = new ContentReadTimePartSettingsViewModel();
        await context.Updater.TryUpdateModelAsync(model, Prefix);

        var parts = model.SelectedSource?.Split(':') ?? ["HtmlBodyPart", ""];
        var partName = parts[0];
        var fieldName = parts.Length > 1 ? parts[1] : string.Empty;

        context.Builder.WithSettings(new ContentReadTimePartSettings
        {
            ContentPartName = !string.IsNullOrEmpty(partName) ? partName : "HtmlBodyPart",
            ContentFieldName = fieldName,
            WordsPerMinute = model.WordsPerMinute > 0 ? model.WordsPerMinute : 200,
        });

        return Edit(contentTypePartDefinition, context);
    }

    private static List<SelectListItem> BuildAvailableSources(ContentTypeDefinition contentTypeDefinition)
    {
        var sources = new List<SelectListItem>();

        foreach (var typePart in contentTypeDefinition.Parts)
        {
            if (typePart.PartDefinition.Name == nameof(ContentReadTimePart))
            {
                continue;
            }

            // Known body parts (e.g., HtmlBodyPart, MarkdownBodyPart)
            if (TextBearingParts.Contains(typePart.PartDefinition.Name))
            {
                sources.Add(new SelectListItem(typePart.PartDefinition.Name, $"{typePart.Name}:"));
            }

            // Text-bearing fields on parts (e.g., HtmlField, TextField, MarkdownField)
            foreach (var field in typePart.PartDefinition.Fields)
            {
                if (TextBearingFieldTypes.Contains(field.FieldDefinition.Name))
                {
                    var display = $"{typePart.Name} ? {field.Name} ({field.FieldDefinition.Name})";
                    sources.Add(new SelectListItem(display, $"{typePart.Name}:{field.Name}"));
                }
            }
        }

        return sources;
    }
}
