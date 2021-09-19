using Xunit;
using GameOfLife;

namespace GameOfLife.Tests;

public class SettingsTests
{
    [Theory]
    [InlineData("300", "50", "90")]
    [InlineData("50", "30", "20")]
    public void Constructor_Succeeds_With3ProperArguments(string x, string y, string populationRatio)
    {
        var args = new string[] { x, y, populationRatio };
        var settings = new GameOfLife.Settings(args);

        Assert.NotNull(settings);
        Assert.Equal(byte.Parse(populationRatio), settings.InitialPopulationRatio);
    }

    [Fact]
    public void Constructor_ThrowsArgumentException_WhenTooManyArguments()
    {
        var args = new string[] { "-1", "-1", "20", "20", "100" };
        Assert.Throws<ArgumentException>(() => new GameOfLife.Settings(args));
    }
}