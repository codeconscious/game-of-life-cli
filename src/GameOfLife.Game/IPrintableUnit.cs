namespace GameOfLife.Game;

/// <summary>
/// An interface expected to be used for cells or
/// cell groups that can be printed to the screen.
/// </summary>
public interface IPrintableUnit
{
    public Point PrintLocation { get; }
}