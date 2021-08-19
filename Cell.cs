namespace GameOfLife
{
    public readonly record struct Cell
    {
        public Coordinates Location { get; init; }
        public bool IsOn { get; init; }

        public Cell(byte row, byte column, bool isOn = false)
        {
            Location = new Coordinates(row, column);
            IsOn = isOn;
        }
    }
}