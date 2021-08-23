namespace GameOfLife
{
    public class Grid
    {
        public Cell[,] CellGrid { get; init; }

        /// <summary>
        /// The count of rows (counting vertically) in the grid.
        /// </summary>
        public byte RowCount => (byte) CellGrid.GetLength(0);

        /// <summary>
        /// The count of columns (counting horizontally) in the grid.
        /// </summary>
        public byte ColumnCount => (byte) CellGrid.GetLength(1);

        /// <summary>
        /// A dictionary that maps each cell (key) with its neighbor cells (values).
        /// </summary>
        public Dictionary<Cell, List<Cell>> NeighborMap { get; init; }

        public GridStatus Status { get; private set; } = GridStatus.Alive;

        /// <summary>
        /// A log of the last few cell updates made. Used for testing if the grid is stale.
        /// </summary>
        private Queue<string> ChangeHistory { get; } = new Queue<string>(ChangeHistoryMaxItems);

        private const int ChangeHistoryMaxItems = 7;

        public readonly Dictionary<bool, char> GridChars =
            new()
            {
                { true,  'X' }, // TODO: Convert into a setting.
                { false, '·' }
            };

        public IReadOnlyList<Cell> AllCellsFlattened => CellGrid.Cast<Cell>().ToList();

        public Grid(byte rowCount, byte columnCount,
                    List<Coordinates> cellsToTurnOn)
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

        public Grid(Settings gridSettings)
        {
            CellGrid = new Cell[gridSettings.RowCount, gridSettings.ColumnCount];

            Random random = new();

            // Create the cells and populate the grid with them.
            for (byte row = 0; row < gridSettings.RowCount; row++)
            {
                for (byte column = 0; column < gridSettings.ColumnCount; column++)
                {
                    var shouldTurnOn = random.Next(100) < gridSettings.InitialPopulationRatio;
                    CellGrid[row,column] = new Cell(row, column, shouldTurnOn);
                }
            }

            NeighborMap = GetCellNeighbors(this);
        }

        private static Dictionary<Cell, List<Cell>> GetCellNeighbors(Grid grid)
        {
            var cellsWithNeighbors = new Dictionary<Cell, List<Cell>>();

            foreach (var cell in grid.AllCellsFlattened)
            {
                var neighborCoordinates = GetCellNeighborCoordinates(grid, cell.Coordinates);
                cellsWithNeighbors.Add(cell, GetCellsByCoordinates(grid, neighborCoordinates));
            }

            return cellsWithNeighbors;
        }

        /// <summary>
        /// Returns all valid cell coordinates for a specific cells neighbors.
        /// Invalid coordinates (i.e., negative and those beyond the grid) are ignored.
        /// </summary>
        /// <param name="grid"></param>
        /// <param name="sourceCellCoordinates"></param>
        private static List<Coordinates> GetCellNeighborCoordinates(Grid grid,
                                                                    Coordinates sourceCellCoordinates)
        {
            var potentialCoordinateValues = new List<(int Row, int Column)>();

            // Gather all potential neighbor values, including invalid ones.
            for (var row = -1; row <= 1; row++)
            {
                for (var column = -1; column <= 1; column++)
                {
                    potentialCoordinateValues.Add((sourceCellCoordinates.Row + row,
                                                   sourceCellCoordinates.Column + column));
                }
            }

            // Remove the source cell coordinates, which were automatically included above.
            potentialCoordinateValues.Remove((sourceCellCoordinates.Row,
                                              sourceCellCoordinates.Column));

            var validCoordinateValues = potentialCoordinateValues
                                            .Where(v => v.Row >= 0 &&
                                                        v.Column >= 0 &&
                                                        v.Row < grid.RowCount &&
                                                        v.Column < grid.ColumnCount);

            return validCoordinateValues.Select(v => new Coordinates((byte)v.Row,
                                                                     (byte)v.Column))
                                        .ToList();
        }

        private static List<Cell> GetCellsByCoordinates(Grid grid, List<Coordinates> coordinates)
        {
            return grid.AllCellsFlattened.Where(c => coordinates.Contains(c.Coordinates))
                                         .ToList();
        }

        /// <summary>
        /// Updates the cells in the grid as needed.
        /// </summary>
        public List<Cell> GetUpdatesForNextIteration()
        {
            var cellsToUpdate = Utilities.GetCellsToUpdate(this);

            if (cellsToUpdate.Count == 0)
            {
                this.Status = GridStatus.Stagnated;
                return new List<Cell>();
            }

            foreach (var cell in cellsToUpdate)
                cell.FlipStatus();

            return cellsToUpdate;
        }

        /// <summary>
        /// Print the entire grid to the console.
        /// </summary>
        public void Print()
        {
            Clear();

            for (byte row = 0; row < RowCount; row++)
            {
                for (byte column = 0; column < ColumnCount; column++)
                {
                    var isAlive = CellGrid[row,column].IsAlive;

                    ForegroundColor = isAlive ? ConsoleColor.Green
                                              : ConsoleColor.DarkGray;

                    SetCursorPosition(column, row);

                    Write(GridChars[isAlive]);
                }

                WriteLine();
            }

            ResetColor();
        }

        /// <summary>
        /// Print only updated cells for the current iteration.
        /// </summary>
        /// <param name="cellsForUpdate"></param>
        public void PrintUpdates(List<Cell> cellsForUpdate)
        {
            if (cellsForUpdate.Count == 0)
            {
                this.Status = GridStatus.Stagnated;
                return;
            }

            try
            {
                foreach (var cell in cellsForUpdate)
                {
                    ForegroundColor = cell.IsAlive ? ConsoleColor.Green
                                                   : ConsoleColor.DarkGray;

                    SetCursorPosition(cell.Coordinates.Column, cell.Coordinates.Row);

                    Write(GridChars[cell.IsAlive]);
                }
            }
            catch (Exception ex)
            {
                Clear();
                ResetColor();
                WriteLine(ex.Message);
                throw;
            }

            ResetColor();
        }

        /// <summary>
        /// Updates the change history, then uses it to check for grid staleness.
        /// </summary>
        /// <param name="cellsToUpdate"></param>
        public void UpdateAndCheckChangeHistory(IList<Cell> cellsToUpdate)
        {
            var updateSignature = string.Join(";", cellsToUpdate.Select(c => $"{c.Coordinates.Row},{c.Coordinates.Column},{c.IsAlive}"));

            ChangeHistory.Enqueue(updateSignature);

            // If a identical update exists in the history, then the grid is repeating itself.
            if (ChangeHistory.Count != ChangeHistory.Distinct().Count())
            {
                this.Status = GridStatus.Looping;
                return;
            }

            // Otherwise, remove the oldest history item, if needed.
            if (ChangeHistory.Count > ChangeHistoryMaxItems)
                ChangeHistory.Dequeue();
        }

        public void Abort() => this.Status = GridStatus.Aborted;
    }
}