namespace GameOfLife.Game;

public static class ExtensionMethods
{
    /// <summary>
    /// Determines if a Point is valid -- i.e., within established bounds.
    /// Point coordinates cannot be negative, nor can they exceed the specified limits.
    /// </summary>
    public static bool IsValid(this ref Point point, int maxWidth, int maxHeight)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(maxWidth);
        ArgumentOutOfRangeException.ThrowIfNegative(maxHeight);

        return
            point is { X: >= 0, Y: >= 0 } &&
            point.X < maxWidth &&
            point.Y < maxHeight;
    }
}
