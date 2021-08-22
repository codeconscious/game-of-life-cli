namespace GameOfLife
{
    internal static class Program
    {
        private const string Instructions =
            "Run the Game of Life by supplying the following arguments:\n" +
            "   - Number of rows .......... 3 to 256 (inclusive) or else -1 to fit the console height\n" +
            "   - Number of columns ....... 3 to 256 (inclusive) or else -1 to fit the console width\n" +
            "   - Initial density (%) ..... 1 to 99 (inclusive, digits only) or else -1 to set randomly\n" +
            "   - (Optional) Delay between iterations in milliseconds";

        private static void Main(string[] args)
        {
            // Prepare the grid settings.
            Settings settings;
            if (args.Length == 1 && args[0] == "--debug")
            {
                WriteLine("Proceeding with default settings.");
                settings = new Settings(new[] { "35", "150", "40" });
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
        /// Generate the grid and run endlessly or until the grid is dead or stale.
        /// </summary>
        /// <param name="settings"></param>
        private static void RunGame(Settings settings)
        {
            nuint iteration = 1;

            var startTime = DateTime.Now;

            Grid grid = new(settings);
            grid.Print();

            var duration = DateTime.Now - startTime;
            var outputRow = grid.RowCount + 1;

            PrintStatus(iteration, duration, outputRow);

            // Process and print grid updates
            do
            {
                if (Console.KeyAvailable)
                    break;

                iteration++;
                startTime = DateTime.Now;

                var cellsToUpdate = grid.GetUpdatesForNextIteration();

                grid.UpdateAndCheckChangeHistory(cellsToUpdate);

                grid.PrintUpdates(cellsToUpdate, settings.IterationDelay);

                duration = DateTime.Now - startTime;

                PrintStatus(iteration, duration, outputRow);
            }
            while (grid.Status == GridStatus.Alive);

            PrintResults(grid.Status, iteration, outputRow);
        }

        private static void PrintStatus(nuint iteration, TimeSpan duration, int outputRow)
        {
            SetCursorPosition(0, outputRow);
            WriteLine($"Iteration {iteration:#,##0} ({duration.TotalMilliseconds:#,##0}ms)  ");
            Write("Press any key to quit."); // TODO: Move elsewhere
        }

        private static void PrintResults(GridStatus finalStatus, nuint iterations, int outputRow)
        {
            var statusStatement = finalStatus switch
            {
                GridStatus.Dead => "All cells died",
                GridStatus.Looping => "Infinite looping was reached",
                GridStatus.Stagnated => "The grid stagnated",
                _ => "An unexpected state was reached"
            };

            SetCursorPosition(0, outputRow);

            WriteLine($"{statusStatement} after {iterations:#,##0} iterations.");
        }
    }
}
