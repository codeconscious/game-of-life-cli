namespace GameOfLife.Game;

/// <summary>
/// Represents a single group of cells.
/// </summary>
public class CellGroup : IPrintableUnit
{
    public Dictionary<CellGroupLocation, Cell> MemberCells { get; init; } = new(4);
    public Point WriteLocation { get; init; }

    private CellGroup() { }

    public CellGroup(Cell upperLeftCell, Cell upperRightCell, Cell lowerLeftCell, Cell lowerRightCell, Point location)
    {
        MemberCells.Add(CellGroupLocation.UpperLeft, upperLeftCell);
        MemberCells.Add(CellGroupLocation.UpperRight, upperRightCell);
        MemberCells.Add(CellGroupLocation.LowerLeft, lowerLeftCell);
        MemberCells.Add(CellGroupLocation.LowerRight, lowerRightCell);

        WriteLocation = location;
    }

    public void AddCell(Cell cell, CellGroupLocation locationInGroup)
    {
        MemberCells.Add(locationInGroup, cell);
    }

    /// <summary>
    /// Returns a single character that should be displayed for this cell group.
    /// </summary>
    public char GetCellLifeCharacter()
    {
        int total = 0;

        foreach (Cell cellStatus in MemberCells.Values.ToList().Select(c => c).Reverse())
        {
            total <<= 1;
            total |= (byte) (cellStatus.IsAlive ? 1 : 0);
        }

        return total switch
        {
            // https://unicode-table.com/en/blocks/block-elements/
            15 => '█',
            14 => '▟',
            13 => '▙',
            12 => '▄',
            11 => '▜',
            10 => '▐',
            9 => '▚',
            8 => '▗',
            7 => '▛',
            6 => '▞',
            5 => '▌',
            4 => '▖',
            3 => '▀', // Upper half block
            2 => '▝',
            1 => '▘',
            _ => ' '
        };
    }
}
