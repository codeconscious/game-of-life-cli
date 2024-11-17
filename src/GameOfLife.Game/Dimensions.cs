namespace GameOfLife.Game;

public class Dimensions
{
    public int Width { get; init; }
    public int Height { get; init; }

    public int Area => Width * Height;

    public Dimensions(int width, int height)
    {
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