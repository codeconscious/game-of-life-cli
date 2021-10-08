namespace GameOfLife.Game;

[Flags]
public enum CellGroupLocation : byte
{
    UpperLeft = 0,
    UpperRight = 1,
    LowerLeft = 2,
    LowerRight = 4
}