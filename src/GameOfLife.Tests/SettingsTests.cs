using Xunit;
using GameOfLife;

namespace GameOfLife.Tests;

public class SettingsTests
{
    [Fact]
    public void Constructor_Succeeds_With3ProperArguments()
    {
        var args = new string[] { "-1", "-1", "20" };
        var settings = new GameOfLife.Settings(args);

        Assert.NotNull(settings);
        Assert.Equal(20, settings.InitialPopulationRatio);
    }

    [Fact]
    public void Constructor_ThrowsArgumentException_WhenTooManyArguments()
    {
        var args = new string[] { "-1", "-1", "20", "20", "100" };
        //var settings = new GameOfLife.Settings(args);

        Assert.Throws<ArgumentException>(() => new GameOfLife.Settings(args));
    }
}