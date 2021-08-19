namespace GameOfLife
{
    internal static class Program
    {
        private const byte MinRowsOrColumns = 3;

        private const string Instructions =
            "Pass in three numbers: row count (0-128), column count (0-128) and percentage (1-100).";

        private static void Main(string[] args)
        {
            // Check arguments.
            if (args.Length != 3)
            {
                WriteLine(Instructions);
                return;
            }

            // Verify the row arg is valid.
            if (!byte.TryParse(args[0], out var rowCount) || rowCount < MinRowsOrColumns)
            {
                WriteLine("The first argument is not a number or is out of range.");
                WriteLine(Instructions);
                return;
            }

            // Verify the column arg is valid.
            if (!byte.TryParse(args[1], out var columnCount) || columnCount < MinRowsOrColumns)
            {
                WriteLine("The second argument is not a number or is out of range.");
                WriteLine(Instructions);
                return;
            }

            // Verify the percentage arg is valid.
            if (!byte.TryParse(args[2], out var probability))
            {
                WriteLine("The third argument is not a number or is out of range.");
                WriteLine(Instructions);
                return;
            }

            var board = new Board(rowCount, columnCount, probability);
            board.Print();
        }

        // static Board GetUpdatedBoard(Board board)
        // {

        // }

        // static byte CountLivingNeighbors(Board board, byte x, byte y)
        // {

        // }

        // static IEnumerable<CellLocation> GetCellNeighborCoords(Board board, byte x, byte y)
        // {

        // }
    }
}
