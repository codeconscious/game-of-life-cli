namespace GameOfLife
{
    public static class Printer
    {
        /// <summary>
        /// Outputs a summary of the current iteration.
        /// </summary>
        /// <param name="grid"></param>
        /// <param name="duration">An optional iteration time duration.</param>
        public static void PrintIterationSummary(Grid grid, TimeSpan? duration = null)
        {
            SetCursorPosition(0, grid.OutputRow);

            var durationText = duration == null
                ? ""
                : $"({duration.Value.TotalMilliseconds:#,##0}ms)";

            Write($"<Press any key to quit>  Iteration {grid.CurrentIteration:#,##0} {durationText}  ");
        }

        /// <summary>
        /// Outputs a summary of the entire game. Intended to be used at grid status updates.
        /// </summary>
        /// <param name="grid"></param>
        public static void PrintGameResults(Grid grid)
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