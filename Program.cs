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
            if (args.Length == 0)
            {
                WriteLine("You must specify the grid size by entering a single number.");
                return;
            }

            // Verify the number range is valid.
            if (!byte.TryParse(args[0], out var maxRowColumnCount) || maxRowColumnCount < 3)
            {
                WriteLine("Please pass in a single number between 3 and 128.");
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
                new CellLocation((byte)(maxRowColumnCount - 1),
                                 (byte)(maxRowColumnCount - 1))
            };

            var board = new Board(maxRowColumnCount, maxRowColumnCount, activeLocations);
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

    public enum CellResult { Underpopulated, StayAlive, Overpopulated, ToLive }

    public record Cell
    {
        public CellLocation Location { get; init; }
        public bool IsOn { get; init; }

        public Cell(byte row, byte column, bool isOn = false)
        {
            Location = new CellLocation(row, column);
            IsOn = isOn;
        }
    }

    public record CellLocation(byte Row, byte Column); // TODO: Method to generate random ones

    public class Board
    {
        public Cell[,] CellGrid { get; set; }
        public byte RowCount => (byte) CellGrid.GetLength(0);
        public byte ColumnCount => (byte) CellGrid.GetLength(1);

        public Board(byte rowCount, byte columnCount,
                     IEnumerable<CellLocation> cellsToTurnOn = default)
        {
            CellGrid = new Cell[rowCount, columnCount];

            // Create the cells and populate the grid with them.
            for (byte row = 0; row < rowCount; row++)
            {
                for (byte column = 0; column < columnCount; column++)
                {
                    var testLocation = new CellLocation(row, column);
                    var shouldTurnOn = cellsToTurnOn.Contains(testLocation);
                    CellGrid[row,column] = new Cell(row, column, shouldTurnOn);
                }
            }
        }

        public void PrintGrid()
        {
            // TODO: Try using LINQ instead.
            for (byte row = 0; row < RowCount; row++)
            {
                for (byte column = 0; column < ColumnCount; column++)
                {
                    var isOn = CellGrid[row,column].IsOn;

                    ForegroundColor = isOn ? ConsoleColor.Green
                                           : ConsoleColor.DarkGray;

                    Write(isOn ? "X" : "•");
                }

                WriteLine();
            }

            ResetColor();
        }
    }
}
