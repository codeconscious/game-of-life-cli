using System.Threading.Tasks;

namespace GameOfLife
{
    public static class Utilities
    {
        public static List<Cell> GetCellsToUpdateInParallel(Grid grid)
        {
            var cellsToFlip = new List<Cell>();

            Parallel.ForEach(grid.AllCellsFlattened, cell =>
            {
                var livingNeighborCount = grid.NeighborMap[cell].Count(c => c.IsAlive);

                var willCellBeAlive = WillCellBeAliveNextIteration(cell, livingNeighborCount);

                if (cell.IsAlive != willCellBeAlive)
                    cellsToFlip.Add(cell);
            });

            // Console.Beep();
            return cellsToFlip;
        }

        private static bool WillCellBeAliveNextIteration(Cell cell, int livingNeighborCount)
        {
            return livingNeighborCount switch
            {
                2 or 3 when cell.IsAlive => true,
                3 when !cell.IsAlive => true,
                _ => false
            };
        }

        /// <summary>
        /// Calculates the status of each cell in the provided grid,
        /// then returns a new grid populated with updated cells.
        /// </summary>
        /// <param name="grid"></param>
        // public static Grid GetDescendantGrid(Grid grid)
        // {
        //     var newCells = new List<Cell>();

        //     foreach (var cell in grid.AllCellsFlattened)
        //     {
        //         var livingNeighborCount = CountLivingNeighbors(grid, cell.Coordinates);

        //         var newCell = GetDescendantCell(cell, livingNeighborCount);
        //         newCells.Add(newCell);
        //     }

        //     var newGrid = new Grid(grid.RowCount,
        //                            grid.ColumnCount,
        //                            newCells.Where(c => c.IsAlive)
        //                                    .Select(c => c.Coordinates));

        //     return newGrid;
        // }

        // public static void UpdateGrid(Grid grid)
        // {
        //     foreach (var cell in grid.AllCellsFlattened)
        //     {
        //         // var livingNeighborCount = CountLivingNeighbors(grid, cell.Coordinates);
        //         // cell.UpdateCellStatus(livingNeighborCount);

        //         cell.UpdateCellStatus(grid.NeighborMap[cell].Count(c => c.IsAlive));
        //     }
        // }

        // public static void UpdateGridInParallel(Grid grid)
        // {
        //     Parallel.ForEach(grid.AllCellsFlattened, cell =>
        //     {
        //         // var livingNeighborCount = CountLivingNeighbors(grid, cell.Coordinates);
        //         // cell.UpdateCellStatus(livingNeighborCount);

        //         cell.UpdateCellStatus(grid.NeighborMap[cell].Count(c => c.IsAlive));
        //     });
        // }

        /// <summary>
        /// Counts the number of living neighbors surrounding a cell, given its coordinates.
        /// </summary>
        /// <param name="grid"></param>
        /// <param name="sourceCellCoordinates"></param>
        // private static int CountLivingNeighbors(Grid grid, Coordinates sourceCellCoordinates)
        // {
        //     var neighborCoordinates = GetCellNeighborCoords(grid, sourceCellCoordinates);

        //     return grid.AllCellsFlattened
        //                .Count(c => neighborCoordinates.Contains(c.Coordinates) &&
        //                            c.IsAlive);

        // }

        /// <summary>
        /// Returns all valid cell coordinates for a specific cells neighbors.
        /// Invalid coordinates (i.e., negative and those beyond the grid) are ignored.
        /// </summary>
        /// <param name="grid"></param>
        /// <param name="sourceCellCoordinates"></param>
        // public static IEnumerable<Coordinates> GetCellNeighborCoords(
        //     byte rows, byte columns, Coordinates sourceCellCoordinates)
        // {
        //     var potentialCoordinateValues = new List<(int Row, int Column)>();

        //     // Gather all potential neighbors, including invalid ones.
        //     for (var row = -1; row <= 1; row++)
        //     {
        //         for (var column = -1; column <= 1; column++)
        //         {
        //             potentialCoordinateValues.Add((sourceCellCoordinates.Row + row,
        //                                            sourceCellCoordinates.Column + column));
        //         }
        //     }

        //     // Remove the source cell coordinates, which were included above.
        //     potentialCoordinateValues.Remove((sourceCellCoordinates.Row,
        //                                       sourceCellCoordinates.Column));

        //     var validCoordinateValues = potentialCoordinateValues
        //                                     .Where(v => v.Row >= 0 &&
        //                                                 v.Column >= 0 &&
        //                                                 v.Row < rows &&
        //                                                 v.Column < columns);

        //     return validCoordinateValues.Select(v => new Coordinates((byte)v.Row,
        //                                                              (byte)v.Column));
        // }

        // private static Cell GetDescendantCell(Cell cell, int livingNeighbors)
        // {
        //     var shouldLive = livingNeighbors switch
        //     {
        //         2 or 3 when cell.IsAlive => true,
        //         3 when !cell.IsAlive => true,
        //         _ => false
        //     };

        //     return cell with { IsAlive = shouldLive };
        // }
    }
}