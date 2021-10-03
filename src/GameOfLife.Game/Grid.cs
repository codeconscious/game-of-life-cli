using System.Drawing;

namespace GameOfLife.Game
{
    public class Grid
    {
        public Cell[,] CellGrid { get; init; }
        public int Width { get; private init; }
        public int Height { get; private init; }
        public bool IsHighResMode { get; private init; }

        public float CellCount => Width * Height;

        // public List<CellGroup> CellGroups { get; init; }
        public Dictionary<Cell, CellGroup> CellGroupMap = new();

        /// <summary>
        /// A dictionary that maps each cell (key) with its neighbor cells (values).
        /// </summary>
        public IDictionary<Cell, List<Cell>> NeighborMap { get; private init; }

        public List<Cell> AllCellsFlattened { get; private init; }

        public float PopulationRatio
            => AllCellsFlattened.Count(c => c.IsAlive) / CellCount;

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
                { true, 'X' }, // Other candidates: █  // TODO: Convert into a setting.
                { false, ' ' }
            };

        public nuint CurrentIteration { get; private set; }
        public int OutputRow => Height / (IsHighResMode ? 2 : 1);
        public Stopwatch GameStopwatch { get; private init; } = new();

        public IPrinter GridPrinter { get; private init; }

        #region Setup

