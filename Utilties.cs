namespace GameOfLife
{
    public static class Utilities
    {
        public static Board GetUpdatedBoard(Board board)
        {
            var newCells = new List<Cell>();

            // WriteLine($"Found {board.AllCellsFlattened.Count} cells...");

            foreach (var cell in board.AllCellsFlattened)
            {
                var livingNeighbors = CountLivingNeighbors(board, cell.Coordinates);
                var newCell = CreateDescendantCell(cell, livingNeighbors);
                newCells.Add(newCell);
            }

            var newBoard = new Board(board.RowCount,
                                     board.ColumnCount,
                                     newCells.Where(c => c.IsOn)
                                             .Select(c => c.Coordinates));

            return newBoard;
        }

        private static int CountLivingNeighbors(Board board, Coordinates coordinates)
        {
            var neighborCoords = GetCellNeighborCoords(board, coordinates);
            // WriteLine(neighborCoords.Count());

            var neighborCells = board.AllCellsFlattened
                                     .Where(c => neighborCoords.Contains(c.Coordinates));

            return neighborCells.Count();
        }

        private static IEnumerable<Coordinates> GetCellNeighborCoords(Board board, Coordinates coordinates)
        {
            var potentialCoordinateValues = new List<(int Row, int Column)>();

            for (var row = -1; row <= 1; row++)
            {
                for (var column = -1; column <= 1; column++)
                {
                    potentialCoordinateValues.Add((coordinates.Row + row,
                                                   coordinates.Column + column));
                }
            }

            var validCoordinateValues = potentialCoordinateValues
                .Where(v => v.Row < 0 && v.Column < 0 &&
                       v.Row >= board.RowCount && v.Column >= board.ColumnCount);

            return validCoordinateValues.Select(v => new Coordinates((byte)v.Row,
                                                                     (byte)v.Column));
        }

        private static Cell CreateDescendantCell(Cell cell, int livingNeighbors)
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