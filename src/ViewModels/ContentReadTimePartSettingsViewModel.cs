using Microsoft.AspNetCore.Mvc.Rendering;

namespace Griesoft.OrchardCore.ContentReadTime.ViewModels;

/// <summary>
/// View model for editing <see cref="Models.ReadTimePartSettings"/>.
/// </summary>
public class ContentReadTimePartSettingsViewModel
{
    /// <summary>
    /// Gets or sets the average reading speed used to estimate read time.
    /// </summary>
    public int WordsPerMinute { get; set; } = 200;

    /// <summary>
    /// Gets or sets the selected content source in <c>PartName:FieldName</c> format.
    /// </summary>
    public string SelectedSource { get; set; } = "HtmlBodyPart:";

    /// <summary>
    /// Gets or sets the list of content sources available for read-time calculation.
    /// </summary>
    public List<SelectListItem> AvailableSources { get; set; } = [];
}
