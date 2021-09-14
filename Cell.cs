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

        /// <summary>
        /// Flips (switches) the status of each submitted cell.
        /// If alive, it dies; if dead, it lives.
        /// </summary>
        public static void FlipLifeStatuses(IList<Cell> cellsToUpdate)
        {
            foreach (var cell in cellsToUpdate)
                cell.IsAlive = !cell.IsAlive;
        }
    }
}