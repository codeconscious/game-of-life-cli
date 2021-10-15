using System.IO;
using System.Text.Json;

namespace GameOfLife;

// Represents the desired settings, either from user input or a settings file.
public record SettingsDto : IGridSettings
{
    public bool UseHighResMode { get; init; }
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
    // public void Serialize()
    // {
    //     var dto = new SettingsDto();

    //     var options = new JsonSerializerOptions { WriteIndented = true };
    //     var jsonString = JsonSerializer.Serialize(dto, options);
    //     WriteLine(jsonString);
    // }

    public SettingsDto? GetFromFile(string settingsPath)
    {
        ArgumentNullException.ThrowIfNull(settingsPath);

        if (!File.Exists(settingsPath))
            throw new FileNotFoundException("Settings file missing.", settingsPath);

        var json = System.IO.File.ReadAllText(settingsPath);

        if (string.IsNullOrWhiteSpace(json))
            throw new InvalidDataException("The file was empty.");

        return JsonSerializer.Deserialize<SettingsDto>(json);
    }
}