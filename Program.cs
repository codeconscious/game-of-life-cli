namespace GameOfLife
{
    internal static class Program
    {
        private const string Instructions =
            "Run the Game of Life by supplying the following arguments:\n" +
            "   - Number of rows .......... 3 to 256 (inclusive) or else -1 to fit the console height\n" +
            "   - Number of columns ....... 3 to 256 (inclusive) or else -1 to fit the console width\n" +
            "   - Initial density (%) ..... 1 to 99 (inclusive, digits only) or else -1 to set randomly\n" +
            "   - (Optional) Delay between iterations in milliseconds (Default is 50)\n" +
            "Alternatively, supply only \"--default\" to use the default settings.";

        private static void Main(string[] args)
        {
            // Prepare the settings from the given args.
            Settings settings;

            if (args.Length == 1 && args[0] == "--default")
            {
                WriteLine("Using default settings.");
                settings = new Settings(new[] { "-1", "-1", "-1" });
            }
            else
            {
                try
                {
                    settings = new Settings(args);
                }
                catch (Exception ex)
                {
                    ForegroundColor = ConsoleColor.Yellow;
                    WriteLine(ex.Message);
                    ResetColor();

                    WriteLine(Instructions);
                    return;
                }
            }

            // Create the grid and run the game.
            try
            {
                CursorVisible = false;
                RunGame(settings);
            }
            catch (Exception ex)
            {
                WriteLine(ex.Message);
            }
            finally
            {
                CursorVisible = true;
                ResetColor();
                WriteLine();
            }
        }

        /// <summary>
        /// Generate the grid and run endlessly or until the grid enters a non-alive state.
        /// </summary>
        /// <param name="settings"></param>
        private static void RunGame(Settings settings)
        {
            nuint iteration = 1;

            var gameStartTime = DateTime.Now;
            var iterationStartTime = gameStartTime;

            Grid grid = new(settings);
            grid.Print();

            var duration = DateTime.Now - iterationStartTime;
            var outputRow = grid.RowCount + 1;

            PrintIterationSummary(iteration, duration, outputRow);

            // Process and print grid updates
            do
            {
                // Stop if the user pressed a key.
                if (Console.KeyAvailable)
                    break;

                iteration++;
                iterationStartTime = DateTime.Now;

                var cellsToUpdate = grid.GetUpdatesForNextIteration();

                grid.UpdateAndCheckChangeHistory(cellsToUpdate);

                grid.PrintUpdates(cellsToUpdate, settings.IterationDelay);

                duration = DateTime.Now - iterationStartTime;

                PrintIterationSummary(iteration, duration, outputRow);
            }
            while (grid.Status == GridStatus.Alive);

            var gameDuration = DateTime.Now - gameStartTime;

            PrintGameResults(grid.Status, iteration, gameDuration, outputRow);
        }

        private static void PrintIterationSummary(nuint iteration, TimeSpan duration, int outputRow)
        {
            SetCursorPosition(0, outputRow);
            WriteLine($"Iteration {iteration:#,##0} ({duration.TotalMilliseconds:#,##0}ms)  ");
            Write("Press any key to quit.");
        }

        private static void PrintGameResults(GridStatus finalStatus, nuint iterations,
                                             TimeSpan duration, int outputRow)
        {
            var statusStatement = finalStatus switch
            {
                GridStatus.Dead => "All cells died",
                GridStatus.Looping => "Infinite looping was reached",
                GridStatus.Stagnated => "The grid stagnated",
                _ => "An unexpected state was reached"
            };

            SetCursorPosition(0, outputRow + 1);

            Write($"{statusStatement} after {iterations:#,##0} iterations in {duration.TotalSeconds:#,##0.##}s.");
        }
    }
}
