namespace GameOfLife
{
    /// <summary>
    /// The settings used to create a grid and elsewhere.
    /// </summary>
    public record Settings
    {
        private const byte MinimumRowsColumns = 3;
        public int RowCount { get; init; }
        public int ColumnCount { get; init; }
        public int InitialPopulationRatio { get; init; }
        public ushort IterationDelay { get; init; }
        public const byte MaximumRandomPopulationRatio = 70;

        public Settings(string[] args)
        {
            if (args.Length != 3 && args.Length != 4)
                throw new ArgumentException("An unsupported number of arguments was passed in.");

            // Verify the row arg
            if (args[0] == "-1")
            {
                // Leave room for output and post-completion command line prompt.
                const int bottomMargin = 5;

                RowCount = Console.WindowHeight - bottomMargin;
            }
            else
            {
                if (!ushort.TryParse(args[0], out var rowCount) || rowCount < MinimumRowsColumns)
                    throw new ArgumentOutOfRangeException(nameof(rowCount));

                RowCount = rowCount;
            }

            // Verify the column arg
            if (args[1] == "-1")
            {
                ColumnCount = Console.WindowWidth;
            }
            else
            {
                if (!ushort.TryParse(args[1], out var columnCount) || columnCount < MinimumRowsColumns)
                    throw new ArgumentOutOfRangeException(nameof(columnCount));

                ColumnCount = columnCount;
            }

            // Verify the population ratio arg
            if (args[2] == "-1")
            {
                InitialPopulationRatio = (byte) new Random().Next(MaximumRandomPopulationRatio);
            }
            else
            {
                if (!byte.TryParse(args[2], out var lifeProbability) || lifeProbability > 100)
                    throw new ArgumentOutOfRangeException(nameof(lifeProbability));

                InitialPopulationRatio = lifeProbability;
            }

            // Verify the optional iteration delay arg, or if it's missing, set a default.
            if (args.Length == 4)
            {
                if (!ushort.TryParse(args[3], out var iterationDelay))
                    throw new ArgumentOutOfRangeException(nameof(iterationDelay));

                IterationDelay = iterationDelay;
            }
            else
            {
                IterationDelay = 50; // Default value in milliseconds
            }

            WriteLine($"Grid:            {RowCount} rows x {ColumnCount} columns ({RowCount * ColumnCount:#,##0} cells)");
            WriteLine($"Population:      {InitialPopulationRatio}%");
            WriteLine($"Iteration delay: {IterationDelay}ms");
        }
    }
}