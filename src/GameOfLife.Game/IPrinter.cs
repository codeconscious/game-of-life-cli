namespace GameOfLife.Game;

public interface IPrinter
{
    public void Print(string text);
    public void PrintLine(string text);

    public void PrintEntire(Grid grid, bool shouldClear);
    public void PrintUpdates(Grid grid, List<Cell> cellsForUpdate);
    public void PrintUpdates(IEnumerable<CellGroup> groups, Grid grid);
    public void PrintIterationSummary(Grid grid, TimeSpan? duration = null);
    public void PrintGameSummary(Grid grid);
    public void ClearCurrentLine();
}