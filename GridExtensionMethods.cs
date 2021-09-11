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
            ResetColor();

            SetCursorPosition(0, grid.FirstOutputRow);

            var durationText = duration == null
                ? ""
                : $"({duration.Value.TotalMilliseconds:#,##0}ms)";

            Write($"<Press any key to quit>  Iteration {grid.IterationNumber:#,##0} {durationText}  ");
        }

        /// <summary>
        /// Outputs a single-line summary of the entire game.
        /// Intended to be used when there's an entire-grid state change.
        /// </summary>
        /// <param name="grid"></param>
        public static void PrintGameSummary(this Grid grid)
        {
            var stateStatement = grid.State switch
            {
                GridState.Dead => "Extinction occurred",
                GridState.Looping => "Endless loop reached",
                GridState.Stagnated => "Stagnated",
                GridState.Aborted => "Aborted",
                _ => "Unexpectedly finished"
            };

            var iterationStatement = grid.LastLivingIteration == null
                ? $"{grid.IterationNumber:#,##0} iterations"
                : $"{grid.IterationNumber:#,##0} iterations (alive for {grid.LastLivingIteration:#,##0})";

            ForegroundColor = GridStateColors.GameStateColors[grid.State];

            SetCursorPosition(0, grid.FirstOutputRow + 1);

            // Clear the line, then return to its start.
            // (This might not work when debugging since WindowWidth might equal 0.)
            Write(new string(' ', WindowWidth - 1) + "\r");

            // Ex.: Infinite loop reached after 3,589 iterations in 277.96s (12.91 iterations/s).
            Write($"{stateStatement} after {iterationStatement} in " +
                  $"{grid.GameStopwatch.Elapsed.TotalSeconds:#,##0.###}s " +
                  $"({grid.IterationNumber / grid.GameStopwatch.Elapsed.TotalSeconds:#,##0.###} iterations/s).");

            ResetColor();
        }
    }
}