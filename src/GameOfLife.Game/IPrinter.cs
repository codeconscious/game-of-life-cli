namespace GameOfLife.Game;

public interface IPrinter
{
    public void Write(string text = "");
    public void WriteLine(string text = "");

    public void WriteEntire(Grid grid, bool shouldClear);
    public void WriteUpdates(Grid grid, List<Cell> updatedCells);
    public void WriteUpdates(Grid grid, List<CellGroup> updatedGroups);
    public void WriteIterationSummary(Grid grid, TimeSpan? duration = null);
    public void WriteGameSummary(Grid grid);
}
