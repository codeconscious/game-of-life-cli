namespace GameOfLife.Game
{
    public interface IGridSettings
    {
        public bool UseHighResMode { get; }
        // public byte MinimumWidthHeight { get; }
        public int Width { get; }
        public int Height { get; }
        public int InitialPopulationRatio { get; }

        /// <summary>
        /// The initial delay in milliseconds between two consecutive iterations (turns).
        /// </summary>
        public ushort InitialIterationDelayMs { get; }

        /// <summary>
        /// The maximum population percentage allowed when setting it randomly.
        /// If it's too high, then the game will likely end very quickly.
        /// </summary>
        public const byte MaximumRandomPopulationRatio = 70;
    }
}