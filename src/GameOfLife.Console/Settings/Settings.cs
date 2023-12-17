using System.Text.Json;

namespace GameOfLife
{
    /// <summary>
    /// The settings used to create a grid and affect elements of the game elsewhere.
    /// </summary>
    public class Settings : IGridSettings
    {
        public bool UseHighResMode { get; }
        public byte MinimumWidthHeight { get; } = 5;
        public int Width { get; }
        public int Height { get; }
        public int PopulationRatio { get; }
        public ushort IterationDelayMs { get; }

        /// <summary>
        /// The minimum population percentage allowed when setting it randomly.
        /// If it's too low, then the game will likely end very quickly.
        /// </summary>
        public const byte MinimumRandomPopulationRatio = 10;

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
            if (dto.Width < 0)
            {
                int autoWidth = Console.WindowWidth;

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
            if (dto.Height < 0)
            {
                // Leave room at the bottom of the screen for output (during and after the game).
                const int bottomMargin = 3;

                int autoHeight = Console.WindowHeight - bottomMargin;

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
            if (dto.PopulationRatio < 0)
            {
                PopulationRatio = new Random()
                    .Next(MinimumRandomPopulationRatio, MaximumRandomPopulationRatio);
            }
            else
            {
                if (dto.PopulationRatio >= 100 || dto.PopulationRatio < 1)
                    throw new ArgumentOutOfRangeException(nameof(dto.PopulationRatio));

                PopulationRatio = dto.PopulationRatio;
            }

            IterationDelayMs = dto.IterationDelayMs;

            printer.WriteLine($"Grid:            {Width} columns x {Height} rows ({Width * Height:#,##0} cells)");
            printer.WriteLine($"Population:      {PopulationRatio}%");
            printer.WriteLine($"Iteration delay: {IterationDelayMs}ms");
        }
    }
}
