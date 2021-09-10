namespace GameOfLife
{
    public static class Colors
    {
        public static readonly Dictionary<GridStatus, ConsoleColor> StatusColors
            = new()
            {
                { GridStatus.Alive, ConsoleColor.Green },
                { GridStatus.Dead, ConsoleColor.DarkRed },
                { GridStatus.Looping, ConsoleColor.Cyan },
                { GridStatus.Stagnated, ConsoleColor.Blue },
                { GridStatus.Aborted, ConsoleColor.DarkRed },
            };
    }
}