namespace GameOfLife.Game
{
    public class Cell
    {
        public Point Location { get; init; }
        public bool IsAlive { get; set; }

        public Cell(int x, int y, bool isAlive = false)
        {
            Location = new Point(x, y);
            IsAlive = isAlive;
        }

        /// <summary>
        /// Determines if this cell should be alive or not in the next iteration.
        /// </summary>
        /// <param name="livingNeighborCount">Count of surrounding cells that are alive.</param>
        /// <returns>A bool indicating life (true) or death (false).</returns>
        public bool ShouldCellLive(int livingNeighborCount)
        {
            return livingNeighborCount switch
            {
                2 or 3 when this.IsAlive => true,
                3 when !this.IsAlive => true,
                _ => false
            };
        }

        /// <summary>
        /// Flips (switches) the status of each submitted cell.
        /// If alive, it dies; if dead, it lives.
        /// </summary>
        public static void FlipLifeStatuses(List<Cell> cellsToUpdate)
        {
            foreach (var cell in cellsToUpdate)
                cell.IsAlive = !cell.IsAlive;
        }
    }
}