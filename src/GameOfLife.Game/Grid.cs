namespace GameOfLife.Game
{
    public sealed class Grid
    {
        public bool IsHighResMode { get; private init; }
        public Cell[,] CellGrid { get; init; }
        public int Width { get; private init; }
        public int Height { get; private init; }
        public int Area => CellGrid.GetLength(0) * CellGrid.GetLength(1);
        public Dimensions ScreenDimensions { get; private init; }


        // public List<CellGroup> CellGroups { get; init; }
        public Dictionary<Cell, CellGroup> CellGroupMap = new();

        /// <summary>
        /// A dictionary that maps each cell (key) with its neighbor cells (values).
        /// </summary>
        public IDictionary<Cell, List<Cell>> NeighborMap { get; private init; }

        public List<Cell> AllCellsFlattened { get; private init; }

        public float PopulationRatio
            => (float) AllCellsFlattened.Count(c => c.IsAlive) / Area;

        // The state of the grid should only change once, when the game ends.
        public GridState State { get; private set; } = GridState.Alive;

        /// <summary>
        /// The delay in milliseconds between two consecutive iterations.
        /// </summary>
        public ushort IterationDelayMs { get; private set; }

        /// <summary>
        /// A log of the last few cell updates made. Used for testing for certain grid states.
        /// </summary>
        private Queue<string> ChangeHistory { get; } = new Queue<string>(ChangeHistoryMaxItems);

        private const ushort ChangeHistoryMaxItems = 5;

        public readonly Dictionary<bool, char> GridChars =
            new()
            {
                { true, 'X' }, // TODO: Convert into a setting.
                { false, ' ' }
            };

        public nuint CurrentIteration { get; private set; }
        public int OutputRow => ScreenDimensions.Height;
        public Stopwatch GameStopwatch { get; private init; } = new();

        public IPrinter GridPrinter { get; private init; }

        #region Setup

        public Grid(IGridSettings gridSettings, IPrinter gridPrinter)
        {
            ScreenDimensions = new Dimensions(gridSettings.Width, gridSettings.Height);
            IterationDelayMs = gridSettings.IterationDelayMs;
            Random random = new();

            if (gridSettings.UseHighResMode)
            {
                IsHighResMode = true;
                CellGrid = new Cell[gridSettings.Width * 2, gridSettings.Height * 2];
                Width = CellGrid.GetLength(0);
                Height = CellGrid.GetLength(1);

                // Create the cells and populate the grid with them.
                for (int x = 0; x < gridSettings.Width * 2; x++)
                {
                    for (int y = 0; y < gridSettings.Height * 2; y++)
                    {
                        bool startAlive = random.Next(100) <= gridSettings.PopulationRatio;
                        CellGrid[x,y] = new Cell(x, y, startAlive);
                    }
                }

                CellGroupMap = CreateHighResCellGroupMap();
            }
            else // Use normal resolution
            {
                IsHighResMode = false;
                CellGrid = new Cell[gridSettings.Width, gridSettings.Height];
                Width = CellGrid.GetLength(0);
                Height = CellGrid.GetLength(1);

                // Create the cells and populate the grid with them.
                for (int x = 0; x < gridSettings.Width; x++)
                {
                    for (int y = 0; y < gridSettings.Height; y++)
                    {
                        bool startAlive = random.Next(100) <= gridSettings.PopulationRatio;
                        CellGrid[x,y] = new Cell(x, y, startAlive);
                    }
                }
            }

            AllCellsFlattened = CellGrid.Cast<Cell>().ToList();
            NeighborMap = GetCellNeighbors(this);
            GridPrinter = gridPrinter;

            GameStopwatch.Start();
        }

        /// <summary>
        /// Gets a dictionary that maps all cells in the given grid with their neighboring cells.
        /// </summary>
        /// <param name="grid"></param>
        private static IDictionary<Cell, List<Cell>> GetCellNeighbors(Grid grid)
        {
            ConcurrentDictionary<Cell, List<Cell>> cellsWithNeighbors = new();

            Parallel.ForEach(grid.AllCellsFlattened, cell =>
            {
                List<Point> neighborCoordinates = GetCellNeighborCoordinates(grid, cell.Location);
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
        /// <param name="shouldWrap">Determines whether the grid should wrap.</param>
        private static List<Point> GetCellNeighborCoordinates(
            Grid grid,
            Point sourcePair,
            bool shouldWrap = true)
        {
            // The most variations likely to be held is 14 (9 - 1 + 5 possible corrections).
            List<Point> generatedPairs = new(14);

            // Gather all potential neighbor values, including invalid ones.
            for (int x = -1; x <= 1; x++)
            {
                for (int y = -1; y <= 1; y++)
                {
                    generatedPairs.Add(
                        new Point(
                            sourcePair.X + x,
                            sourcePair.Y + y));
                }
            }

            // Remove the source cell coordinates, which were automatically included above.
            generatedPairs.Remove(sourcePair);

            if (shouldWrap)
            {
                List<Point> correctedPoints = [];

                IEnumerable<Point> invalidPairs = generatedPairs
                    .Where(p => !p.IsValid(grid.Width, grid.Height));

                foreach (Point invalidPair in invalidPairs)
                {
                    Point workingPair = invalidPair;

                    if (workingPair.X < 0)
                        workingPair = workingPair with { X = grid.Width - 1 };
                    if (workingPair.X >= grid.Width)
                        workingPair = workingPair with { X = 0 };
                    if (workingPair.Y < 0)
                        workingPair = workingPair with { Y = grid.Height - 1 };
                    if (workingPair.Y >= grid.Height)
                        workingPair = workingPair with { Y = 0 };

                    correctedPoints.Add(workingPair);
                }

                generatedPairs.AddRange(correctedPoints);
            }

            IEnumerable<Point> validPairs = generatedPairs
                .Where(p => p.IsValid(grid.Width, grid.Height));

            return validPairs
                .Select(v => new Point(v.X, v.Y))
                .ToList();
        }

        /// <summary>
        /// Get a collection of cells from a collection of their respective coordinates.
        /// </summary>
        /// <param name="grid"></param>
        /// <param name="coordinates"></param>
        private static List<Cell> GetCellsByCoordinates(Grid grid, List<Point> coordinates)
        {
            List<Cell> output = new(coordinates.Count);

            foreach (Point pair in coordinates)
                output.Add(grid.CellGrid[pair.X, pair.Y]);

            return output;
        }

        public Dictionary<Cell, CellGroup> CreateHighResCellGroupMap()
        {
            Dictionary<Cell, CellGroup>map = [];

            for (int y = 0; y < Height; y += 2)
            {
                for (int x = 0; x < Width; x += 2)
                {
                    CellGroup newGroup = new(
                        CellGrid[x, y],
                        CellGrid[x + 1, y],
                        CellGrid[x, y + 1],
                        CellGrid[x + 1, y + 1],
                        new Point(x / 2, y / 2));

                    foreach (Cell cell in newGroup.MemberCells.Values.ToList())
                    {
                        map.Add(cell, newGroup);
                    }
                }
            }

            return map;
        }

        #endregion

        /// <summary>
        /// Iterate (advance) the grid to its next incarnation and print the updates.
        /// </summary>
        public void Iterate()
        {
            List<Cell> iterationCells = this.GetCellsToFlip();
            Cell.FlipLifeStatuses(iterationCells);
            this.UpdateHistoryAndGameState(iterationCells);

            if (IsHighResMode)
            {
                List<CellGroup> iterationGroups = new(iterationCells.Count);

                foreach (Cell cell in iterationCells)
                {
                    iterationGroups.Add(CellGroupMap[cell]);
                }

                GridPrinter.WriteUpdates(this, iterationGroups);
            }
            else
            {
                GridPrinter.WriteUpdates(this, iterationCells);
            }
        }

        /// <summary>
        /// Get a list of grid cells whose statuses should be flipped (reversed) this iteration.
        /// </summary>
        private List<Cell> GetCellsToFlip()
        {
            List<Cell> cellsToFlip = [];

            foreach (Cell cell in this.AllCellsFlattened)
            {
                int livingNeighborCount = this.NeighborMap[cell].Count(c => c.IsAlive);

                bool willSurvive = cell.ShouldCellLive(livingNeighborCount);

                if (cell.IsAlive != willSurvive)
                    cellsToFlip.Add(cell);
            }

            return cellsToFlip;
        }

        /// <summary>
        /// Updates the grid change history, then uses it to check grid state.
        /// </summary>
        /// <param name="recentlyFlippedCells"></param>
        private void UpdateHistoryAndGameState(IList<Cell> recentlyFlippedCells)
        {
            CurrentIteration++;

            // No living cell means grid death.
            if (!AllCellsFlattened.Any(c => c.IsAlive))
            {
                UpdateState(GridState.Extinct);
                return;
            }

            // No updates means stagnation.
            if (recentlyFlippedCells.Count == 0)
            {
                UpdateState(GridState.Stagnated);
                return;
            }

            string updatedSignature = string.Concat(
                recentlyFlippedCells.Select(c =>
                    $"{c.Location.X},{c.Location.Y},{c.IsAlive}"));

            ChangeHistory.Enqueue(updatedSignature);

            // Identical updates in the history indicate that the grid is looping.
            if (this.State != GridState.Looping &&
                ChangeHistory.Count != ChangeHistory.Distinct().Count())
            {
                UpdateState(GridState.Looping);
                return;
            }

            // Otherwise, just remove the oldest history item, if needed.
            if (ChangeHistory.Count > ChangeHistoryMaxItems)
                ChangeHistory.Dequeue();
        }

        /// <summary>
        /// Update the grid state, then also print the game state if needed.
        /// </summary>
        /// <param name="newState"></param>
        private void UpdateState(GridState newState)
        {
            bool wasAlive = State == GridState.Alive;

            State = newState;

            if (wasAlive)
            {
                GameStopwatch.Stop();
                GridPrinter.WriteGameSummary(this);
                IterationDelayMs = 250; // TODO: Clean up this magic number.
            }
        }

        public void AbortGame() => UpdateState(GridState.Aborted);

        /// <summary>
        /// Adjust the iteration delay within the extent of valid values.
        /// </summary>
        /// <param name="adjustMs">The number of milliseconds (negative or positive) to adjust by.</param>
        public void AdjustIterationDelayBy(short adjustMs)
        {
            int proposedDelay = IterationDelayMs + adjustMs;

            IterationDelayMs = proposedDelay switch
            {
                <= ushort.MinValue => ushort.MinValue,
                >= ushort.MaxValue => ushort.MaxValue,
                _ => (ushort) proposedDelay
            };
        }
    }
}
