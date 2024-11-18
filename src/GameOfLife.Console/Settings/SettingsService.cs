using System.IO;
using System.Text.Json;
using Spectre.Console;

namespace GameOfLife.Console.Settings;

public class SettingsService
{
    public SettingsDto CreateSettingsFromUserInput()
    {
        AnsiConsole.WriteLine("Enter your settings below. Press the Enter key to submit the default option.");

        bool useHighResMode = AnsiConsole.Confirm("Use high-res mode?");

        short width = AnsiConsole.Prompt(
            new TextPrompt<short>("Grid width? (Enter [yellow]-1[/] for screen width.)")
                .DefaultValue<short>(-1)
                .Validate(width =>
                {
                    return width switch
                    {
                        < -1 => ValidationResult.Error("[red]Invalid width.[/]"),
                        _ => ValidationResult.Success(),
                    };
                }));

        short height = AnsiConsole.Prompt(
            new TextPrompt<short>("Grid height? (Enter [yellow]-1[/] for screen height.)")
                .DefaultValue<short>(-1)
                .Validate(height =>
                {
                    return height switch
                    {
                        < -1 => ValidationResult.Error("[red]Invalid height.[/]"),
                        _ => ValidationResult.Success(),
                    };
                }));

        sbyte ratio = AnsiConsole.Prompt(
            new TextPrompt<sbyte>("Population ratio %? ([yellow]0-99[/], or else [yellow]-1[/] for a random one.)")
                .DefaultValue<sbyte>(-1)
                .Validate(ratio =>
                {
                    return ratio switch
                    {
                        0 => ValidationResult.Error("[red]That's too low.[/]"),
                        >= 100 => ValidationResult.Error("[red]That's too high.[/]"),
                        _ => ValidationResult.Success(),
                    };
                }));

        ushort delay = AnsiConsole.Prompt(
            new TextPrompt<ushort>("Iteration delay in ms? (Enter [yellow]0[/] or higher.)")
                .DefaultValue<ushort>(0));

        return new SettingsDto(useHighResMode, width, height, ratio, delay);
    }

    public void SaveToFile(IGridSettings settings, string fileName, IPrinter printer)
    {
        ArgumentNullException.ThrowIfNull(settings);
        ArgumentNullException.ThrowIfNull(fileName);
        ArgumentNullException.ThrowIfNull(printer);

        JsonSerializerOptions options = new() { WriteIndented = true }; // TODO: Reuse instance.
        var json = JsonSerializer.Serialize(settings, options);

        File.WriteAllText(fileName, json, System.Text.Encoding.UTF8);

        AnsiConsole.WriteLine("Settings saved.");

        PrintSettings(settings);
    }

    private void PrintSettings(IGridSettings settings)
    {
        ArgumentNullException.ThrowIfNull(settings);

        Table table = new();
        table.Border(TableBorder.Rounded);

        table.AddColumn("Setting");
        table.AddColumn("Value");

        table.AddRow("High-res mode", settings.UseHighResMode ? "On" : "Off");
        table.AddRow("Width", settings.Width == -1 ? "Fit" : settings.Width + " rows");
        table.AddRow("Height", settings.Height == -1 ? "Fit" : settings.Height + " columns");
        table.AddRow("Population", settings.PopulationRatio == -1 ? "Random" : settings.PopulationRatio + "%");
        table.AddRow("Iteration delay", settings.IterationDelayMs + "ms");

        AnsiConsole.Write(table);
    }

    public SettingsDto? ReadFromFile(string settingsPath)
    {
        ArgumentNullException.ThrowIfNull(settingsPath);

        var json = File.ReadAllText(settingsPath);

        if (string.IsNullOrWhiteSpace(json))
            throw new InvalidDataException("The settings file was empty.");

        return JsonSerializer.Deserialize<SettingsDto>(json);
    }
}
