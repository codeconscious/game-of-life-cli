using Xunit;

namespace GameOfLife.Tests;

public sealed class CellGroupTests
{
    /*
    [Theory]
    [InlineData(false, false, false, false, ' ')]
    [InlineData(true, false, false, false, '▘')]
    [InlineData(false, true, false, false, '▝')]
    [InlineData(true, true, false, false, '▀')]
    [InlineData(false, false, true, false, '▖')]
    [InlineData(true, false, true, false, '▌')]
    [InlineData(false, true, true, false, '▞')]
    [InlineData(true, true, true, false, '▛')]
    [InlineData(false, false, false, true, '▗')]
    [InlineData(true, false, false, true, '▚')]
    [InlineData(false, true, false, true, '▐')]
    [InlineData(true, true, false, true, '▜')]
    [InlineData(false, false, true, true, '▄')]
    [InlineData(true, false, true, true, '▙')]
    [InlineData(false, true, true, true, '▟')]
    [InlineData(true, true, true, true, '█')]
    public void Constructor_Succeeds_WithValidSettings
        (bool first, bool second, bool third, bool fourth, char expected)
    {
        var testGroup = new CellGroup(
            new Cell(0, 0, first),
            new Cell(1, 0, second),
            new Cell(0, 1, third),
            new Cell(1, 1, fourth));

        var signature = testGroup.GetCellLifeSignature();
        // Console.WriteLine(signature);

        Assert.Equal(CellGroup.GetCharacterToPrint(signature), expected);
    }
    */
}