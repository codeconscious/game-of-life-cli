using System;
using System.Collections.Generic;
using System.Linq;
using static System.Console;

namespace GameOfLife
{
    public static class Program
    {
        static void Main(string[] args)
        {
            // Check arguments.
            if (args.Length != 2)
            {
                WriteLine("You must specify the grid size by entering two numbers between 3 and 128, inclusive.");
                return;
            }

            // Verify the row arg is valid.
            if (!byte.TryParse(args[0], out var maxRowCount) || maxRowCount < 3)
            {
                WriteLine("The first argument is not a number or is out of range. Please pass in numbers between 3 and 128, inclusive.");
                return;
            }

            // Verify the column arg is valid.
            if (!byte.TryParse(args[1], out var maxColumnCount) || maxColumnCount < 3)
            {
                WriteLine("The second argument is not a number or is out of range. Please pass in numbers between 3 and 128, inclusive.");
                return;
            }

            var activeLocations = new HashSet<CellLocation>
            {
                new CellLocation(0, 0),
                new CellLocation(1, 2),
                new CellLocation(4, 5),
                new CellLocation(5, 5),
                new CellLocation(5, 6),
                new CellLocation(8, 4),

                // Always activate the lower-rightmost cell.
                new CellLocation((byte)(maxRowCount - 1),
                                 (byte)(maxColumnCount - 1))
            };

            var board = new Board(maxRowCount, maxColumnCount, activeLocations);
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
