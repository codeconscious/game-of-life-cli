﻿namespace GameOfLife
{
    internal static class Program
    {
        private const string Instructions =
            "Pass in three numbers: row count (3-128), column count (3-128), and activation percentage (1-100).";

        private static void Main(string[] args)
        {
            GridSettings settings;
            try
            {
                settings = new GridSettings(args);
            }
            catch (Exception ex)
            {
                ForegroundColor = ConsoleColor.Yellow;
                WriteLine(ex.Message);
                WriteLine(Instructions);
                ResetColor();
                return;
            }

            WriteLine();

            var seedGrid = new Grid(settings);
            seedGrid.Print();
            WriteLine();

            var descendantGrid = Utilities.GetDescendantGrid(seedGrid);
            descendantGrid.Print();
            WriteLine();
        }
    }
}
