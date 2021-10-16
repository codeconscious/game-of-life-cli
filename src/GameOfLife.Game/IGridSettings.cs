namespace GameOfLife.Game
{
    public interface IGridSettings
    {
        /// <summary>
        /// Determines whether to use normal characters or else
        /// block characters to allow a higher-resolution simulation.
        /// </summary>
        public bool UseHighResMode { get; }

        /// <summary>
        /// The desired width of the output area.
        /// </summary>
        public int Width { get; }

        /// <summary>
        /// The desired height of the output area.
        /// </summary>
        public int Height { get; }

        /// <summary>
        /// The percentage of cells that should be alive at
        /// the beginning of the simulation.
        /// </summary>
        public int PopulationRatio { get; }

        /// <summary>
        /// The initial delay in milliseconds between two consecutive iterations (turns).
        /// (The user can adjust this during the game.)
        /// </summary>
        public ushort IterationDelayMs { get; }

        /// <summary>
        /// The maximum population percentage allowed when setting it randomly.
        /// If it's too high, then the game will likely end very quickly.
        /// </summary>
        public const byte MaximumRandomPopulationRatio = 70;
    }
}