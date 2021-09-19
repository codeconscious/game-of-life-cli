using Xunit;
using GameOfLife;

namespace GameOfLife.Tests;

public class SettingsTests
{
    [Theory]
    [InlineData("300", "50", "90")]
    [InlineData("50", "30", "20")]
    [InlineData("3", "3", "1")]
    [InlineData("20000", "20000", "99")]
    public void Constructor_Succeeds_With3ProperArguments(string width, string height, string populationRatio)
    {
        var args = new string[] { width, height, populationRatio };
        var settings = new Settings(args);

        Assert.NotNull(settings);
        Assert.Equal(ushort.Parse(width), settings.Width);
        Assert.Equal(ushort.Parse(height), settings.Height);
        Assert.Equal(byte.Parse(populationRatio), settings.InitialPopulationRatio);
    }

    [Theory]
    [InlineData("300", "50", "90", "500")]
    [InlineData("50", "30", "20", "10000")]
    [InlineData("3", "3", "1", "0")]
    [InlineData("20000", "20000", "99", "20000")]
    public void Constructor_Succeeds_With4ProperArguments(string width, string height, string populationRatio, string delay)
    {
        var args = new string[] { width, height, populationRatio, delay };
        var settings = new Settings(args);

        Assert.NotNull(settings);
        Assert.Equal(ushort.Parse(width), settings.Width);
        Assert.Equal(ushort.Parse(height), settings.Height);
        Assert.Equal(byte.Parse(populationRatio), settings.InitialPopulationRatio);
        Assert.Equal(ushort.Parse(delay), settings.InitialIterationDelayMs);
    }

    [Theory]
    [InlineData("-2", "50", "90")]
    [InlineData("50", "-0", "20")]
    [InlineData("5", "5", "-5")]
    [InlineData("5", "5", "0")]
    [InlineData("5", "5", "50.5")]
    [InlineData("5", "5", "101")]
    [InlineData("5", "5", "200")]
    public void Constructor_ThrowsArgumentOutOfRangeException_WithImproperArgumentInTrio(string width, string height, string populationRatio)
    {
        var args = new string[] { width, height, populationRatio };
        Assert.Throws<ArgumentOutOfRangeException>(() => new GameOfLife.Settings(args));
    }

    [Theory]
    [InlineData("-2", "50", "90", "5000")]
    [InlineData("50", "-0", "20", "10000")]
    [InlineData("5", "5", "200", "-10")]
    [InlineData("5", "5", "200", "200000000")]
    public void Constructor_ThrowsArgumentOutOfRangeException_WithImproperArgumentInQuartet(string width, string height, string populationRatio, string delay)
    {
        var args = new string[] { width, height, populationRatio, delay };
        Assert.Throws<ArgumentOutOfRangeException>(() => new GameOfLife.Settings(args));
    }

    [Fact]
    public void Constructor_ThrowsArgumentException_WhenTooManyArguments()
    {
        var args = new string[] { "-1", "-1", "20", "20", "100" };
        Assert.Throws<ArgumentException>(() => new GameOfLife.Settings(args));
    }
}