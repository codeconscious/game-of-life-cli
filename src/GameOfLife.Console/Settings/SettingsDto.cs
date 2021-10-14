using System.IO;
using System.Text.Json;

namespace GameOfLife;

public record SettingsDto : IGridSettings
{
    public bool UseHighResMode { get; set; }
    public int MinimumWidthHeight { get; init; }
    public int Width { get; init; }
    public int Height { get; init; }
    public int InitialPopulationRatio { get; init; }
    public ushort InitialIterationDelayMs { get; init; }

    // public SettingsDto(bool useHighRes = false)
    // {
    //     UseHighResMode = useHighRes;
    // }
}

public class SettingsService
{
    public void Serialize()
    {
        var dto = new SettingsDto();

        var options = new JsonSerializerOptions { WriteIndented = true };
        var jsonString = JsonSerializer.Serialize(dto, options);
        WriteLine(jsonString);
    }

    public SettingsDto? Deserialize(string settingsPath)
    {
        ArgumentNullException.ThrowIfNull(settingsPath);

        if (!File.Exists(settingsPath))
            throw new FileNotFoundException("Settings file missing.", settingsPath);

        return JsonSerializer.Deserialize<SettingsDto>(settingsPath);
    }
}