namespace GameOfLife
{
    public static class ExtensionMethods
    {
        #region Grid-related

        /// <summary>
        /// Outputs the entire grid to the console. Intended to be used at game start.
        /// </summary>
        /// <param name="grid"></param>
        /// <param name="shouldClear">Specifies whether the screen be cleared first.</param>
        public static void PrintEntire(this Grid grid, bool shouldClear)
        {
            ArgumentNullException.ThrowIfNull(grid);

            if (shouldClear)
                Clear();

            ForegroundColor = GridStateColors.GameStateColors[grid.State];

            for (var y = 0; y < grid.Height; y++)
            {
                for (var x = 0; x < grid.Width; x++)
                {
                    SetCursorPosition(x, y);
                    Write(grid.GridChars[grid.CellGrid[x, y].IsAlive]);
                }
            }
        }

        /// <summary>
        /// Outputs only updated cells for the current iteration.
        /// </summary>
        /// <param name="cellsForUpdate"></param>
        public static void PrintUpdates(this Grid grid, List<Cell> cellsForUpdate)
        {
            ArgumentNullException.ThrowIfNull(grid);

            ForegroundColor = GridStateColors.GameStateColors[grid.State];

            try
            {
                foreach (var cell in cellsForUpdate)
                {
                    SetCursorPosition(cell.Location.X, cell.Location.Y);
                    Write(grid.GridChars[cell.IsAlive]);
                }
            }
            catch (Exception)
            {
                Clear();
                ResetColor();
                throw;
            }
        }

        /// <summary>
        /// Outputs a single-line summary of the current iteration.
        /// </summary>
        /// <param name="grid"></param>
        /// <param name="duration">An optional iteration time duration.</param>
        public static void PrintIterationSummary(this Grid grid, TimeSpan? duration = null)
        {
            ArgumentNullException.ThrowIfNull(grid);

            ResetColor();

            SetCursorPosition(0, grid.OutputRow);

            var durationText = duration == null
                ? ""
                : $"({duration.Value.TotalMilliseconds:#,##0}ms)";

            Write($"<Press any key to quit>  Iteration {grid.CurrentIteration:#,##0} {durationText}  ");
        }

        /// <summary>
        /// Outputs a single-line summary of the entire game.
        /// Intended to be used when there's an entire-grid state change.
        /// </summary>
        /// <param name="grid"></param>
        public static void PrintGameSummary(this Grid grid)
        {
            ArgumentNullException.ThrowIfNull(grid);

            var stateClause = grid.State switch
            {
                GridState.Extinct => "Extinction",
                GridState.Looping => "Endless loop",
                GridState.Stagnated => "Stagnation",
                GridState.Aborted => "Aborted",
                _ => "Unknown" // Should never be reached
            };

            var iterationClause = $"{grid.CurrentIteration:#,##0} iterations";

            var seconds = grid.GameStopwatch.Elapsed.TotalSeconds;

            var secondsClause = $"{seconds:#,##0.###} sec";

            var iterationsPerSecondClause =
                (grid.CurrentIteration / seconds).ToString("#,##0.###") +
                " iterations/sec";

            var gridClause = $"{grid.Width} × {grid.Height}";

            ForegroundColor = GridStateColors.GameStateColors[grid.State];

            // Move to the output row, then clear it.
            SetCursorPosition(0, grid.OutputRow);
            Utility.ClearCurrentLine();

            // Ex.: Endless loop | 813 iterations | 4.181 sec | 194.431 iterations/sec | 44 × 178 | 7,832 cells
            Write($"{stateClause} | {iterationClause} | {secondsClause} | " +
                  $"{iterationsPerSecondClause} | {gridClause} | {grid.TotalCells:#,##0} cells");

            // If we're looping, then show how to exit.
            if (grid.State == GridState.Looping)
            {
                ForegroundColor = ConsoleColor.Gray;
                SetCursorPosition(0, grid.OutputRow + 1);
                WriteLine("Press any key to quit.");
            }
        }

        #endregion

        /// <summary>
        /// Determines if a Point is valid -- i.e., within established bounds.
        /// Point coordinates cannot be negative, nor can they exceed the specified limits.
        /// </summary>
        public static bool IsValid(this Point point, int maxWidth, int maxHeight)
        {
            // TODO: Prevent null values in the first place.
            if (maxWidth < 0)
                throw new ArgumentOutOfRangeException(nameof(maxWidth));
            if (maxHeight < 0)
                throw new ArgumentOutOfRangeException(nameof(maxHeight));

            return
                point.X >= 0 &&
                point.Y >= 0 &&
                point.X < maxWidth &&
                point.Y < maxHeight;
        }
    }
}