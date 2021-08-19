namespace GameOfLife
{
    public class Board
    {
        public Cell[,] CellGrid { get; init; }
        public byte RowCount => (byte) CellGrid.GetLength(0);
        public byte ColumnCount => (byte) CellGrid.GetLength(1);

        public readonly IReadOnlyDictionary<bool, string> GridChars =
            new Dictionary<bool, string>
            {
                { true,  "X" },
                { false, "•" }
            };

        public Board(byte rowCount, byte columnCount,
                     IEnumerable<Coordinates> cellsToTurnOn)
        {
            CellGrid = new Cell[rowCount, columnCount];

            // Create the cells and populate the grid with them.
            for (byte row = 0; row < rowCount; row++)
            {
                for (byte column = 0; column < columnCount; column++)
                {
                    var coodinates = new Coordinates(row, column);
                    var shouldTurnOn = cellsToTurnOn.Contains(coodinates);
                    CellGrid[row,column] = new Cell(row, column, shouldTurnOn);
                }
            }
        }

        public Board(byte rowCount, byte columnCount, byte probability)
        {
            CellGrid = new Cell[rowCount, columnCount];

            Random random = new();

            // Create the cells and populate the grid with them.
            for (byte row = 0; row < rowCount; row++)
            {
                for (byte column = 0; column < columnCount; column++)
                {
                    var shouldTurnOn = random.Next(100) < probability;
                    CellGrid[row,column] = new Cell(row, column, shouldTurnOn);
                }
            }
        }

        public void Print()
        {
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