namespace GameOfLife
{
    public class Cell
    {
        public CoordinatePair Coordinates { get; init; }
        public bool IsAlive { get; set; }

        public Cell(int row, int column, bool isOn = false)
        {
            Coordinates = new CoordinatePair(row, column);
            IsAlive = isOn;
        }

        public bool WillCellBeAliveNextIteration(int livingNeighborCount)
        {
            return livingNeighborCount switch
            {
                2 or 3 when this.IsAlive => true,
                3 when !this.IsAlive => true,
                _ => false
            };
        }

        public void FlipLifeStatus() => IsAlive = !IsAlive;

        /// <summary>
        /// Updates the cells in the grid as needed, then returns the affected cells.
        /// </summary>
        public static void FlipStatuses(IList<Cell> cellsToUpdate)
        {
            foreach (var cell in cellsToUpdate)
                cell.FlipLifeStatus();
        }
    }
}