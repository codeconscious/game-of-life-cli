using Xunit;

namespace GameOfLife.Tests;

public class GridTests
{
    /*
    [Theory]
    [InlineData("0", "300", "50", "90")]
    [InlineData("0", "50", "30", "20")]
    // [InlineData("0", "3", "3", "1")]
    [InlineData("0", "1000", "1000", "99")]
    public void Constructor_Succeeds_WithValidSettings
        (string useHiRes, string width, string height, string populationRatio, string delay = "50")
    {
        var printer = new TestPrinter();
        var args = new string[] { useHiRes, width, height, populationRatio, delay };
        var settings = new Settings(args, printer);
        var grid = new Grid(settings, printer);

        Assert.NotNull(grid);
        Assert.Equal(ushort.Parse(width), grid.Width);
        Assert.Equal(ushort.Parse(height), grid.Height);
        Assert.Equal(GridState.Alive, grid.State);
    }
    */
}