namespace GameOfLife
{
    public class Board
    {
        public Cell[,] CellGrid { get; set; }
        public byte RowCount => (byte) CellGrid.GetLength(0);
        public byte ColumnCount => (byte) CellGrid.GetLength(1);

        public readonly IReadOnlyDictionary<bool, string> GridChars =
            new Dictionary<bool, string>
            {
                { true,  "X" },
                { false, "•" }
            };

        public Board(byte rowCount, byte columnCount,
                     IEnumerable<Coordinates> cellsToTurnOn = default)
        {
            CellGrid = new Cell[rowCount, columnCount];

            // Create the cells and populate the grid with them.
            for (byte row = 0; row < rowCount; row++)
            {
                for (byte column = 0; column < columnCount; column++)
                {
                    var testLocation = new Coordinates(row, column);
                    var shouldTurnOn = cellsToTurnOn.Contains(testLocation);
                    CellGrid[row,column] = new Cell(row, column, shouldTurnOn);
                }
            }
        }

        public void PrintGrid()
        {
            // TODO: Try using LINQ instead.
            for (byte row = 0; row < RowCount; row++)
            {
                for (byte column = 0; column < ColumnCount; column++)
                {
                    var isOn = CellGrid[row,column].IsOn;

                    ForegroundColor = isOn ? ConsoleColor.Green
                                           : ConsoleColor.DarkGray;

                    Write(GridChars[isOn]);
                }

                WriteLine();
            }

            ResetColor();
        }
    }
}