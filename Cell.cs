namespace GameOfLife
{
    public readonly record struct Cell
    {
        public Coordinates Coordinates { get; init; }
        public bool IsOn { get; init; }

        public Cell(byte row, byte column, bool isOn = false)
        {
            Coordinates = new Coordinates(row, column);
            IsOn = isOn;
        }
    }
}