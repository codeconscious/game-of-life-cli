namespace GameOfLife
{
    /// <summary>
    /// The settings used to create a grid and affect elements of the game elsewhere.
    /// </summary>
    public class Settings
    {
        private const byte MinimumRowsColumns = 3;
        public int RowCount { get; init; }
        public int ColumnCount { get; init; }
        public int InitialPopulationRatio { get; init; }

        /// <summary>
        /// The delay in milliseconds between two consecutive iterations (turns).
        /// </summary>
        /// <value></value>
        public ushort IterationDelayMs { get; private set; }

        /// <summary>
        /// The maximum population percentage allowed when setting it randomly.
        /// If it's too high, then the game will likely end very quickly.
        /// </summary>
        public const byte MaximumRandomPopulationRatio = 70;

        /// <summary>
        /// Constructor that is expected to take in arguments from the user.
        /// </summary>
        /// <param name="args"></param>
        public Settings(string[] args)
        {
            if (args.Length != 3 && args.Length != 4)
                throw new ArgumentException("An unsupported number of arguments was passed in.");

            // Verify the row arg
            if (args[0] == "-1")
            {
                // Leave room at the bottom of the screen for output (during and after the game).
                const int bottomMargin = 3;

                RowCount = Console.WindowHeight == 0 // This can occur when debugging.
                    ? 30 // Debugging value
                    : Console.WindowHeight - bottomMargin;
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
                ColumnCount = Console.WindowWidth == 0 // This can occur when debugging.
                    ? 200 // Debugging value
                    : Console.WindowWidth;
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
                InitialPopulationRatio = new Random().Next(MaximumRandomPopulationRatio);
            }
            else
            {
                if (!byte.TryParse(args[2], out var lifeProbability) ||
                    lifeProbability > 100 || lifeProbability < 1)
                {
                    throw new ArgumentOutOfRangeException(nameof(lifeProbability));
                }

                InitialPopulationRatio = lifeProbability;
            }

            // Verify the optional iteration delay arg, or if it's missing, set a default.
            if (args.Length == 4)
            {
                if (!ushort.TryParse(args[3], out var iterationDelay))
                    throw new ArgumentOutOfRangeException(nameof(iterationDelay));

                IterationDelayMs = iterationDelay;
            }
            else
            {
                IterationDelayMs = 50; // Default value in milliseconds
            }

            WriteLine($"Grid:            {RowCount} rows x {ColumnCount} columns ({RowCount * ColumnCount:#,##0} cells)");
            WriteLine($"Population:      {InitialPopulationRatio}%");
            WriteLine($"Iteration delay: {IterationDelayMs}ms");
        }

        /// <summary>
        /// Adjust the iteration delay within the extent of valid values.
        /// </summary>
        /// <param name="adjustMs">The number of milliseconds (negative or positive) to adjust by.</param>
        public void AdjustIterationDelayBy(short adjustMs)
        {
            var proposedDelay = IterationDelayMs + adjustMs;

            if (proposedDelay >= ushort.MinValue &&
                proposedDelay < ushort.MaxValue)
            {
                IterationDelayMs = (ushort) proposedDelay;
            }
        }
    }
}