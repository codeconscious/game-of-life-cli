namespace GameOfLife
{
    public record struct CoordinatePair
    {
        public int Row { get; init; }
        public int Column { get; init; }

        public CoordinatePair(int row, int column)
        {
            Row = row;
            Column = column;
        }

        /// <summary>
        /// Determines whether the coordinate values for this pair are valid or not.
        /// </summary>
        /// <param name="totalRows"></param>
        /// <param name="totalColumns"></param>
        public bool IsValid(int totalRows, int totalColumns)
        {
            return
                this.Row >= 0 &&
                this.Column >= 0 &&
                this.Row < totalRows &&
                this.Column < totalColumns;
        }
    }
}