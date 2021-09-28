using GameOfLife.Game;

namespace GameOfLife
{
    public static class ExtensionMethods
    {
        /// <summary>
        /// Determines if a Point is valid -- i.e., within established bounds.
        /// Point coordinates cannot be negative, nor can they exceed the specified limits.
        /// </summary>
        public static bool IsValid(this Point point, int maxWidth, int maxHeight)
        {
            if (maxWidth < 0)
                throw new ArgumentOutOfRangeException(nameof(maxWidth));
            if (maxHeight < 0)
                throw new ArgumentOutOfRangeException(nameof(maxHeight));

            return
                point.X >= 0 &&
                point.Y >= 0 &&
                point.X < maxWidth &&
                point.Y < maxHeight;
        }
    }
}