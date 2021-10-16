using GameOfLife.Game;
using System.IO;

namespace GameOfLife
{
    internal static class Program
    {
        private const string Instructions =
            "Run Conway's Game of Life in your terminal!\n" +
            "Supply the following arguments:\n" +
            "   - High-res mode ........... Enter 0 for standard resolution or 1 for high resolution" +
            "   - Number of columns ....... At least 3, or else -1 to fit the console width\n" +
            "   - Number of rows .......... At least 3, or else -1 to fit the console height\n" +
            "   - Initial density (%) ..... 1 to 99 (inclusive, digits only), or else -1 for random\n" +
            "   - (Optional) Delay between iterations in milliseconds (Otherwise, defaults to 50)\n" +
            "Alternatively, supply only \"--default\" to use the default settings (in which all values are -1).\n" +
            "During the simulation, you can press the left and right arrow keys to adjust the iteration speed.";

        private static void Main(string[] args)
        {
            // This is particularly necessary for high-res mode on Windows.
            OutputEncoding = System.Text.Encoding.UTF8;

            IPrinter printer = new ConsolePrinter();

            IGridSettings gameSettings;

            const string settingsFile = "custom.gol";

            if (args.Length == 0)
            {
                if (File.Exists(settingsFile))
                {
                    printer.PrintLine("Parsing custom settings...");

                    var settingsService = new SettingsService();
                    var settingsDto = settingsService.GetFromFile(settingsFile);

                    if (settingsDto == null)
                    {
                        printer.PrintLine($"Could not parse settings file \"{settingsFile}\".");
                        return; // TODO: Use default settings instead.
                    }

                    gameSettings = new Settings(settingsDto, printer);
                }
                else
                {
                    printer.PrintLine("Using default settings...");

                    var defaultDto = new SettingsDto(false, -1, -1, -1, 0);

                    gameSettings = new Settings(defaultDto, printer);
                }
            }
            else if (args.Length == 1)
            {
                if (args[0] == "--help" || args[0] == "-h")
                {
                    printer.PrintLine(Instructions);
                    return;
                }

                if (args[0] == "--create-settings" || args[0] == "-c")
                {
                    var settingsService = new SettingsService();
                    var settingsDto = settingsService.CreateSettingsFromUserInput();
                    settingsService.SaveToFile(settingsDto, settingsFile, printer);
                    return;
                }

                ForegroundColor = ConsoleColor.Red; // TODO: Add to the PrintLine method parameters.
                printer.PrintLine("Unrecognized command.");
                ForegroundColor = default;
                printer.PrintLine(Instructions);
                return;
            }
            else // More than 1 argument
            {
                ForegroundColor = ConsoleColor.Red; // TODO: Add to the PrintLine method parameters.
                printer.PrintLine("Too many arguments were entered.");
                ForegroundColor = default;
                printer.PrintLine(Instructions);
                return;
            }

            // Create the grid and run the game.
            try
            {
                CursorVisible = false;
                StartGame(gameSettings, printer);
            }
            catch (Exception ex)
            {
                ForegroundColor = ConsoleColor.Red;
                WriteLine(ex.Message);
                ForegroundColor = ConsoleColor.Yellow;
                WriteLine(ex.StackTrace);
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
        private static void StartGame(IGridSettings settings, IPrinter printer)
        {
            var iterationStopwatch = new Stopwatch();
            iterationStopwatch.Start();

            Write("Preparing... ");
            Grid grid = new(settings, printer);
            WriteLine("done in " + grid.GameStopwatch.Elapsed.TotalMilliseconds.ToString("#,##0") + "ms");

            printer.PrintEntire(grid, shouldClear: true);
            Thread.Sleep(settings.InitialIterationDelayMs);

            printer.PrintIterationSummary(grid, iterationStopwatch.Elapsed);

            // Process and print subsequent updates until an end state is reached.
            do
            {
                // Handle user key presses
                if (Console.KeyAvailable)
                {
                    var key = Console.ReadKey(true).Key;

                    if (key == ConsoleKey.LeftArrow) // Make slower
                    {
                        grid.AdjustIterationDelayBy(50);
                    }
                    else if (key == ConsoleKey.RightArrow) // Make faster
                    {
                        grid.AdjustIterationDelayBy(-50);
                    }
                    else // Any other key
                    {
                        grid.AbortGame();
                        break;
                    }
                }

                iterationStopwatch.Restart();

                grid.Iterate();
                Thread.Sleep(grid.IterationDelayMs);

                if (grid.State == GridState.Alive)
                    printer.PrintIterationSummary(grid, iterationStopwatch.Elapsed);
            }
            while (grid.State == GridState.Alive || grid.State == GridState.Looping);

            // Clear the line, then return to its start.
            // (This might not work when debugging since WindowWidth might equal 0.)
            SetCursorPosition(0, grid.OutputRow + 1);
            printer.ClearCurrentLine();

            // Place the cursor after the program output.
            SetCursorPosition(0, grid.OutputRow);
        }
    }
}
