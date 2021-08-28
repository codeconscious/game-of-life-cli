namespace GameOfLife
{
    public class Grid
    {
        public Cell[,] CellGrid { get; init; }

        /// <summary>
        /// The count of rows (counting vertically) in the grid.
        /// </summary>
        public int RowCount { get; private init; }

        /// <summary>
        /// The count of columns (counting horizontally) in the grid.
        /// </summary>
        public int ColumnCount { get; private init; }

        /// <summary>
        /// A dictionary that maps each cell (key) with its neighbor cells (values).
        /// </summary>
        public IDictionary<Cell, List<Cell>> NeighborMap { get; private init; }

        public List<Cell> AllCellsFlattened { get; private init; }

        public GridStatus Status { get; private set; } = GridStatus.Alive;

        /// <summary>
        /// A log of the last few cell updates made. Used for testing for certain grid statuses.
        /// </summary>
        private Queue<string> ChangeHistory { get; } = new Queue<string>(ChangeHistoryMaxItems);

        private const ushort ChangeHistoryMaxItems = 7; // TODO: Make a setting

        public readonly Dictionary<bool, char> GridChars =
            new()
            {
                { true, 'X' }, // Other candidates: █  // TODO: Convert into a setting.
                { false, '·' }
            };

        public nuint CurrentIteration { get; private set; } = 0;
        public int OutputRow => RowCount + 1;
        public Stopwatch Stopwatch { get; private init; } = new();

        /// <summary>
        /// Constructor that start the game using a specific collection of cells
        /// to initially be given life.
        /// </summary>
        /// <param name="rowCount"></param>
        /// <param name="columnCount"></param>
        /// <param name="startAliveCells">Cells to begin the game alive.</param>
        public Grid(int rowCount, int columnCount,
                    List<CoordinatePair> startAliveCells)
        {
            CellGrid = new Cell[rowCount, columnCount];

            RowCount = CellGrid.GetLength(0);
            ColumnCount = CellGrid.GetLength(1);

            // Create all cells and populate the grid with them.
            for (var row = 0; row < rowCount; row++)
            {
                for (var column = 0; column < columnCount; column++)
                {
                    var coordinates = new CoordinatePair(row, column);
                    var startAlive = startAliveCells.Contains(coordinates);
                    CellGrid[row,column] = new Cell(row, column, startAlive);
                }
            }

            AllCellsFlattened = CellGrid.Cast<Cell>().ToList();

            NeighborMap = GetCellNeighbors(this);

            Stopwatch.Start();
        }

        /// <summary>
        /// Constructor that start the game using specified settings.
        /// </summary>
        /// <param name="gridSettings"></param>
        public Grid(Settings gridSettings)
        {
            CellGrid = new Cell[gridSettings.RowCount, gridSettings.ColumnCount];

            RowCount = CellGrid.GetLength(0);
            ColumnCount = CellGrid.GetLength(1);

            var random = new Random();

            // Create the cells and populate the grid with them.
            for (var row = 0; row < gridSettings.RowCount; row++)
            {
                for (var column = 0; column < gridSettings.ColumnCount; column++)
                {
                    var startAlive = random.Next(100) <= gridSettings.InitialPopulationRatio;
                    CellGrid[row,column] = new Cell(row, column, startAlive);
                }
            }

            AllCellsFlattened = CellGrid.Cast<Cell>().ToList();

            NeighborMap = GetCellNeighbors(this);

            Stopwatch.Start();
        }

        /// <summary>
        /// Gets a dictionary that maps all cells in the given grid with their neighboring cells.
        /// </summary>
        /// <param name="grid"></param>
        private static IDictionary<Cell, List<Cell>> GetCellNeighbors(Grid grid)
        {
            var cellsWithNeighbors = new ConcurrentDictionary<Cell, List<Cell>>();

            Parallel.ForEach(grid.AllCellsFlattened, cell =>
            {
                var neighborCoordinates = GetCellNeighborCoordinates(grid, cell.Coordinates);

                cellsWithNeighbors.TryAdd(cell, GetCellsByCoordinates(grid, neighborCoordinates));
            });

            return cellsWithNeighbors;
        }

        /// <summary>
        /// Returns all valid cell coordinates for a specific cells neighbors.
        /// Invalid coordinates (i.e., negative and those beyond the grid) are ignored.
        /// </summary>
        /// <param name="grid"></param>
        /// <param name="sourcePair"></param>
        /// <param name="shouldWrap">Determines whether cell calculations should wrap around the grid</param>
        private static List<CoordinatePair> GetCellNeighborCoordinates(Grid grid,
                                                                       CoordinatePair sourcePair,
                                                                       bool shouldWrap = true)
        {
            // The most variations likely to be held is 14 (9 - 1 + 5 possible corrections).
            var generatedPairs = new List<CoordinatePair>(14);

            // Gather all potential neighbor values, including invalid ones.
            for (var row = -1; row <= 1; row++)
            {
                for (var column = -1; column <= 1; column++)
                {
                    generatedPairs.Add(
                        new CoordinatePair(sourcePair.Row + row,
                                           sourcePair.Column + column));
                }
            }

            // Remove the source cell coordinates, which were automatically included above.
            generatedPairs.Remove(sourcePair);

            if (shouldWrap)
            {
                var correctedPairs = new List<CoordinatePair>();

                foreach (var invalidPair in generatedPairs.Where(p => !p.IsValid(grid.RowCount, grid.ColumnCount)))
                {
                    var workingPair = invalidPair;

                    if (workingPair.Row < 0)
                        workingPair = workingPair with { Row = grid.RowCount - 1 };
                    if (workingPair.Row >= grid.RowCount)
                        workingPair = workingPair with { Row = 0 };
                    if (workingPair.Column < 0)
                        workingPair = workingPair with { Column = grid.ColumnCount - 1 };
                    if (workingPair.Column >= grid.ColumnCount)
                        workingPair = workingPair with { Column = 0 };

                    correctedPairs.Add(workingPair);
                }

                generatedPairs.AddRange(correctedPairs);
            }

            var validPairs = generatedPairs.Where(p => p.IsValid(grid.RowCount, grid.ColumnCount));

            return validPairs.Select(v => new CoordinatePair(v.Row, v.Column))
                             .ToList();
        }

        /// <summary>
        /// Get a collection of cells from a collection of their respective coordinates.
        /// </summary>
        /// <param name="grid"></param>
        /// <param name="coordinates"></param>
        private static List<Cell> GetCellsByCoordinates(Grid grid, List<CoordinatePair> coordinates)
        {
            var output = new List<Cell>(coordinates.Count);

            foreach (var pair in coordinates)
                output.Add(grid.CellGrid[pair.Row, pair.Column]);

            return output;
        }

        /// <summary>
        /// Updates the cells in the grid as needed, then returns the affected cells.
        /// </summary>
        public List<Cell> GetUpdatesForNextIteration()
        {
            var cellsToUpdate = GetCellsToFlip(this);

            if (cellsToUpdate.Count == 0)
            {
                //UpdateStatus(GridStatus.Stagnated);
                return new List<Cell>();
            }

            foreach (var cell in cellsToUpdate)
                cell.FlipStatus();

            return cellsToUpdate;
        }

        /// <summary>
        /// Get a list of grid cells whose statuses should be flipped (reversed).
        /// </summary>
        /// <param name="grid"></param>
        public static List<Cell> GetCellsToFlip(Grid grid)
        {
            var cellsToFlip = new List<Cell>();

            foreach (var cell in grid.AllCellsFlattened)
            {
                var livingNeighborCount = grid.NeighborMap[cell].Count(c => c.IsAlive);

                var willCellBeAlive = cell.WillCellBeAliveNextIteration(livingNeighborCount);

                if (cell.IsAlive != willCellBeAlive)
                    cellsToFlip.Add(cell);
            }

            return cellsToFlip;
        }

        /// <summary>
        /// Updates the change history, then uses it to check grid status.
        /// </summary>
        /// <param name="cellsToUpdate"></param>
        public void CheckGridStatus(IList<Cell> cellsToUpdate)
        {
            CurrentIteration++;

            // No living cell means grid death.
            if (!AllCellsFlattened.Any(c => c.IsAlive))
            {
                UpdateStatus(GridStatus.Dead);
                return;
            }

            // No updates means stagnation.
            if (!cellsToUpdate.Any())
            {
                UpdateStatus(GridStatus.Stagnated);
                return;
            }

            var updateSignature = string.Concat(
                cellsToUpdate.Select(c => $"{c.Coordinates.Row},{c.Coordinates.Column},{c.IsAlive}"));

            ChangeHistory.Enqueue(updateSignature);

            // Identical updates in the history indicate that the grid is looping.
            if (this.Status != GridStatus.Looping &&
                ChangeHistory.Count != ChangeHistory.Distinct().Count())
            {
                UpdateStatus(GridStatus.Looping);
                return;
            }

            // Otherwise, just remove the oldest history item, if needed.
            if (ChangeHistory.Count > ChangeHistoryMaxItems)
                ChangeHistory.Dequeue();
        }

        /// <summary>
        /// Update the grid status, then also print the game status if needed.
        /// </summary>
        /// <param name="newStatus"></param>
        public void UpdateStatus(GridStatus newStatus)
        {
            var shouldPrintResults = Status == GridStatus.Alive;

            Status = newStatus;

            if (shouldPrintResults)
                this.PrintGameStatus();
        }
    }
}