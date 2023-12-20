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
            ArgumentOutOfRangeException.ThrowIfNegative(maxWidth);
            ArgumentOutOfRangeException.ThrowIfNegative(maxHeight);

            return
                point.X >= 0 &&
                point.Y >= 0 &&
                point.X < maxWidth &&
                point.Y < maxHeight;
        }
    }
}
