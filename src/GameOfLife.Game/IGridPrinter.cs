namespace GameOfLife.Game;

public interface IGridPrinter
{
    public void PrintEntire(Grid grid, bool shouldClear);
    public void PrintUpdates(Grid grid, List<Cell> cellsForUpdate);
    public void PrintIterationSummary(Grid grid, TimeSpan? duration = null);
    public void PrintGameSummary(Grid grid);
}