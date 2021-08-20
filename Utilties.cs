namespace GameOfLife
{
    public static class Utilities
    {
        /// <summary>
        /// Calculates the status of each cell in the provided grid,
        /// then returns a new grid populated with updated cells.
        /// </summary>
        /// <param name="grid"></param>
        public static Grid GetDescendantGrid(Grid grid)
        {
            var newCells = new List<Cell>();

            foreach (var cell in grid.AllCellsFlattened)
            {
                var livingNeighbors = CountLivingNeighbors(grid, cell.Coordinates);

                var newCell = GetDescendantCell(cell, livingNeighbors);
                newCells.Add(newCell);
            }

            var newGrid = new Grid(grid.RowCount,
                                   grid.ColumnCount,
                                   newCells.Where(c => c.IsOn)
                                           .Select(c => c.Coordinates));

            return newGrid;
        }

        /// <summary>
        /// Counts the number of living neighbors surrounding a cell, given its coordinates.
        /// </summary>
        /// <param name="grid"></param>
        /// <param name="sourceCellCoordinates"></param>
        private static int CountLivingNeighbors(Grid grid, Coordinates sourceCellCoordinates)
        {
            var neighborCoordinates = GetCellNeighborCoords(grid, sourceCellCoordinates);

            var neighborCells = grid.AllCellsFlattened
                                    .Where(c => neighborCoordinates.Contains(c.Coordinates) &&
                                                c.IsOn);

            return neighborCells.Count();
        }

        /// <summary>
        /// Returns all valid cell coordinates for a specific cells neighbors.
        /// Invalid coordinates (i.e., negative and those beyond the grid) are ignored.
        /// </summary>
        /// <param name="grid"></param>
        /// <param name="sourceCellCoordinates"></param>
        private static IEnumerable<Coordinates> GetCellNeighborCoords(
            Grid grid, Coordinates sourceCellCoordinates)
        {
            var potentialCoordinateValues = new List<(int Row, int Column)>();

            // Gather all potential neighbors, including invalid ones.
            for (var row = -1; row <= 1; row++)
            {
                for (var column = -1; column <= 1; column++)
                {
                    potentialCoordinateValues.Add((sourceCellCoordinates.Row + row,
                                                   sourceCellCoordinates.Column + column));
                }
            }

            // Remove the source cell coordinates, which were included above.
            potentialCoordinateValues.Remove((sourceCellCoordinates.Row,
                                              sourceCellCoordinates.Column));

            var validCoordinateValues = potentialCoordinateValues
                                            .Where(v => v.Row >= 0 &&
                                                        v.Column >= 0 &&
                                                        v.Row < grid.RowCount &&
                                                        v.Column < grid.ColumnCount);

            return validCoordinateValues.Select(v => new Coordinates((byte)v.Row,
                                                                     (byte)v.Column));
        }

        private static Cell GetDescendantCell(Cell cell, int livingNeighbors)
        {
            var shouldLive = livingNeighbors switch
            {
                2 or 3 when cell.IsOn => true,
                3 when !cell.IsOn => true,
                _ => false
            };

            return cell with { IsOn = shouldLive };
        }
    }
}