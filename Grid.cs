namespace GameOfLife
{
    public class Grid
    {
        public Cell[,] CellGrid { get; init; }
        public byte RowCount => (byte) CellGrid.GetLength(0);
        public byte ColumnCount => (byte) CellGrid.GetLength(1);
        public Dictionary<Cell, IEnumerable<Cell>> NeighborMap { get; init; }
        public bool IsAlive => AllCellsFlattened.Any(c => c.IsAlive);

        public readonly Dictionary<bool, char> GridChars =
            new()
            {
                { true,  'X' }, // TODO: Convert into a setting.
                { false, '·' }
            };

        public IReadOnlyList<Cell> AllCellsFlattened => CellGrid.Cast<Cell>().ToList();

        public Grid(byte rowCount, byte columnCount,
                    IEnumerable<Coordinates> cellsToTurnOn)
        {
            CellGrid = new Cell[rowCount, columnCount];

            // Create all cells and populate the grid with them.
            for (byte row = 0; row < rowCount; row++)
            {
                for (byte column = 0; column < columnCount; column++)
                {
                    var coordinates = new Coordinates(row, column);
                    var shouldTurnOn = cellsToTurnOn.Contains(coordinates);
                    CellGrid[row,column] = new Cell(row, column, shouldTurnOn);
                }
            }

            NeighborMap = GetCellNeighbors(this);

            WriteLine("End of ctor");
        }

        public Grid(GridSettings gridSettings)
        {
            CellGrid = new Cell[gridSettings.RowCount, gridSettings.ColumnCount];

            Random random = new();

            // Create the cells and populate the grid with them.
            for (byte row = 0; row < gridSettings.RowCount; row++)
            {
                for (byte column = 0; column < gridSettings.ColumnCount; column++)
                {
                    var shouldTurnOn = random.Next(100) < gridSettings.LifeProbability;
                    CellGrid[row,column] = new Cell(row, column, shouldTurnOn);
                }
            }

            NeighborMap = GetCellNeighbors(this);
        }

        private static Dictionary<Cell, IEnumerable<Cell>> GetCellNeighbors(Grid grid)
        {
            var cellsWithNeighbors = new Dictionary<Cell, IEnumerable<Cell>>();

            foreach (var cell in grid.AllCellsFlattened)
            {
                var neighborCoordinates = GetCellNeighborCoordinates(grid, cell.Coordinates);
                cellsWithNeighbors.Add(cell, GetCellsByCoordinates(grid, neighborCoordinates));
            }

            return cellsWithNeighbors;
        }

        // TODO: Review this for performance.
        /// <summary>
        /// Returns all valid cell coordinates for a specific cells neighbors.
        /// Invalid coordinates (i.e., negative and those beyond the grid) are ignored.
        /// </summary>
        /// <param name="grid"></param>
        /// <param name="sourceCellCoordinates"></param>
        private static IEnumerable<Coordinates> GetCellNeighborCoordinates(
            Grid grid, Coordinates sourceCellCoordinates)
        {
            var potentialCoordinateValues = new List<(int Row, int Column)>();

            // Gather all potential neighbors, including invalid ones.
            for (var row = -1; row <= 1; row++)
            {
                for (var column = -1; column <= 1; column++)
                {
                    potentialCoordinateValues.Add((sourceCellCoordinates.Row + row,
                                                   sourceCellCoordinates.Column + column));
                }
            }

            // Remove the source cell coordinates, which were included above.
            potentialCoordinateValues.Remove((sourceCellCoordinates.Row,
                                              sourceCellCoordinates.Column));

            var validCoordinateValues = potentialCoordinateValues
                                            .Where(v => v.Row >= 0 &&
                                                        v.Column >= 0 &&
                                                        v.Row < grid.RowCount &&
                                                        v.Column < grid.ColumnCount);

            return validCoordinateValues.Select(v => new Coordinates((byte)v.Row,
                                                                     (byte)v.Column));
        }

        private static IEnumerable<Cell> GetCellsByCoordinates(Grid grid, IEnumerable<Coordinates> coordinates)
        {
            return grid.AllCellsFlattened.Where(c => coordinates.Contains(c.Coordinates));
        }

        public void UpdateForNextIteration()
        {
            foreach (var cell in Utilities.GetCellsToUpdateInParallel(this))
                cell.FlipStatus();
        }

        public void Print(ushort delay = 0)
        {
            Clear();

            for (byte row = 0; row < RowCount; row++)
            {
                for (byte column = 0; column < ColumnCount; column++)
                {
                    var isOn = CellGrid[row,column].IsAlive;

                    ForegroundColor = isOn ? ConsoleColor.Green
                                           : ConsoleColor.DarkGray;

                    SetCursorPosition(column, row);

                    Write(GridChars[isOn]);
                }

                WriteLine();
            }

            ResetColor();

            System.Threading.Thread.Sleep(delay);
        }
    }
}