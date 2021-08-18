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
            var activeLocations = new HashSet<CellLocation>
            {
                new CellLocation(0,0),
                new CellLocation(1,2),
                new CellLocation(5,5)
            };
            var board = new Board(9, 9, activeLocations);
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

        public Cell(ushort x, ushort y, bool isOn = false)
        {
            Location = new CellLocation(x, y);
            IsOn = isOn;
        }
    }

    public record CellLocation(ushort X, ushort Y); // TODO: Method to generate random ones

    public class Board
    {
        public Cell[,] CellGrid { get; set; }
        public ushort ColumnCount => (ushort) CellGrid.GetLength(0);
        public ushort RowCount => (ushort) CellGrid.GetLength(1);

        public Board(ushort columnCount, ushort rowCount,
                     IEnumerable<CellLocation> cellsToTurnOn = default)
        {
            CellGrid = new Cell[columnCount, rowCount];

            // Create the cells and populate the grid with them.
            for (ushort column = 0; column < columnCount; column++)
            {
                for (ushort row = 0; row < rowCount; row++)
                {
                    var testLocation = new CellLocation(column, row);
                    var shouldTurnOn = cellsToTurnOn.Contains(testLocation);
                    CellGrid[column,row] = new Cell(column, row, shouldTurnOn);
                }
            }
        }

        public void PrintGrid()
        {
            // TODO: Try using LINQ instead.
            for (ushort column = 0; column < ColumnCount; column++)
            {
                for (ushort row = 0; row < RowCount; row++)
                {
                    Write(CellGrid[column,row].IsOn ? "X" : "•");
                }

                WriteLine();
            }
        }
    }
}
