using Xunit;

namespace GameOfLife.Tests;

public class SettingsTests
{
    [Theory]
    [InlineData("300", "50", "90")]
    [InlineData("50", "30", "20")]
    [InlineData("3", "3", "1")]
    [InlineData("20000", "20000", "99")]
    public void Constructor_Succeeds_With3ProperArguments
        (string width, string height, string populationRatio)
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
    public void Constructor_Succeeds_With4ProperArguments
        (string width, string height, string populationRatio, string delay)
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
    [InlineData("-1", "-1", "1")]
    [InlineData("-1", "-1", "-1", "0")]
    [InlineData("-1", "3", "1", "0")]
    [InlineData("20000", "20000", "99", "20000")]
    public void Constructor_Succeeds_WithNegativeArguments
        (string width, string height, string populationRatio, string delay = "50")
    {
        var args = new string[] { width, height, populationRatio, delay };
        var settings = new Settings(args);

        Assert.NotNull(settings);
        Assert.True(settings.Width > 0);
        Assert.True(settings.Height > 0);
        Assert.True(settings.InitialPopulationRatio > 0 && settings.InitialPopulationRatio < 100);

        if (populationRatio != "-1")
            Assert.Equal(byte.Parse(populationRatio), settings.InitialPopulationRatio);

        Assert.Equal(ushort.Parse(delay), settings.InitialIterationDelayMs);
    }

    [Theory]
    [InlineData("-2", "50", "90")]
    [InlineData("5", "5", "-5")]
    [InlineData("-2", "50", "90", "5000")]
    [InlineData("50", "-10", "20", "10000")]
    public void Constructor_ThrowsOverflowException_WithOutOfRangeNumbers
        (string width, string height, string populationRatio, string delay = "50")
    {
        var args = new string[] { width, height, populationRatio, delay };
        Assert.Throws<OverflowException>(() => new GameOfLife.Settings(args));
    }

    [Theory]
    [InlineData("3.2", "5", "20")]
    [InlineData("5", "0.5", "20")]
    [InlineData("5", "5", "50.5")]
    public void Constructor_ThrowsFormatException_WithNonIntegers
        (string width, string height, string populationRatio, string delay = "50")
    {
        var args = new string[] { width, height, populationRatio, delay };
        Assert.Throws<FormatException>(() => new GameOfLife.Settings(args));
    }

    [Theory]
    [InlineData("50", "-0", "20")]
    [InlineData("5", "5", "200")]
    [InlineData("5", "5", "200", "200000000")]
    [InlineData("5", "5", "200", "-10")]
    public void Constructor_ThrowsArgumentOutOfRangeException_WithOutOfRangeNumbers
        (string width, string height, string populationRatio, string delay = "50")
    {
        var args = new string[] { width, height, populationRatio, delay };
        Assert.Throws<ArgumentOutOfRangeException>(() => new GameOfLife.Settings(args));
    }

    [Theory]
    [InlineData("@", "50", "90")]
    [InlineData("50", "+", "20")]
    [InlineData("5", "5", "::::")]
    [InlineData("A", "5", "20")]
    [InlineData("5", "Bb", "20")]
    [InlineData("5", "5", "101%")]
    [InlineData("あ", "5", "200")]
    public void Constructor_ThrowsFormatException_WithImproperNonNumericArguments
        (string width, string height, string populationRatio, string delay = "50")
    {
        var args = new string[] { width, height, populationRatio, delay };
        Assert.Throws<FormatException>(() => new GameOfLife.Settings(args));
    }

    [Fact]
    public void Constructor_ThrowsArgumentException_WhenTooFewArguments()
    {
        var args = new string[] { "10" };
        Assert.Throws<ArgumentException>(() => new GameOfLife.Settings(args));
    }

    [Fact]
    public void Constructor_ThrowsArgumentException_WhenTooManyArguments()
    {
        var args = new string[] { "-1", "-1", "20", "20", "100" };
        Assert.Throws<ArgumentException>(() => new GameOfLife.Settings(args));
    }
}