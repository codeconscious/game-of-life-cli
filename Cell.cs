namespace GameOfLife
{
    public record Cell
    {
        public CellLocation Location { get; init; }
        public bool IsOn { get; init; }

        public Cell(byte row, byte column, bool isOn = false)
        {
            Location = new CellLocation(row, column);
            IsOn = isOn;
        }
    }
}