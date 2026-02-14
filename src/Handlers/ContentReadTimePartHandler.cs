using System.Text.Json.Nodes;
using Griesoft.OrchardCore.ContentReadTime.Models;
using Griesoft.OrchardCore.ContentReadTime.Services;
using OrchardCore.ContentManagement;
using OrchardCore.ContentManagement.Handlers;
using OrchardCore.ContentManagement.Metadata;

namespace Griesoft.OrchardCore.ContentReadTime.Handlers;

/// <summary>
/// Handles publishing events for <see cref="ReadTimePart"/> to calculate and persist the estimated reading time.
/// </summary>
public sealed class ContentReadTimePartHandler : ContentPartHandler<ContentReadTimePart>
{
    private readonly IContentDefinitionManager _contentDefinitionManager;
    private readonly IContentReadTimeCalculator _readTimeCalculator;

    /// <summary>
    /// Initializes a new instance of the <see cref="ContentReadTimePartHandler"/> class.
    /// </summary>
    public ContentReadTimePartHandler(
        IContentDefinitionManager contentDefinitionManager,
        IContentReadTimeCalculator readTimeCalculator)
    {
        _contentDefinitionManager = contentDefinitionManager;
        _readTimeCalculator = readTimeCalculator;
    }

    /// <inheritdoc />
    public override async Task PublishingAsync(PublishContentContext context, ContentReadTimePart part)
    {
        var contentTypeDefinition = await _contentDefinitionManager.GetTypeDefinitionAsync(context.ContentItem.ContentType);
        var contentTypePartDefinition = contentTypeDefinition?.Parts
            .FirstOrDefault(p => p.PartDefinition.Name == nameof(ContentReadTimePart));

        if (contentTypePartDefinition is null)
        {
            return;
        }

        var settings = contentTypePartDefinition.GetSettings<ContentReadTimePartSettings>();
        var text = ExtractText(context.ContentItem, settings);

        part.Minutes = _readTimeCalculator.Calculate(text, settings.WordsPerMinute);
        part.Apply();
    }

    private static string? ExtractText(ContentItem contentItem, ContentReadTimePartSettings settings)
    {
        var partNode = contentItem.Content[settings.ContentPartName];
        if (partNode is null)
        {
            return null;
        }

        if (string.IsNullOrEmpty(settings.ContentFieldName))
        {
            // Reading from a body part directly (e.g., HtmlBodyPart.Html, MarkdownBodyPart.Markdown)
            return TryGetTextValue(partNode);
        }

        // Reading from a field on a part (e.g., BlogPost.MainContent)
        var fieldNode = partNode[settings.ContentFieldName];
        return fieldNode is null ? null : TryGetTextValue(fieldNode);
    }

    private static string? TryGetTextValue(JsonNode node)
    {
        return node["Html"]?.GetValue<string>()
            ?? node["Markdown"]?.GetValue<string>()
            ?? node["Text"]?.GetValue<string>();
    }
}
