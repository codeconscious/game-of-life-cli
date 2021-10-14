using System.Text.Json;

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

        public byte MinimumWidthHeight { get; } = 3;

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

        public Settings(SettingsDto? dto, IPrinter printer)
        {
            ArgumentNullException.ThrowIfNull(dto);
            ArgumentNullException.ThrowIfNull(printer);

            UseHighResMode = dto.UseHighResMode;

            // Verify the width (X axis) arg
            if (dto.Width == -1)
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
                if (dto.Width < MinimumWidthHeight)
                    throw new ArgumentOutOfRangeException(nameof(dto.Width));

                Width = dto.Width;
            }

            // Verify the height (Y axis) arg
            if (dto.Height == -1)
            {
                // Leave room at the bottom of the screen for output (during and after the game).
                const int bottomMargin = 3;

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
                if (dto.Height < MinimumWidthHeight)
                    throw new ArgumentOutOfRangeException(nameof(dto.Height));

                Height = dto.Height;
            }

            // Verify the population ratio arg
            if (dto.InitialPopulationRatio == -1)
            {
                InitialPopulationRatio = new Random().Next(MaximumRandomPopulationRatio);
            }
            else
            {
                if (dto.InitialPopulationRatio > 100 || dto.InitialPopulationRatio < 1)
                    throw new ArgumentOutOfRangeException(nameof(dto.InitialPopulationRatio));

                InitialPopulationRatio = dto.InitialPopulationRatio;
            }

            InitialIterationDelayMs = dto.InitialIterationDelayMs;

            printer.PrintLine($"Grid:            {Width} columns x {Height} rows ({Width * Height:#,##0} cells)");
            printer.PrintLine($"Population:      {InitialPopulationRatio}%");
            printer.PrintLine($"Iteration delay: {InitialIterationDelayMs}ms");
        }
    }
}