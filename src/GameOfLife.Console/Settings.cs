namespace GameOfLife
{
    /// <summary>
    /// The settings used to create a grid and affect elements of the game elsewhere.
    /// </summary>
    public class Settings : IGridSettings
    {
        /// <summary>
        /// Determines whether to use normal characters or else
        /// block characters to allow a higher-resolution simulation.
        /// </summary>
        public bool UseHighResMode { get; }

        public byte MinimumWidthHeight { get; } = 5;

        /// <summary>
        /// The desired width of the output area.
        /// </summary>
        public int Width { get; private init; }

        /// <summary>
        /// The desired height of the output area.
        /// </summary>
        public int Height { get; private init; }

        /// <summary>
        /// The percentage of cells that should be alive at
        /// the beginning of the simulation.
        /// </summary>
        public int InitialPopulationRatio { get; private init; }

        /// <summary>
        /// The initial delay in milliseconds between two consecutive iterations (turns).
        /// (The user can adjust this during the game.)
        /// </summary>
        public ushort InitialIterationDelayMs { get; private init; }

        /// <summary>
        /// The maximum population percentage allowed when setting it randomly.
        /// If it's too high, then the game will likely end very quickly.
        /// </summary>
        public const byte MaximumRandomPopulationRatio = 70;

        /// <summary>
        /// Constructor that accepts arguments from the user.
        /// </summary>
        /// <param name="args"></param>
        /// <param name="printer"></param>
        public Settings(string[] args, IPrinter printer)
        {
            if (args.Length != 4 && args.Length != 5)
                throw new ArgumentException("An unsupported number of arguments was passed in.");

            UseHighResMode = args[0] switch
            {
                "1" => true,
                "0" => false,
                _ => throw new ArgumentOutOfRangeException(args[0])
            };

            // Verify the width (X axis) arg
            if (args[1] == "-1")
            {
                var autoWidth = Console.WindowWidth;

                // Ensure an even number in high-res mode because each
                // cell group contains an even number of cells.
                if (UseHighResMode && autoWidth % 2 != 0)
                    autoWidth--;

                Width = Console.WindowWidth == 0 // This can occur when debugging.
                    ? 200 // Debugging value
                    : autoWidth;
            }
            else
            {
                var width = ushort.Parse(args[1]);

                if (width < MinimumWidthHeight)
                    throw new ArgumentOutOfRangeException(nameof(width));

                Width = width;
            }

            // Verify the height (Y axis) arg
            if (args[2] == "-1")
            {
                // Leave room at the bottom of the screen for output (during and after the game).
                var bottomMargin = 3;

                var autoHeight = Console.WindowHeight - bottomMargin;

                // Ensure an even number in high-res mode.
                if (UseHighResMode && autoHeight % 2 != 0)
                    autoHeight--;

                Height = Console.WindowHeight == 0 // This can occur when debugging.
                    ? 30 // Debugging value
                    : autoHeight;
            }
            else
            {
                var height = ushort.Parse(args[2]);

                if (height < MinimumWidthHeight)
                    throw new ArgumentOutOfRangeException(nameof(height));

                Height = height;
            }

            // Verify the population ratio arg
            if (args[3] == "-1")
            {
                InitialPopulationRatio = new Random().Next(MaximumRandomPopulationRatio);
            }
            else
            {
                var populationRatio = byte.Parse(args[3]);

                if (populationRatio > 100 || populationRatio < 1)
                    throw new ArgumentOutOfRangeException(nameof(populationRatio));

                InitialPopulationRatio = populationRatio;
            }

            // Verify the optional iteration delay arg, or if it's missing, set a default.
            if (args.Length == 5)
            {
                InitialIterationDelayMs = ushort.Parse(args[4]);
            }
            else
            {
                InitialIterationDelayMs = 0; // Default value in milliseconds
            }

            printer.PrintLine($"Grid:            {Width} columns x {Height} rows ({Width * Height:#,##0} cells)");
            printer.PrintLine($"Population:      {InitialPopulationRatio}%");
            printer.PrintLine($"Iteration delay: {InitialIterationDelayMs}ms");
        }
    }
}