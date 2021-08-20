namespace GameOfLife
{
    public static class Utilities
    {
        public static Board GetDescendantBoard(Board board)
        {
            var newCells = new List<Cell>();

            // WriteLine($"Found {board.AllCellsFlattened.Count} cells...");

            foreach (var cell in board.AllCellsFlattened)
            {
                var livingNeighbors = CountLivingNeighbors(board, cell.Coordinates);
                // WriteLine("livingNeighbors == " + livingNeighbors);

                var newCell = GetDescendantCell(cell, livingNeighbors);
                newCells.Add(newCell);
            }
            // WriteLine("newCells.Count == " + newCells.Count());

            // WriteLine("  newCells.Where(c => c.IsOn): " + newCells.Count(c => c.IsOn));
            var newBoard = new Board(board.RowCount,
                                     board.ColumnCount,
                                     newCells.Where(c => c.IsOn)
                                             .Select(c => c.Coordinates));
            // WriteLine("Cells on: " + newBoard.AllCellsFlattened.Count(c => c.IsOn));

            return newBoard;
        }

        private static int CountLivingNeighbors(Board board, Coordinates coordinates)
        {
            // var coords = coordinates.Row + "," + coordinates.Column;
            var neighborCoords = GetCellNeighborCoords(board, coordinates);
            // WriteLine($"  {coords} neighborCoords.Count: " + neighborCoords.Count());

            var neighborCells = board.AllCellsFlattened
                                     .Where(c => neighborCoords.Contains(c.Coordinates) && c.IsOn);
            // WriteLine($"  {coords} neighborCells.Count: " + neighborCells.Count());

            return neighborCells.Count();
        }

        private static IEnumerable<Coordinates> GetCellNeighborCoords(Board board, Coordinates sourceCellCoordinates)
        {
            var potentialCoordinateValues = new List<(int Row, int Column)>();

            for (var row = -1; row <= 1; row++)
            {
                for (var column = -1; column <= 1; column++)
                {
                    potentialCoordinateValues.Add((sourceCellCoordinates.Row + row,
                                                   sourceCellCoordinates.Column + column));
                }
            }
            potentialCoordinateValues.Remove((sourceCellCoordinates.Row, sourceCellCoordinates.Column));
            // WriteLine("  Potential: " + potentialCoordinateValues.Count());

            var validCoordinateValues = potentialCoordinateValues
                .Where(v => v.Row >= 0 && v.Column >= 0 &&
                       v.Row < board.RowCount && v.Column < board.ColumnCount);
            // WriteLine("Valid: " + validCoordinateValues.Count());

            return validCoordinateValues.Select(v => new Coordinates((byte)v.Row,
                                                                     (byte)v.Column));
        }

        private static Cell GetDescendantCell(Cell cell, int livingNeighbors)
        {
            // Write($"Cell {cell.Coordinates.Row},{cell.Coordinates.Column} " +
            //       $"is {(cell.IsOn ? "is ON" : "is off")} " +
            //       $"and has {livingNeighbors} living neighbors");

            var shouldLive = livingNeighbors switch
            {
                2 or 3 when cell.IsOn => true,
                3 when !cell.IsOn => true,
                _ => false
            };

            // WriteLine("; shouldLive == " + shouldLive.ToString().ToUpper());

            return cell with { IsOn = shouldLive };
        }
    }
}