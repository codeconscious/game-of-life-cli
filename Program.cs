namespace GameOfLife
{
    internal static class Program
    {
        private const string Instructions =
            "Pass in three numbers: row count (3-128), column count (3-128), and activation percentage (1-100).";

        private static void Main(string[] args)
        {
            GridSettings settings;
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

            Grid grid = new(settings);
            grid.Print();

            var iteration = 1;
            const string iterationLabel = "Iteration:";
            var iterationRow = grid.RowCount + 1;
            SetCursorPosition(0, iterationRow);
            Write(iterationLabel + iteration);

            // TODO: Refactor for performance (Don't create new boards each time)
            do
            {
                var startTime = DateTime.Now;

                grid = Utilities.GetDescendantGrid(grid);
                grid.Print(0);

                var endTime = DateTime.Now - startTime;

                // Print iteration
                SetCursorPosition(0, iterationRow);
                Write($"{iterationLabel} {++iteration} ({endTime.TotalMilliseconds}ms)");
            }
            while (grid.IsAlive); // TODO: Add an isStale property too
        }
    }
}
