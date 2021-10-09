using GameOfLife.Game;

namespace GameOfLife.Tests;

/// <summary>
/// A class to avoid printing to the console during unit tests.
/// </summary>
public class TestPrinter : IPrinter
{
    public void Print(string text) { }

    public void PrintLine(string text) { }

    public void PrintEntire(Grid grid, bool shouldClear) { }

    public void PrintGameSummary(Grid grid) { }

    public void PrintIterationSummary(Grid grid, TimeSpan? duration = null) { }

    public void PrintUpdates(Grid grid, List<Cell> cellsForUpdate) { }

    public void PrintUpdates(Grid grid, List<CellGroup> groups) { }

    public void ClearCurrentLine() { }
}