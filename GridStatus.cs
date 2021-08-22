namespace GameOfLife
{
    public enum GridStatus
    {
        Alive,
        Looping, // Stuck in an infinite loop.
        Stagnated, // No further grid changes to make.
        Dead, // No living cells remaining.
    }
}