namespace GameOfLife.Game;

public sealed class Dimensions
{
    public int Width { get; init; }
    public int Height { get; init; }

    public int Area => Width * Height;

    public Dimensions(int width, int height)
    {
        ArgumentNullException.ThrowIfNull(width);
        ArgumentNullException.ThrowIfNull(height);

        if (width < 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(width),
                width,
                "A positive number must be specified.");
        }

        if (height < 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(height),
                height,
                "A positive number must be specified.");
        }

        Width = width;
        Height = height;
    }
}