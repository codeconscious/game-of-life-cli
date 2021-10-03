using Xunit;
using GameOfLife.Game;

namespace GameOfLife.Tests;

public class CellGroupTests
{
    [Theory]
    [InlineData(false, false, false, false, ' ')]
    [InlineData(true, false, false, false, '▘')]
    [InlineData(false, true, false, false, '▝')]
    [InlineData(true, true, false, false, '▀')]
    [InlineData(true, true, true, true, '█')]
    public void Constructor_Succeeds_WithValidSettings
        (bool first, bool second, bool third, bool fourth, char expected)
    {
        var testGroup = new CellGroup(
            new Cell(0, 0, first),
            new Cell(1, 0, second),
            new Cell(0, 1, third),
            new Cell(1, 1, fourth));

        var signature = testGroup.GetSignature();
        Console.WriteLine(signature);

        Assert.Equal(CellGroup.GetCharacterToPrint(signature), expected);
    }
}