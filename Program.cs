namespace GameOfLife
{
    internal static class Program
    {
        private const string Instructions =
            "Pass in three numbers: row count (3-128), column count (3-128), and activation percentage (1-100).";

        private static void Main(string[] args)
        {
            // Prepare the grid settings.
            GridSettings settings;
            if (args.Length == 1 && args[0] == "--debug")
            {
                WriteLine("Proceeding with default settings.");
                settings = new GridSettings(new[] {"30", "150", "50"});
            }
            else
            {
                try
                {
                    settings = new GridSettings(args);
                }
                catch (Exception ex)
                {
                    ForegroundColor = ConsoleColor.Yellow;
                    WriteLine(ex.Message);
                    WriteLine(Instructions);
                    ResetColor();
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

        private static void RunGame(GridSettings settings)
        {
            nuint iteration = 1;

            var startTime = DateTime.Now;

            Grid grid = new(settings);
            grid.Print();

            var duration = DateTime.Now - startTime;
            var outputRow = grid.RowCount + 1;

            PrintStatusLine(iteration, duration, outputRow);

            // Process and print grid updates
            do
            {
                if (Console.KeyAvailable)
                    break;

                iteration++;
                startTime = DateTime.Now;

                var updates = grid.GetUpdatesForNextIteration();
                grid.PrintUpdates(updates, 500); // TODO: Make the delay a setting.

                duration = DateTime.Now - startTime;

                PrintStatusLine(iteration, duration, outputRow);
            }
            while (!grid.IsStale && grid.IsAlive); // TODO: Need to check for endless loops too.
        }

        private static void PrintStatusLine(nuint iteration, TimeSpan duration, int outputRow)
        {
            SetCursorPosition(0, outputRow);
            WriteLine($"Iteration {iteration:#,##0} ({duration.TotalMilliseconds:#,##0}ms)  ");
            WriteLine("Please any key to quit."); // TODO: Move elsewhere
        }
    }
}
