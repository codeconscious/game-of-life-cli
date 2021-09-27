namespace GameOfLife.Game
{
    /// <summary>
    /// An enum indicating the state of the grid/game.
    /// </summary>
    public enum GridState
    {
        /// <summary>
        /// The initial state indicating the game is ongoing.
        /// </summary>
        Alive,

        /// <summary>
        /// The grid is trapped in an infinite loop.
        /// </summary>
        Looping,

        /// <summary>
        /// There can be no further changes in any future iterations.
        /// </summary>
        Stagnated,

        /// <summary>
        /// No living cells remain.
        /// </summary>
        Extinct,

        /// <summary>
        /// The user ended the game.
        /// </summary>
        Aborted
    }
}