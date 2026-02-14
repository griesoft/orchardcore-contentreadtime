using OrchardCore.ContentManagement;

namespace Griesoft.OrchardCore.ContentReadTime.Models;

/// <summary>
/// Content part that stores the estimated reading time for a content item.
/// </summary>
public class ContentReadTimePart : ContentPart
{
    /// <summary>
    /// Gets or sets the estimated reading time in minutes.
    /// </summary>
    public int Minutes { get; set; }
}
