namespace GameOfLife
{
    public enum GridStatus
    {
        Alive,
        Looping, // Stuck in an infinite loop.
        Stagnated, // No further changes in future iterations.
        Dead, // No living cells remaining.
        Aborted
    }
}