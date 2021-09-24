using Xunit;
using GameOfLife;

namespace GameOfLife.Tests;

public class GridTests
{
    [Theory]
    [InlineData("300", "50", "90")]
    [InlineData("50", "30", "20")]
    [InlineData("3", "3", "1")]
    [InlineData("20000", "20000", "99")]
    public void Constructor_Succeeds_WithValidSettings
        (string width, string height, string populationRatio, string delay = "50")
    {
        var args = new string[] { width, height, populationRatio, delay };
        var settings = new Settings(args);

        Assert.NotNull(settings);
        Assert.Equal(ushort.Parse(width), settings.Width);
        Assert.Equal(ushort.Parse(height), settings.Height);
        Assert.Equal(byte.Parse(populationRatio), settings.InitialPopulationRatio);
    }
}