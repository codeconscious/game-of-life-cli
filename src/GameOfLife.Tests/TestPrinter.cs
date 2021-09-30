using GameOfLife.Game;

namespace GameOfLife.Tests;

public class TestPrinter : IPrinter
{
    public void Print(string text) { }

    public void PrintEntire(Grid grid, bool shouldClear) { }

    public void PrintGameSummary(Grid grid) { }

    public void PrintIterationSummary(Grid grid, TimeSpan? duration = null) { }

    public void PrintLine(string text) { }

    public void PrintUpdates(Grid grid, List<Cell> cellsForUpdate) { }
}