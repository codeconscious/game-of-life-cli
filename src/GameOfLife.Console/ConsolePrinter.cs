namespace GameOfLife.Console;

public class ConsolePrinter : IPrinter
{
    public void Write(string text = "") => System.Console.Write(text);

    public void WriteLine(string text = "") => System.Console.WriteLine(text);

    public void WriteEntire(Grid grid, bool shouldClear)
    {
        if (grid.IsHighResMode)
            WriteEntireHighRes(grid, shouldClear);
        else
            WriteEntireStandardRes(grid, shouldClear);
    }

    /// <summary>
    /// Outputs the entire grid to the console. Intended to be used at game start.
    /// </summary>
    /// <param name="grid"></param>
    /// <param name="shouldClear">Specifies whether the screen be cleared first.</param>
    private void WriteEntireStandardRes(Grid grid, bool shouldClear)
    {
        ArgumentNullException.ThrowIfNull(grid);

        if (shouldClear)
            Clear();

        ForegroundColor = GridStateColors.GameStateColors[grid.State];

        for (int x = 0; x < grid.ScreenDimensions.Width; x++)
        {
            for (int y = 0; y < grid.ScreenDimensions.Height; y++)
            {
                SetCursorPosition(x, y);
                System.Console.Write(grid.GridChars[grid.CellGrid[x, y].IsAlive]);
            }
        }
    }

    /// <summary>
    /// Outputs the entire grid to the console. Intended to be used at game start.
    /// </summary>
    /// <param name="grid"></param>
    /// <param name="shouldClear">Specifies whether the screen be cleared first.</param>
    private void WriteEntireHighRes(Grid grid, bool shouldClear)
    {
        ArgumentNullException.ThrowIfNull(grid);

        if (shouldClear)
            Clear();

        ForegroundColor = GridStateColors.GameStateColors[grid.State];

        try
        {
            foreach (CellGroup group in grid.CellGroupMap.Values.Distinct().ToList())
            {
                SetCursorPosition(group.WriteLocation.X, group.WriteLocation.Y);
                char @char = group.GetCellLifeCharacter();
                System.Console.Write(@char);
            }
        }
        catch (Exception)
        {
            Clear();
            ResetColor();
            throw;
        }
    }

    /// <summary>
    /// Outputs only updated cells for the current iteration.
    /// </summary>
    /// <param name="grid"></param>
    /// <param name="updatedCells"></param>
    public void WriteUpdates(Grid grid, List<Cell> updatedCells)
    {
        ArgumentNullException.ThrowIfNull(grid);
        ArgumentNullException.ThrowIfNull(updatedCells);

        ForegroundColor = GridStateColors.GameStateColors[grid.State];

        try
        {
            foreach (Cell cell in updatedCells)
            {
                SetCursorPosition(cell.Location.X, cell.Location.Y);
                System.Console.Write(grid.GridChars[cell.IsAlive]);
            }
        }
        catch (Exception)
        {
            Clear();
            ResetColor();
            throw;
        }
    }

    /// <summary>
    /// Outputs only updated cells for the current iteration.
    /// </summary>
    /// <param name="grid"></param>
    /// <param name="updatedGroups"></param>
    public void WriteUpdates(Grid grid, List<CellGroup> updatedGroups)
    {
        ArgumentNullException.ThrowIfNull(updatedGroups);
        ArgumentNullException.ThrowIfNull(grid);

        ForegroundColor = GridStateColors.GameStateColors[grid.State];

        try
        {
            foreach (CellGroup group in updatedGroups)
            {
                SetCursorPosition(group.WriteLocation.X, group.WriteLocation.Y);
                char @char = group.GetCellLifeCharacter();
                System.Console.Write(@char);
            }
        }
        catch (Exception)
        {
            Clear();
            ResetColor();
            throw;
        }
    }

    /// <summary>
    /// Outputs a single-line summary of the current iteration.
    /// </summary>
    /// <param name="grid"></param>
    /// <param name="duration">An optional iteration time duration.</param>
    public void WriteIterationSummary(Grid grid, TimeSpan? duration = null)
    {
        ArgumentNullException.ThrowIfNull(grid);

        ResetColor();
        SetCursorPosition(0, grid.OutputRow);
        string durationClause = duration == null
            ? string.Empty
            : $"({duration.Value.TotalMilliseconds:#,##0}ms)";

        System.Console.Write($"<Press any key to quit>  Iteration {grid.CurrentIteration:#,##0} {durationClause}  ");
    }

    /// <summary>
    /// Outputs a single-line summary of the entire game.
    /// Intended to be used when there's an entire-grid state change.
    /// </summary>
    /// <param name="grid"></param>
    public void WriteGameSummary(Grid grid)
    {
        ArgumentNullException.ThrowIfNull(grid);

        string stateClause = grid.State switch
        {
            GridState.Extinct => "Extinction",
            GridState.Looping => "Endless loop",
            GridState.Stagnated => "Stagnation",
            GridState.Aborted => "Aborted",
            _ => "Unknown" // Should never be reached
        };

        string iterationClause = $"{grid.CurrentIteration:#,##0} iterations";
        double seconds = grid.GameStopwatch.Elapsed.TotalSeconds;
        string secondsClause = $"{seconds:#,##0.###} sec";
        string iterationsPerSecondClause =
            $"{(grid.CurrentIteration / seconds):#,##0.###} iterations/sec";
        string gridClause = $"{grid.Width} × {grid.Height}";
        string populationClause = $"{grid.PopulationRatio * 100:0.##}%";

        ForegroundColor = GridStateColors.GameStateColors[grid.State];

        // Move to the output row, then clear it.
        SetCursorPosition(0, grid.OutputRow);

        // Ex.: Extinction | 813 iterations | 4.181 sec | 194.431 iterations/sec | 44 × 178 | 7,832 cells
        System.Console.Write(
            string.Join(
                " | ",
                new List<string> {
                    stateClause,
                    iterationClause,
                    secondsClause,
                    iterationsPerSecondClause,
                    gridClause,
                    grid.Area.ToString("#,##0 cells"),
                    populationClause + " alive"
                }));

        if (grid.State != GridState.Looping)
            return;
        
        // The grid is looping, so show how to exit.
        ForegroundColor = ConsoleColor.Gray;
        SetCursorPosition(0, grid.OutputRow + 1);
        System.Console.WriteLine("Press any key to quit.");
    }
}

