namespace GameOfLife
{
    public enum GridState
    {
        Alive,
        Looping, // Stuck in an infinite loop.
        Stagnated, // No further changes in future iterations.
        Extinct, // No living cells remaining.
        Aborted
    }
}