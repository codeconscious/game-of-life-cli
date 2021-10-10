using GameOfLife.Game;

namespace GameOfLife
{
    public class ConsolePrinter : IPrinter
    {
        public void Print(string text) => Write(text ?? "");

        public void PrintLine(string text) => WriteLine(text ?? "");

        public void PrintEntire(Grid grid, bool shouldClear)
        {
            // TODO: Refactor to use polymorphism instead.
            if (grid.IsHighResMode)
                PrintEntireHighRes(grid, shouldClear);
            else
                PrintEntireStandardRes(grid, shouldClear);
        }

        /// <summary>
        /// Outputs the entire grid to the console. Intended to be used at game start.
        /// </summary>
        /// <param name="grid"></param>
        /// <param name="shouldClear">Specifies whether the screen be cleared first.</param>
        private void PrintEntireStandardRes(Grid grid, bool shouldClear)
        {
            ArgumentNullException.ThrowIfNull(grid);

            if (shouldClear)
                Clear();

            ForegroundColor = GridStateColors.GameStateColors[grid.State];

            for (var x = 0; x < grid.ScreenDimensions.Width; x++)
            {
                for (var y = 0; y < grid.ScreenDimensions.Height; y++)
                {
                    SetCursorPosition(x, y);
                    Write(grid.GridChars[grid.CellGrid[x, y].IsAlive]);
                }
            }
        }

        /// <summary>
        /// Outputs the entire grid to the console. Intended to be used at game start.
        /// </summary>
        /// <param name="grid"></param>
        /// <param name="shouldClear">Specifies whether the screen be cleared first.</param>
        private void PrintEntireHighRes(Grid grid, bool shouldClear)
        {
            ArgumentNullException.ThrowIfNull(grid);

            if (shouldClear)
                Clear();

            ForegroundColor = GridStateColors.GameStateColors[grid.State];

            try
            {
                foreach (var group in grid.CellGroupMap.Values.Distinct().ToList())
                {
                    SetCursorPosition(group.PrintLocation.X, group.PrintLocation.Y);
                    var signature = group.GetCellLifeSignature();
                    var @char = CellGroup.GetCharacterToPrint(signature);
                    Write(@char);
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
        public void PrintUpdates(Grid grid, List<Cell> updatedCells)
        {
            ArgumentNullException.ThrowIfNull(grid);
            ArgumentNullException.ThrowIfNull(updatedCells);

            ForegroundColor = GridStateColors.GameStateColors[grid.State];

            try
            {
                foreach (var cell in updatedCells)
                {
                    SetCursorPosition(cell.Location.X, cell.Location.Y);
                    Write(grid.GridChars[cell.IsAlive]);
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
        public void PrintUpdates(Grid grid, List<CellGroup> updatedGroups)
        {
            ArgumentNullException.ThrowIfNull(updatedGroups);
            ArgumentNullException.ThrowIfNull(grid);

            ForegroundColor = GridStateColors.GameStateColors[grid.State];

            try
            {
                foreach (var group in updatedGroups)
                {
                    SetCursorPosition(group.PrintLocation.X, group.PrintLocation.Y);

                    var signature = group.GetCellLifeSignature();
                    var @char = CellGroup.GetCharacterToPrint(signature);
                    Write(@char);
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
        public void PrintIterationSummary(Grid grid, TimeSpan? duration = null)
        {
            ArgumentNullException.ThrowIfNull(grid);

            ResetColor();

            SetCursorPosition(0, grid.OutputRow);

            var durationClause = duration == null
                ? ""
                : $"({duration.Value.TotalMilliseconds:#,##0}ms)";

            Write($"<Press any key to quit>  Iteration {grid.CurrentIteration:#,##0} {durationClause}  ");
        }

        /// <summary>
        /// Outputs a single-line summary of the entire game.
        /// Intended to be used when there's an entire-grid state change.
        /// </summary>
        /// <param name="grid"></param>
        public void PrintGameSummary(Grid grid)
        {
            ArgumentNullException.ThrowIfNull(grid);

            var stateClause = grid.State switch
            {
                GridState.Extinct => "Extinction",
                GridState.Looping => "Endless loop",
                GridState.Stagnated => "Stagnation",
                GridState.Aborted => "Aborted",
                _ => "Unknown" // Should never be reached
            };

            var iterationClause = $"{grid.CurrentIteration:#,##0} iterations";

            var seconds = grid.GameStopwatch.Elapsed.TotalSeconds;

            var secondsClause = $"{seconds:#,##0.###} sec";

            var iterationsPerSecondClause =
                (grid.CurrentIteration / seconds).ToString("#,##0.###") +
                " iterations/sec";

            var gridClause = $"{grid.Width} × {grid.Height}";

            var populationClause = $"{grid.PopulationRatio * 100:0.##}%";

            ForegroundColor = GridStateColors.GameStateColors[grid.State];

            // Move to the output row, then clear it.
            SetCursorPosition(0, grid.OutputRow);
            ClearCurrentLine();

            // Ex.: Extinction | 813 iterations | 4.181 sec | 194.431 iterations/sec | 44 × 178 | 7,832 cells
            Write(string.Join(" | ", new string[]
                {
                    stateClause,
                    iterationClause,
                    secondsClause,
                    iterationsPerSecondClause,
                    gridClause,
                    grid.Area.ToString("#,##0 cells"),
                    populationClause + " alive"
                  }));

            // If the grid is looping, then show how to exit.
            if (grid.State == GridState.Looping)
            {
                ForegroundColor = ConsoleColor.Gray;
                SetCursorPosition(0, grid.OutputRow + 1);
                WriteLine("Press any key to quit.");
            }
        }

        /// <summary>
        /// Fills the current terminal line with spaces, effectively erasing it.
        /// </summary>
        public void ClearCurrentLine()
        {
            // Write(new string(' ', Console.WindowWidth - 1) + "\r");
        }
    }
}