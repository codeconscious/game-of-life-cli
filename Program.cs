namespace GameOfLife
{
    internal static class Program
    {
        private const string Instructions =
            "Run the Game of Life by supplying the following arguments:\n" +
            "   - Number of rows .......... At least 3, or else -1 to fit the console height\n" +
            "   - Number of columns ....... At least 3, or else -1 to fit the console width\n" +
            "   - Initial density (%) ..... 1 to 99 (inclusive, digits only), or else -1 for random\n" +
            "   - (Optional) Delay between iterations in milliseconds (Default is 50)\n" +
            "Alternatively, supply only \"--default\" to use the default settings (in which all values are -1).";

        private static void Main(string[] args)
        {
#if DEBUG
            WriteLine("Debugging");
#endif

            Settings settings;

            if (args.Length == 1 && args[0] == "--default")
            {
                WriteLine("Using default settings.");
                settings = new Settings(new[] { "-1", "-1", "-1" });
            }
            else // Create settings from the individual args.
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
        /// Generate the grid and then continuously updates until it enters a non-living state.
        /// </summary>
        /// <param name="settings"></param>
        private static void RunGame(Settings settings)
        {
            var gameStopwatch = new Stopwatch();
            gameStopwatch.Start();

            uint iteration = 1;

            Write("Preparing... ");
            Grid grid = new(settings);
            WriteLine($"done in " + gameStopwatch.Elapsed.TotalMilliseconds.ToString("#,##0") + "ms");

            grid.Print();

            Thread.Sleep(settings.IterationDelay);

            var outputRow = grid.RowCount + 1;
            var iterationStopwatch = new Stopwatch();

            // Process and print subsequent updates until an end state is reached.
            do
            {
                // Abort if the user pressed a key.
                if (Console.KeyAvailable)
                {
                    grid.Abort();
                    break;
                }

                iteration++;
                iterationStopwatch.Restart();

                var cellsToUpdate = grid.GetUpdatesForNextIteration();
                grid.UpdateAndCheckChangeHistory(cellsToUpdate);
                grid.PrintUpdates(cellsToUpdate);
                Thread.Sleep(settings.IterationDelay);

                PrintIterationSummary(iteration, iterationStopwatch.Elapsed, outputRow);
            }
            while (grid.Status == GridStatus.Alive);

            PrintGameResults(grid.Status, iteration, gameStopwatch.Elapsed, outputRow);
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
                GridStatus.Looping => "Infinite pattern loop reached",
                GridStatus.Stagnated => "Stagnated",
                GridStatus.Aborted => "Aborted",
                _ => "Unexpectedly finished"
            };

            ForegroundColor = finalStatus switch
            {
                GridStatus.Dead => ConsoleColor.DarkRed,
                GridStatus.Looping => ConsoleColor.Cyan,
                GridStatus.Stagnated => ConsoleColor.Blue,
                GridStatus.Aborted => ConsoleColor.DarkRed,
                _ => ConsoleColor.Red
            };

            SetCursorPosition(0, outputRow + 1);

            Write($"{statusStatement} after {iterations:#,##0} iterations in " +
                  $"{duration.TotalSeconds:#,##0.##}s " +
                  $"({iterations / duration.TotalSeconds:#,##0.##} iterations/s).");

            ResetColor();
        }
    }
}
