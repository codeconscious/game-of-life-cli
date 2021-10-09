using Xunit;

namespace GameOfLife.Tests;

public class SettingsTests
{
    private static readonly TestPrinter Printer = new();

    [Theory]
    [InlineData("0", "300", "50", "90")]
    [InlineData("0", "50", "30", "20")]
    [InlineData("0", "3", "3", "1")]
    [InlineData("0", "2000", "2000", "99")]
    public void Constructor_Succeeds_With3ProperArguments
        (string useHiRes, string width, string height, string populationRatio)
    {
        var args = new string[] { useHiRes, width, height, populationRatio };
        var settings = new Settings(args, Printer);

        Assert.NotNull(settings);
        Assert.Equal(ushort.Parse(width), settings.Width);
        Assert.Equal(ushort.Parse(height), settings.Height);
        Assert.Equal(byte.Parse(populationRatio), settings.InitialPopulationRatio);
    }

    [Theory]
    [InlineData("0", "300", "50", "90", "500")]
    [InlineData("0", "50", "30", "20", "10000")]
    [InlineData("0", "3", "3", "1", "0")]
    [InlineData("0", "2000", "2000", "99", "30000")]
    public void Constructor_Succeeds_With4ProperArguments
        (string useHiRes, string width, string height, string populationRatio, string delay)
    {
        var args = new string[] { useHiRes, width, height, populationRatio, delay };
        var settings = new Settings(args, Printer);

        Assert.NotNull(settings);
        Assert.Equal(ushort.Parse(width), settings.Width);
        Assert.Equal(ushort.Parse(height), settings.Height);
        Assert.Equal(byte.Parse(populationRatio), settings.InitialPopulationRatio);
        Assert.Equal(ushort.Parse(delay), settings.InitialIterationDelayMs);
    }

    [Theory]
    [InlineData("0", "-1", "-1", "1")]
    [InlineData("0", "-1", "-1", "-1", "0")]
    [InlineData("0", "-1", "3", "1", "0")]
    [InlineData("0", "2000", "2000", "99", "30000")]
    public void Constructor_Succeeds_WithNegativeArguments
        (string useHiRes, string width, string height, string populationRatio, string delay = "50")
    {
        var args = new string[] { useHiRes, width, height, populationRatio, delay };
        var settings = new Settings(args, Printer);

        Assert.NotNull(settings);
        Assert.True(settings.Width > 0);
        Assert.True(settings.Height > 0);
        Assert.True(settings.InitialPopulationRatio > 0 && settings.InitialPopulationRatio < 100);

        if (populationRatio != "-1")
            Assert.Equal(byte.Parse(populationRatio), settings.InitialPopulationRatio);

        Assert.Equal(ushort.Parse(delay), settings.InitialIterationDelayMs);
    }

    [Theory]
    [InlineData("0", "-2", "50", "90")]
    [InlineData("0", "5", "5", "-5")]
    [InlineData("0", "-2", "50", "90", "5000")]
    [InlineData("0", "50", "-10", "20", "10000")]
    public void Constructor_ThrowsOverflowException_WithOutOfRangeNumbers
        (string useHiRes, string width, string height, string populationRatio, string delay = "50")
    {
        var args = new string[] { useHiRes, width, height, populationRatio, delay };
        Assert.Throws<OverflowException>(() => new Settings(args, Printer));
    }

    [Theory]
    [InlineData("0", "3.2", "5", "20")]
    [InlineData("0", "5", "0.5", "20")]
    [InlineData("0", "5", "5", "50.5")]
    public void Constructor_ThrowsFormatException_WithNonIntegers
        (string useHiRes, string width, string height, string populationRatio, string delay = "50")
    {
        var args = new string[] { useHiRes, width, height, populationRatio, delay };
        Assert.Throws<FormatException>(() => new Settings(args, Printer));
    }

    [Theory]
    [InlineData("0", "50", "-0", "20")]
    [InlineData("0", "5", "5", "200")]
    [InlineData("0", "5", "5", "200", "200000000")]
    [InlineData("0", "5", "5", "200", "-10")]
    public void Constructor_ThrowsArgumentOutOfRangeException_WithOutOfRangeNumbers
        (string useHiRes, string width, string height, string populationRatio, string delay = "50")
    {
        var args = new string[] { useHiRes, width, height, populationRatio, delay };

        Assert.Throws<ArgumentOutOfRangeException>(() => new Settings(args, Printer));
    }

    [Theory]
    [InlineData("0", "@", "50", "90")]
    [InlineData("0", "50", "+", "20")]
    [InlineData("0", "5", "5", "::::")]
    [InlineData("0", "A", "5", "20")]
    [InlineData("0", "5", "Bb", "20")]
    [InlineData("0", "5", "5", "101%")]
    [InlineData("0", "あ", "5", "200")]
    public void Constructor_ThrowsFormatException_WithImproperNonNumericArguments
        (string useHiRes, string width, string height, string populationRatio, string delay = "50")
    {
        var args = new string[] { useHiRes, width, height, populationRatio, delay };

        Assert.Throws<FormatException>(() => new Settings(args, Printer));
    }

    [Fact]
    public void Constructor_ThrowsArgumentException_WhenTooFewArguments()
    {
        var args = new string[] { "10" };

        Assert.Throws<ArgumentException>(() => new Settings(args, Printer));
    }

    [Fact]
    public void Constructor_ThrowsArgumentException_WhenTooManyArguments()
    {
        var args = new string[] { "0", "-1", "-1", "20", "20", "100" };

        Assert.Throws<ArgumentException>(() => new Settings(args, Printer));
    }
}