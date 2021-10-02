namespace GameOfLife.Game;

public class CellGroup
{
    // public Cell[,] MemberCells { get; init; } = new Cell[2,2];
    public Dictionary<CellGroupLocation, Cell> MemberCells { get; init; } = new(4);

    public CellGroup() { }

    public CellGroup(Cell upperLeftCell, Cell upperRightCell, Cell lowerLeftCell, Cell lowerRightCell)
    {
        // MemberCells[0,0] = upperLeftCell;
        // MemberCells[1,0] = upperRightCell;
        // MemberCells[0,1] = lowerLeftCell;
        // MemberCells[1,1] = lowerRightCell;

        MemberCells.Add(CellGroupLocation.UpperLeft, upperLeftCell);
        MemberCells.Add(CellGroupLocation.UpperRight, upperRightCell);
        MemberCells.Add(CellGroupLocation.LowerLeft, lowerLeftCell);
        MemberCells.Add(CellGroupLocation.LowerRight, lowerRightCell);
    }

    public void AddCell(Cell cell, CellGroupLocation locationInGroup)
    {
        MemberCells.Add(locationInGroup, cell);
    }

    [Flags]
    public enum CellGroupLocation : byte
    {
        UpperLeft = 0,
        UpperRight = 1,
        LowerLeft = 2,
        LowerRight = 4
    }

    /// <summary>
    /// Convert the life status to a single hex char??
    /// </summary>
    /// <returns></returns>
    public int GetSignature()
    {
        var total = 0;

        // TODO: Do without LINQ.
        foreach (var cellStatus in MemberCells.Values.ToList())
        {
            total <<= 1;
            // Write("  " + total + "  ");

            total |= (byte) (cellStatus.IsAlive ? 1 : 0);
        }

        return total;
    }

    public static char GetCharacterToPrint(int signature)
    {
        return signature switch
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