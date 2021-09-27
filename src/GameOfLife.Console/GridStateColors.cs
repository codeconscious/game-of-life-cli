using GameOfLife.Game;

namespace GameOfLife
{
    public static class GridStateColors
    {
        public static readonly Dictionary<GridState, ConsoleColor> GameStateColors
            = new()
            {
                { GridState.Alive, ConsoleColor.Green },
                { GridState.Extinct, ConsoleColor.DarkRed },
                { GridState.Looping, ConsoleColor.Cyan },
                { GridState.Stagnated, ConsoleColor.Blue },
                { GridState.Aborted, ConsoleColor.Red },
            };
    }
}