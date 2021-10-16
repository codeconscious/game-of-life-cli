namespace GameOfLife;

/// <summary>
/// A setting DTO for creating a Settings object or saving settings.
/// </summary>
public record SettingsDto(
    bool UseHighResMode,
    int Width,
    int Height,
    int InitialPopulationRatio,
    ushort InitialIterationDelayMs) : IGridSettings;