        /// <summary>
        /// Constructor that start the game using specified settings.
        /// </summary>
        /// <param name="gridSettings"></param>
        public Grid(IGridSettings gridSettings, IPrinter gridPrinter)
        {
            CellGrid = new Cell[gridSettings.Width, gridSettings.Height];

            Width = CellGrid.GetLength(0);
            Height = CellGrid.GetLength(1);
            GridPrinter = gridPrinter;
            IsHighResMode = gridSettings.UseHighResMode;

            IterationDelayMs = gridSettings.InitialIterationDelayMs;

            var random = new Random();

            // Create the cells and populate the grid with them.
            for (var x = 0; x < gridSettings.Width; x++)
            {
                for (var y = 0; y < gridSettings.Height; y++)
                {
                    var startAlive = random.Next(100) <= gridSettings.InitialPopulationRatio;
                    CellGrid[x,y] = new Cell(x, y, startAlive);
                }
            }

            // CellGroups = CreateCellGroups();
            if (gridSettings.UseHighResMode)
                CreateCellGroupMap();

            AllCellsFlattened = CellGrid.Cast<Cell>().ToList();

            NeighborMap = GetCellNeighbors(this);

            GameStopwatch.Start();
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
                var neighborCoordinates = GetCellNeighborCoordinates(grid, cell.Location);

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
        private static List<Point> GetCellNeighborCoordinates(Grid grid,
                                                              Point sourcePair,
                                                              bool shouldWrap = true)
        {
            // The most variations likely to be held is 14 (9 - 1 + 5 possible corrections).
            var generatedPairs = new List<Point>(14);

            // Gather all potential neighbor values, including invalid ones.
            for (var x = -1; x <= 1; x++)
            {
                for (var y = -1; y <= 1; y++)
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
                var correctedPoints = new List<Point>();

                foreach (var invalidPair in generatedPairs.Where(p => !p.IsValid(grid.Width, grid.Height)))
                {
                    var workingPair = invalidPair;

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

            var validPairs = generatedPairs.Where(p => p.IsValid(grid.Width, grid.Height));

            return validPairs.Select(v => new Point(v.X, v.Y))
                             .ToList();
        }

        /// <summary>
        /// Get a collection of cells from a collection of their respective coordinates.
        /// </summary>
        /// <param name="grid"></param>
        /// <param name="coordinates"></param>
        private static List<Cell> GetCellsByCoordinates(Grid grid, List<Point> coordinates)
        {
            var output = new List<Cell>(coordinates.Count);

            foreach (var pair in coordinates)
                output.Add(grid.CellGrid[pair.X, pair.Y]);

            return output;
        }

        public void CreateCellGroupMap()
        {
            // var groups = new List<CellGroup>((Width / 2) * (Height / 2));

            for (var y = 0; y < Height; y += 2)
            {
                for (var x = 0; x < Width; x += 2)
                {
                    var newGroup = new CellGroup(
                        CellGrid[x, y],
                        CellGrid[x + 1, y],
                        CellGrid[x, y + 1],
                        CellGrid[x + 1, y + 1]);
                    // var newGroup = new CellGroup();
                    // newGroup.AddCell(CellGrid[x, y], CellGroup.CellGroupLocation.UpperLeft);
                    // newGroup.AddCell(CellGrid[x + 1, y], CellGroup.CellGroupLocation.UpperRight);
                    // newGroup.AddCell(CellGrid[x, y + 1], CellGroup.CellGroupLocation.LowerLeft);
                    // newGroup.AddCell(CellGrid[x + 1, y + 1], CellGroup.CellGroupLocation.LowerRight);

                    foreach (var cell in newGroup.MemberCells.Values.ToList())
                    {
                        CellGroupMap.Add(cell, newGroup);
                    }

                    // WriteLine(string.Join("; ", CellGroupMap.Values.ToList().SelectMany(g => g.MemberCells.Select(c => c.Value.Location)).Where(p => p.X == 0)));

                    // foreach(var group in newGroup.MemberCells)
                    //     WriteLine(group.Value.Location);
                    // WriteLine();

                    // groups.Add(newGroup);
                }
            }

            // return groups;
        }

        #endregion

        /// <summary>
        /// Iterate (advance) the grid to its next incarnation and print the updates.
        /// </summary>
        public void Iterate()
        {
            var cellsToFlip = this.GetCellsToFlip();
            Cell.FlipLifeStatuses(cellsToFlip);
            this.UpdateHistoryAndGameState(cellsToFlip);

            if (IsHighResMode)
            {
                // var affectedGroups = CellGroups.Where(g => cellsToFlip.Any(c => g.MemberCells.Values.ToList().Contains(c))).ToList();
                var affectedGroups = new List<CellGroup>(cellsToFlip.Count);

                foreach (var cell in cellsToFlip)
                {
                    if (CellGroupMap.ContainsKey(cell))
                    {
                        affectedGroups.Add(CellGroupMap[cell]);
                    }
                    else
                    {
                        // WriteLine(string.Join("; ", CellGroupMap.Values.ToList().SelectMany(g => g.MemberCells.Select(c => c.Value.Location)).Where(p => p.X == 0).Distinct()));
                        throw new InvalidOperationException($"Cell {cell.Location} is not in {nameof(CellGroupMap)}!");
                    }
                }
                GridPrinter.PrintUpdates(affectedGroups, this);
            }
            else
            {
                GridPrinter.PrintUpdates(this, cellsToFlip);
            }
        }

        /// <summary>
        /// Get a list of grid cells whose statuses should be flipped (reversed) this iteration.
        /// </summary>
        private List<Cell> GetCellsToFlip()
        {
            var cellsToFlip = new List<Cell>();

            foreach (var cell in this.AllCellsFlattened)
            {
                var livingNeighborCount = this.NeighborMap[cell].Count(c => c.IsAlive);

                var willCellBeAlive = cell.ShouldCellLive(livingNeighborCount);

                if (cell.IsAlive != willCellBeAlive)
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

            var updateSignature = string.Concat(
                recentlyFlippedCells.Select(c => $"{c.Location.X},{c.Location.Y},{c.IsAlive}"));

            ChangeHistory.Enqueue(updateSignature);

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
            var wasAlive = State == GridState.Alive;

            State = newState;

            if (wasAlive)
            {
                GameStopwatch.Stop();
                GridPrinter.PrintGameSummary(this);
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
            var proposedDelay = IterationDelayMs + adjustMs;

            IterationDelayMs = proposedDelay switch
            {
                <= ushort.MinValue => ushort.MinValue,
                >= ushort.MaxValue => ushort.MaxValue,
                _ => (ushort) proposedDelay
            };
        }
    }
}