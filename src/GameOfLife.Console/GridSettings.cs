namespace GameOfLife
{
    /// <summary>
    /// The settings used to create a grid and affect elements of the game elsewhere.
    /// </summary>
    public class Settings : IGridSettings
    {
        public bool UseHighResMode { get; }
        public byte MinimumWidthHeight { get; } = 3;
        public int Width { get; private init; }
        public int Height { get; private init; }
        public int InitialPopulationRatio { get; private init; }

        /// <summary>
        /// The initial delay in milliseconds between two consecutive iterations (turns).
        /// </summary>
        public ushort InitialIterationDelayMs { get; private init; }

        /// <summary>
        /// The maximum population percentage allowed when setting it randomly.
        /// If it's too high, then the game will likely end very quickly.
        /// </summary>
        public const byte MaximumRandomPopulationRatio = 70;

        /// <summary>
        /// Constructor that is expected to take in arguments from the user.
        /// </summary>
        /// <param name="args"></param>
        public Settings(string[] args, IPrinter printer)
        {
            // TODO: Make this a proper setting.
            UseHighResMode = true;

            if (args.Length != 3 && args.Length != 4)
                throw new ArgumentException("An unsupported number of arguments was passed in.");

            // Verify the width (X axis) arg
            if (args[0] == "-1")
            {
                Width = Console.WindowWidth == 0 // This can occur when debugging.
                    ? 200 // Debugging value
                    : Console.WindowWidth;
            }
            else
            {
                var width = ushort.Parse(args[0]);

                if (width < MinimumWidthHeight)
                    throw new ArgumentOutOfRangeException(nameof(width));

                Width = width;
            }

            // Verify the height (Y axis) arg
            if (args[1] == "-1")
            {
                // Leave room at the bottom of the screen for output (during and after the game).
                const int bottomMargin = 3;

                Height = Console.WindowHeight == 0 // This can occur when debugging.
                    ? 30 // Debugging value
                    : Console.WindowHeight - bottomMargin;
            }
            else
            {
                var height = ushort.Parse(args[1]);

                if (height < MinimumWidthHeight)
                    throw new ArgumentOutOfRangeException(nameof(height));

                Height = height;
            }

            // Verify the population ratio arg
            if (args[2] == "-1")
            {
                InitialPopulationRatio = new Random().Next(MaximumRandomPopulationRatio);
            }
            else
            {
                var populationRatio = byte.Parse(args[2]);

                if (populationRatio > 100 || populationRatio < 1)
                    throw new ArgumentOutOfRangeException(nameof(populationRatio));

                InitialPopulationRatio = populationRatio;
            }

            // Verify the optional iteration delay arg, or if it's missing, set a default.
            if (args.Length == 4)
            {
                InitialIterationDelayMs = ushort.Parse(args[3]);
            }
            else
            {
                InitialIterationDelayMs = 50; // Default value in milliseconds
            }

            printer.PrintLine($"Grid:            {Width} columns x {Height} rows ({Width * Height:#,##0} cells)");
            printer.PrintLine($"Population:      {InitialPopulationRatio}%");
            printer.PrintLine($"Iteration delay: {InitialIterationDelayMs}ms");
        }
    }
}