using Xunit;

namespace GameOfLife.Tests;

public class GridTests
{
    [Theory]
    [InlineData("300", "50", "90")]
    [InlineData("50", "30", "20")]
    // [InlineData("3", "3", "1")]
    [InlineData("2000", "2000", "99")]
    public void Constructor_Succeeds_WithValidSettings
        (string width, string height, string populationRatio, string delay = "50")
    {
        var printer = new TestPrinter();
        var args = new string[] { width, height, populationRatio, delay };
        var settings = new Settings(args, printer);
        var grid = new Grid(settings, printer);

        Assert.NotNull(grid);
        Assert.Equal(ushort.Parse(width), grid.Width);
        Assert.Equal(ushort.Parse(height), grid.Height);
        Assert.Equal(GridState.Alive, grid.State);
    }
}