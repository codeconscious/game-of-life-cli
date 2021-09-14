namespace GameOfLife
{
    public static class GridExtensionMethods
    {
        /// <summary>
        /// Outputs the entire grid to the console. Intended to be used at game start.
        /// </summary>
        /// <param name="grid"></param>
        /// <param name="shouldClear">Should the screen be cleared first?</param>
        public static void PrintEntire(this Grid grid, bool shouldClear)
        {
            ArgumentNullException.ThrowIfNull(grid);

            if (shouldClear)
                Clear();

            ForegroundColor = GridStateColors.GameStateColors[grid.State];

            for (var row = 0; row < grid.RowCount; row++)
            {
                for (var column = 0; column < grid.ColumnCount; column++)
                {
                    SetCursorPosition(column, row);
                    Write(grid.GridChars[grid.CellGrid[row, column].IsAlive]);
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
                    SetCursorPosition(cell.Coordinates.Column, cell.Coordinates.Row);
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

            var iterationClause = grid.LastLivingIteration == null
                ? $"{grid.CurrentIteration:#,##0} iterations"
                : $"{grid.CurrentIteration:#,##0} iterations (alive for {grid.LastLivingIteration:#,##0})";

            var seconds = grid.GameStopwatch.Elapsed.TotalSeconds;

            var secondsClause = $"{seconds:#,##0.###} sec";

            var iterationsPerSecondClause = ((grid.LastLivingIteration ?? grid.CurrentIteration) /
                                            seconds).ToString("#,##0.###") + " iterations/sec";

            var gridClause = $"{grid.RowCount} × {grid.ColumnCount}";

            ForegroundColor = GridStateColors.GameStateColors[grid.State];

            SetCursorPosition(0, grid.OutputRow);

            // Clear the line, then return to its start.
            // (This might not work when debugging since WindowWidth might equal 0.)
            Utility.ClearCurrentLine();

            // Ex.: Endless loop | 813 iterations | 4.181 sec | 194.431 iterations/sec | 44 × 178 | 7,832 cells
            Write($"{stateClause} | {iterationClause} | {secondsClause} | " +
                  $"{iterationsPerSecondClause} | {gridClause} | {grid.TotalCells:#,##0} cells");

            // If we're looping, then show how to exit.
            if (grid.State == GridState.Looping)
            {
                ForegroundColor = ConsoleColor.White;
                SetCursorPosition(0, grid.OutputRow + 1);
                WriteLine("Press any key to quit.");
            }
        }
    }
}