namespace GameOfLife
{
    public class Cell
    {
        public Coordinates Coordinates { get; init; }
        public bool IsAlive { get; set; }


        public Cell(byte row, byte column)
        {
            Coordinates = new Coordinates(row, column);
        }

        public Cell(byte row, byte column, bool isOn = false)
        {
            Coordinates = new Coordinates(row, column);
            IsAlive = isOn;
        }

        public void UpdateCellStatus(int livingNeighborCount)
        {
            IsAlive = livingNeighborCount switch
            {
                2 or 3 when this.IsAlive => true,
                3 when !this.IsAlive => true,
                _ => false
            };
        }

        public void FlipStatus() => IsAlive = !IsAlive;
    }
}