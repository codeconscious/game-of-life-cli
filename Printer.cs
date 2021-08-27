namespace GameOfLife
{
    public static class GridPrintExtensionMethods
    {
        /// <summary>
        /// Outputs the entire grid to the console. Intended to be used at game start.
        /// </summary>
        public static void Print(this Grid grid)
        {
            Clear();

            for (var row = 0; row < grid.RowCount; row++)
            {
                for (var column = 0; column < grid.ColumnCount; column++)
                {
                    var isAlive = grid.CellGrid[row, column].IsAlive;

                    ForegroundColor = isAlive ? ConsoleColor.Green
                                              : ConsoleColor.DarkGray;

                    SetCursorPosition(column, row);

                    Write(grid.GridChars[isAlive]);
                }
            }

            ResetColor();
        }

        /// <summary>
        /// Outputs only updated cells for the current iteration.
        /// </summary>
        /// <param name="cellsForUpdate"></param>
        public static void PrintUpdates(this Grid grid, List<Cell> cellsForUpdate)
        {
            try
            {
                foreach (var cell in cellsForUpdate)
                {
                    ForegroundColor = cell.IsAlive ? ConsoleColor.Green
                                                   : ConsoleColor.DarkGray;

                    SetCursorPosition(cell.Coordinates.Column, cell.Coordinates.Row);

                    Write(grid.GridChars[cell.IsAlive]);
                }

                ResetColor();
            }
            catch (Exception ex)
            {
                Clear();
                ResetColor();
                WriteLine(ex.Message);
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
            SetCursorPosition(0, grid.OutputRow);

            var durationText = duration == null
                ? ""
                : $"({duration.Value.TotalMilliseconds:#,##0}ms)";

            Write($"<Press any key to quit>  Iteration {grid.CurrentIteration:#,##0} {durationText}  ");
        }

        /// <summary>
        /// Outputs a single-line summary of the entire game.
        /// Intended to be used for entire-grid status changes.
        /// </summary>
        /// <param name="grid"></param>
        public static void PrintGameStatus(this Grid grid)
        {
            var statusStatement = grid.Status switch
            {
                GridStatus.Dead => "All cells died",
                GridStatus.Looping => "Infinite loop reached",
                GridStatus.Stagnated => "Stagnated",
                GridStatus.Aborted => "Aborted",
                _ => "Unexpectedly finished"
            };

            ForegroundColor = grid.Status switch
            {
                GridStatus.Dead => ConsoleColor.DarkRed,
                GridStatus.Looping => ConsoleColor.Cyan,
                GridStatus.Stagnated => ConsoleColor.Blue,
                GridStatus.Aborted => ConsoleColor.DarkRed,
                _ => ConsoleColor.Red
            };

            SetCursorPosition(0, grid.OutputRow + 1);

            // Ex.: Infinite loop reached after 3,589 iterations in 277.96s (12.91 iterations/s).
            Write($"{statusStatement} after {grid.CurrentIteration:#,##0} iterations in " +
                  $"{grid.Stopwatch.Elapsed.TotalSeconds:#,##0.##}s " +
                  $"({grid.CurrentIteration / grid.Stopwatch.Elapsed.TotalSeconds:#,##0.##} iterations/s).");

            ResetColor();
        }
    }
}