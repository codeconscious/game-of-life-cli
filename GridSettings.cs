namespace GameOfLife
{
    /// <summary>
    /// The settings used to create a grid.
    /// </summary>
    public readonly record struct GridSettings
    {
        private const byte MinimumRowsColumns = 3;
        public byte RowCount { get; init; }
        public byte ColumnCount { get; init; }
        public byte LifeProbability { get; init; }

        public GridSettings(string[] args)
        {
            if (args.Length != 3)
                throw new ArgumentException("The correct number of arguments was not passed in.");

            // Verify the row arg is valid.
            if (!byte.TryParse(args[0], out var rowCount) || rowCount < MinimumRowsColumns)
                throw new ArgumentOutOfRangeException(nameof(rowCount));

            // Verify the column arg is valid.
            if (!byte.TryParse(args[1], out var columnCount) || columnCount < MinimumRowsColumns)
                throw new ArgumentOutOfRangeException(nameof(columnCount));

            // Verify the percentage arg is valid.
            if (!byte.TryParse(args[2], out var lifeProbability) || lifeProbability > 100)
                throw new ArgumentOutOfRangeException(nameof(lifeProbability));

            RowCount = rowCount;
            ColumnCount = columnCount;
            LifeProbability = lifeProbability;
        }
    }
}