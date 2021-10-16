using Spectre.Console;
using System.IO;

namespace GameOfLife
{
    internal static class Program
    {
        private const string SettingsFile = "settings.json";
        private const short IterationDelayAdjustmentMs = 50;

        private static void PrintInstructions()
        {
            var table = new Table();
            table.Border(TableBorder.Rounded).BorderColor(Spectre.Console.Color.Grey19);

            table.AddColumn(new TableColumn("[blue]Conway's Game of Life in your terminal![/]"));

            table.Columns[0].PadLeft(20).PadRight(20);

            table.AddRow("Simply run with no arguments to begin. " +
                $"If the file \"{SettingsFile}\" exists in the program's folder, " +
                "its settings will be parsed and used. " +
                "Otherwise, the program will use its own default settings.");

            table.AddEmptyRow();

            table.AddRow("Use [bold yellow]\"--save-settings\"[/] or [bold yellow]\"-s\"[/] to create and save a new settings file.");

            table.AddEmptyRow();

            table.AddRow("During the simulation, you can press the [bold yellow]left and right arrow keys[/] to " +
                $"adjust the simulation speed by {IterationDelayAdjustmentMs}ms.");

            table.AddEmptyRow();

            table.AddRow("Online at https://github.com/codeconscious/GameOfLife/");

            AnsiConsole.Write(table);
        }

        private static void Main(string[] args)
        {
            // This is particularly necessary for high-res mode on Windows.
            OutputEncoding = System.Text.Encoding.UTF8;
            ResetColor();

            IPrinter printer = new ConsolePrinter();

            IGridSettings gameSettings;

            const string settingsFile = "settings.json";

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
                if (args[0] == "--help" || args[0] == "-h" || args[0] == "-hh")
                {
                    PrintInstructions();
                    return;
                }

                if (args[0] == "--save-settings" || args[0] == "-s")
                {
                    var settingsService = new SettingsService();
                    var settingsDto = settingsService.CreateSettingsFromUserInput();
                    settingsService.SaveToFile(settingsDto, settingsFile, printer);
                    return;
                }

                ForegroundColor = ConsoleColor.Red; // TODO: Add to the PrintLine method parameters.
                printer.PrintLine("Unrecognized command.");
                ForegroundColor = default;
                PrintInstructions();
                return;
            }
            else // More than 1 argument
            {
                ForegroundColor = ConsoleColor.Red; // TODO: Add to the PrintLine method parameters.
                printer.PrintLine("Too many arguments were entered.");
                ForegroundColor = default;
                PrintInstructions();
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
            var grid = new Game.Grid(settings, printer);
            WriteLine("done in " + grid.GameStopwatch.Elapsed.TotalMilliseconds.ToString("#,##0") + "ms");

            printer.PrintEntire(grid, shouldClear: true);
            Thread.Sleep(settings.IterationDelayMs);

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
                        grid.AdjustIterationDelayBy(IterationDelayAdjustmentMs);
                    }
                    else if (key == ConsoleKey.RightArrow) // Make faster
                    {
                        grid.AdjustIterationDelayBy(IterationDelayAdjustmentMs * -1);
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
