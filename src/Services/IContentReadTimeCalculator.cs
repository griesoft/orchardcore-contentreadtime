namespace Griesoft.OrchardCore.ContentReadTime.Services;

/// <summary>
/// Calculates the estimated reading time for a given text.
/// </summary>
public interface IContentReadTimeCalculator
{
    /// <summary>
    /// Calculates the estimated reading time in minutes.
    /// </summary>
    /// <param name="text">The HTML or plain text content to evaluate.</param>
    /// <param name="wordsPerMinute">Average reading speed. Defaults to 200.</param>
    /// <returns>The estimated reading time in minutes, rounded up.</returns>
    int Calculate(string? text, int wordsPerMinute = 200);
}
