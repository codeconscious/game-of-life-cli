namespace GameOfLife
{
    internal static class Program
    {
        private const string Instructions =
            "Run the Game of Life by supplying the following arguments:\n" +
            "   - Number of rows .......... At least 3, or else -1 to fit the console height\n" +
            "   - Number of columns ....... At least 3, or else -1 to fit the console width\n" +
            "   - Initial density (%) ..... 1 to 99 (inclusive, digits only), or else -1 for random\n" +
            "   - (Optional) Delay between iterations in milliseconds (Otherwise, defaults to 50)\n" +
            "Alternatively, supply only \"--default\" to use the default settings (in which all values are -1).\n" +
            "During the simulation, you can press the left and right arrow keys to adjust the iteration speed.";

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
                StartGame(settings);
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
        private static void StartGame(Settings settings)
        {
            var iterationStopwatch = new Stopwatch();
            iterationStopwatch.Start();

            Write("Preparing... ");
            Grid grid = new(settings);
            WriteLine("done in " + grid.GameStopwatch.Elapsed.TotalMilliseconds.ToString("#,##0") + "ms");

            grid.Print();
            Thread.Sleep(settings.IterationDelayMs);

            grid.PrintIterationSummary(iterationStopwatch.Elapsed);

            // Process and print subsequent updates until an end state is reached.
            do
            {
                // Handle user key presses
                if (Console.KeyAvailable)
                {
                    var key = Console.ReadKey(true).Key;

                    if (key == ConsoleKey.LeftArrow) // Make slower
                    {
                        settings.AdjustIterationDelayBy(50);
                    }
                    else if (key == ConsoleKey.RightArrow) // Make faster
                    {
                        settings.AdjustIterationDelayBy(-50);
                    }
                    else // Any other key
                    {
                        grid.AbortGame();
                        break;
                    }
                }

                iterationStopwatch.Restart();

                grid.Iterate();
                Thread.Sleep(settings.IterationDelayMs);

                grid.PrintIterationSummary(iterationStopwatch.Elapsed);
            }
            while (grid.Status == GridStatus.Alive || grid.Status == GridStatus.Looping);

            grid.PrintGameStatus();

            // Place the cursor after the program output.
            SetCursorPosition(0, grid.OutputRow + 1);
        }
    }
}
