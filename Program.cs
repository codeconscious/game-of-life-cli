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

            // Check the number range.
            if (!byte.TryParse(args[0], out var maxRowColumnCount))
            {
                WriteLine("Please pass in a single number smaller than 128.");
                return;
            }

            var activeLocations = new HashSet<CellLocation>
            {
                new CellLocation(0, 0),
                new CellLocation(1, 2),
                new CellLocation(5, 5),
                new CellLocation((ushort)(maxRowColumnCount - 1),
                                 (ushort)(maxRowColumnCount - 1))
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

        public Cell(ushort row, ushort column, bool isOn = false)
        {
            Location = new CellLocation(row, column);
            IsOn = isOn;
        }
    }

    public record CellLocation(ushort Row, ushort Column); // TODO: Method to generate random ones

    public class Board
    {
        public Cell[,] CellGrid { get; set; }
        public ushort RowCount => (ushort) CellGrid.GetLength(0);
        public ushort ColumnCount => (ushort) CellGrid.GetLength(1);

        public Board(ushort rowCount, ushort columnCount,
                     IEnumerable<CellLocation> cellsToTurnOn = default)
        {
            CellGrid = new Cell[rowCount, columnCount];

            // Create the cells and populate the grid with them.
            for (ushort row = 0; row < rowCount; row++)
            {
                for (ushort column = 0; column < columnCount; column++)
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
            for (ushort row = 0; row < RowCount; row++)
            {
                for (ushort column = 0; column < ColumnCount; column++)
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
