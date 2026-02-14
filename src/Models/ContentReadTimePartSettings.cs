namespace Griesoft.OrchardCore.ContentReadTime.Models;

/// <summary>
/// Settings for <see cref="ReadTimePart"/> that control which content source is used and reading speed.
/// </summary>
public class ContentReadTimePartSettings
{
    /// <summary>
    /// Gets or sets the name of the content part to read text from.
    /// </summary>
    public string ContentPartName { get; set; } = "HtmlBodyPart";

    /// <summary>
    /// Gets or sets the name of the content field within the part. Empty when reading directly from a body part.
    /// </summary>
    public string ContentFieldName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the average reading speed used to estimate read time.
    /// </summary>
    public int WordsPerMinute { get; set; } = 200;
}
