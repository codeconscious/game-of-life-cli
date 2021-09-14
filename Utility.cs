namespace GameOfLife
{
    public static class Utility
    {
        /// <summary>
        /// Fills the current terminal line with spaces, effectively erasing it.
        /// </summary>
        public static void ClearCurrentLine()
        {
            Write(new string(' ', Console.WindowWidth - 1) + "\r");
        }
    }
}