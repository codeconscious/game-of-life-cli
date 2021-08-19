namespace GameOfLife
{
    static class Program
    {
        const byte MinRowsOrColumns = 3;

        static void Main(string[] args)
        {
            const string instructions = "Specify the grid size by entering two numbers between 3 and 128, inclusive.";

            // Check arguments.
            if (args.Length != 2)
            {
                WriteLine(instructions);
                return;
            }

            // Verify the row arg is valid.
            if (!byte.TryParse(args[0], out var rowCount) || rowCount < MinRowsOrColumns)
            {
                WriteLine("The first argument is not a number or is out of range.");
                WriteLine(instructions);
                return;
            }

            // Verify the column arg is valid.
            if (!byte.TryParse(args[1], out var columnCount) || columnCount < MinRowsOrColumns)
            {
                WriteLine("The second argument is not a number or is out of range.");
                WriteLine(instructions);
                return;
            }

            var activeLocations = new HashSet<Coordinates>
            {
                new Coordinates(0, 0),
                new Coordinates(1, 2),
                new Coordinates(4, 5),
                new Coordinates(5, 5),
                new Coordinates(5, 6),
                new Coordinates(8, 4),

                // Always activate the lower-rightmost cell.
                new Coordinates((byte)(rowCount - 1),
                                (byte)(columnCount - 1))
            };

            var board = new Board(rowCount, columnCount, activeLocations);
            board.PrintGrid();
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
