using System.Text.RegularExpressions;

namespace Griesoft.OrchardCore.ContentReadTime.Services;

/// <inheritdoc />
public sealed partial class ContentReadTimeCalculator : IContentReadTimeCalculator
{
    /// <inheritdoc />
    public int Calculate(string? text, int wordsPerMinute = 200)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            return 0;
        }

        if (wordsPerMinute <= 0)
        {
            wordsPerMinute = 200;
        }

        // Strip HTML tags
        var plainText = HtmlTagRegex().Replace(text, " ");

        // Decode HTML entities
        plainText = System.Net.WebUtility.HtmlDecode(plainText);

        // Count words by splitting on whitespace
        var wordCount = plainText.Split((char[]?)null, StringSplitOptions.RemoveEmptyEntries).Length;

        return (int)Math.Ceiling((double)wordCount / wordsPerMinute);
    }

    [GeneratedRegex(@"<[^>]+>")]
    private static partial Regex HtmlTagRegex();
}
