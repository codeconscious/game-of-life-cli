namespace GameOfLife
{
    public static class Utilities
    {
        public static List<Cell> GetCellsToUpdate(Grid grid)
        {
            var cellsToFlip = new List<Cell>();

            foreach (var cell in grid.AllCellsFlattened)
            {
                var livingNeighborCount = grid.NeighborMap[cell].Count(c => c.IsAlive);

                var willCellBeAlive = WillCellBeAliveNextIteration(cell, livingNeighborCount);

                if (cell.IsAlive != willCellBeAlive)
                    cellsToFlip.Add(cell);
            }

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
    }
}