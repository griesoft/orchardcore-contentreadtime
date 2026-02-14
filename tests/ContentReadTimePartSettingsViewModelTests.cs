using Griesoft.OrchardCore.ContentReadTime.ViewModels;
using Xunit;

namespace Griesoft.OrchardCore.ContentReadTime.Tests;

public class ContentReadTimePartSettingsViewModelTests
{
    [Fact]
    public void Constructor_SetsDefaultValues()
    {
        var viewModel = new ContentReadTimePartSettingsViewModel();

        Assert.Equal(200, viewModel.WordsPerMinute);
        Assert.Equal("HtmlBodyPart:", viewModel.SelectedSource);
        Assert.NotNull(viewModel.AvailableSources);
        Assert.Empty(viewModel.AvailableSources);
    }

    [Fact]
    public void Properties_CanBeSet()
    {
        var viewModel = new ContentReadTimePartSettingsViewModel
        {
            WordsPerMinute = 300,
            SelectedSource = "BlogPost:MainContent",
            AvailableSources = [new("Test", "test:")]
        };

        Assert.Equal(300, viewModel.WordsPerMinute);
        Assert.Equal("BlogPost:MainContent", viewModel.SelectedSource);
        Assert.Single(viewModel.AvailableSources);
    }
}
