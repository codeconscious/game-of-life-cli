namespace GameOfLife
{
    internal static class Program
    {
        private const string Instructions =
            "Pass in three numbers: row count (3-128), column count (3-128), and activation percentage (1-100).";

        private static void Main(string[] args)
        {
            BoardSettings settings;
            try
            {
                settings = new BoardSettings(args);
            }
            catch (ArgumentException ex)
            {
                 WriteLine(ex.Message);
                 WriteLine(Instructions);
                 return;
            }

            WriteLine();

            var seedBoard = new Board(settings);
            seedBoard.Print();
            WriteLine();

            var descendantBoard = Utilities.GetDescendantBoard(seedBoard);
            descendantBoard.Print();
            WriteLine();
        }
    }
}
