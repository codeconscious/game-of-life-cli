namespace GameOfLife
{
    public readonly record struct BoardSettings
    {
        private const byte MinRowsOrColumns = 3;

        public byte RowCount { get; init; }
        public byte ColumnCount { get; init; }
        public byte Probability { get; init; }

        public BoardSettings(string[] args)
        {
            // Check arguments.
            if (args.Length != 3)
                throw new ArgumentException("The correct number of arguments was not passed in.");

            // Verify the row arg is valid.
            if (!byte.TryParse(args[0], out var rowCount) || rowCount < MinRowsOrColumns)
                throw new ArgumentOutOfRangeException("The row argument is out of range.");

            // Verify the column arg is valid.
            if (!byte.TryParse(args[1], out var columnCount) || columnCount < MinRowsOrColumns)
                throw new ArgumentOutOfRangeException("The column argument is out of range.");

            // Verify the percentage arg is valid.
            if (!byte.TryParse(args[2], out var probability) || probability > 100)
                throw new ArgumentOutOfRangeException("The probability argument is out of range.");

            RowCount = rowCount;
            ColumnCount = columnCount;
            Probability = probability;
        }
    }
}