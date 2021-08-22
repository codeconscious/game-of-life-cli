namespace GameOfLife
{
    /// <summary>
    /// The settings used to create a grid and elsewhere.
    /// </summary>
    public readonly record struct GridSettings
    {
        private const byte MinimumRowsColumns = 3;
        public byte RowCount { get; init; }
        public byte ColumnCount { get; init; }
        public byte LifeProbability { get; init; }
        public ushort IterationDelay { get; init; }

        public GridSettings(string[] args)
        {
            if (args.Length != 3 && args.Length != 4)
                throw new ArgumentException("An unsupported number of arguments was passed in.");

            // Verify the row arg
            if (!byte.TryParse(args[0], out var rowCount) || rowCount < MinimumRowsColumns)
                throw new ArgumentOutOfRangeException(nameof(rowCount));

            // Verify the column arg
            if (!byte.TryParse(args[1], out var columnCount) || columnCount < MinimumRowsColumns)
                throw new ArgumentOutOfRangeException(nameof(columnCount));

            // Verify the percentage arg
            if (!byte.TryParse(args[2], out var lifeProbability) || lifeProbability > 100)
                throw new ArgumentOutOfRangeException(nameof(lifeProbability));

            // Mandatory arg assignments
            RowCount = rowCount;
            ColumnCount = columnCount;
            LifeProbability = lifeProbability;

            // Verify the optional iteration delay arg, if specified; otherwise, set the default.
            if (args.Length == 4)
            {
                if (!ushort.TryParse(args[3], out var iterationDelay))
                    throw new ArgumentOutOfRangeException(nameof(iterationDelay));

                IterationDelay = iterationDelay;
            }
            else
            {
                IterationDelay = 50; // Milliseconds
            }
        }
    }
}