using Griesoft.OrchardCore.ContentReadTime.Services;
using Xunit;

namespace Griesoft.OrchardCore.ContentReadTime.Tests;

public class ContentReadTimeCalculatorTests
{
    private readonly ContentReadTimeCalculator _calculator = new();

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("\t\n")]
    public void Calculate_NullOrWhitespace_ReturnsZero(string? text)
    {
        Assert.Equal(0, _calculator.Calculate(text));
    }

    [Fact]
    public void Calculate_ExactWordsPerMinute_ReturnsOne()
    {
        var text = string.Join(" ", Enumerable.Repeat("word", 200));
        Assert.Equal(1, _calculator.Calculate(text, 200));
    }

    [Fact]
    public void Calculate_RoundsUp()
    {
        var text = string.Join(" ", Enumerable.Repeat("word", 201));
        Assert.Equal(2, _calculator.Calculate(text, 200));
    }

    [Fact]
    public void Calculate_SingleWord_ReturnsOne()
    {
        Assert.Equal(1, _calculator.Calculate("hello"));
    }

    [Fact]
    public void Calculate_HtmlTags_AreStripped()
    {
        var html = "<p>one <b>two</b> three</p>";
        Assert.Equal(1, _calculator.Calculate(html, 200));
    }

    [Fact]
    public void Calculate_HtmlEntities_AreDecoded()
    {
        // "&amp;" decodes to "&", which counts as a word
        var text = "one &amp; two &lt; three";
        Assert.Equal(1, _calculator.Calculate(text, 200));
    }

    [Fact]
    public void Calculate_CustomWordsPerMinute()
    {
        var text = string.Join(" ", Enumerable.Repeat("word", 100));
        Assert.Equal(1, _calculator.Calculate(text, 100));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-100)]
    public void Calculate_InvalidWordsPerMinute_DefaultsTo200(int wpm)
    {
        var text = string.Join(" ", Enumerable.Repeat("word", 200));
        Assert.Equal(1, _calculator.Calculate(text, wpm));
    }

    [Fact]
    public void Calculate_MultipleMinutes()
    {
        var text = string.Join(" ", Enumerable.Repeat("word", 600));
        Assert.Equal(3, _calculator.Calculate(text, 200));
    }

    [Fact]
    public void Calculate_ComplexHtml_StripsAllTags()
    {
        var html = "<div class=\"content\"><h1>Title</h1><p>Body <a href=\"#\">link</a></p></div>";
        // Words: Title, Body, link = 3
        Assert.Equal(1, _calculator.Calculate(html, 200));
    }

    [Fact]
    public void Calculate_DefaultWordsPerMinute_Is200()
    {
        var text = string.Join(" ", Enumerable.Repeat("word", 200));
        Assert.Equal(1, _calculator.Calculate(text));
    }
}
