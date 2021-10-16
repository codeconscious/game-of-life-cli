using Spectre.Console;
using System.IO;
using System.Text.Json;

namespace GameOfLife;

public class SettingsService
{
    public SettingsDto CreateSettingsFromUserInput()
    {
        var useHighResMode = AnsiConsole.Confirm("Use [yellow]high-res[/] mode?");

        var width = AnsiConsole.Prompt(
            new TextPrompt<short>("Grid width? (\"Enter [green]-1[/] for screen width.)")
                .Validate(width =>
                {
                    return width switch
                    {
                        < -1 => ValidationResult.Error("[red]Invalid width.[/]"),
                        _ => ValidationResult.Success(),
                    };
                }));

        var height = AnsiConsole.Prompt(
            new TextPrompt<short>("Grid height? (Enter [green]-1[/] for screen height.)")
                .Validate(height =>
                {
                    return height switch
                    {
                        < -1 => ValidationResult.Error("[red]Invalid height.[/]"),
                        _ => ValidationResult.Success(),
                    };
                }));

        var ratio = AnsiConsole.Prompt(
            new TextPrompt<byte>("Population ratio %? ([green]0-99[/], or else [green]-1[/] for a random one.)")
                .Validate(ratio =>
                {
                    return ratio switch
                    {
                        0 => ValidationResult.Error("[red]That's too low.[/]"),
                        >= 100 => ValidationResult.Error("[red]That's too high.[/]"),
                        _ => ValidationResult.Success(),
                    };
                }));

        var delay = AnsiConsole.Prompt(
            new TextPrompt<ushort>("Iteration delay (in ms)? Enter [green]0[/] or higher.)"));

        return new SettingsDto(useHighResMode, width, height, ratio, delay);
    }

    public void SaveToFile(IGridSettings settings, string fileName, IPrinter printer)
    {
        ArgumentNullException.ThrowIfNull(settings);
        ArgumentNullException.ThrowIfNull(fileName);
        ArgumentNullException.ThrowIfNull(printer);

        if (File.Exists(fileName))
        {
            printer.PrintLine($"The file \"{fileName}\" already exists. Overwrite it? (Y/N)");

            // TODO: This should be a loop, or else use Spectre.
            var key = Console.ReadKey();
            if (char.ToLowerInvariant(key.KeyChar) != 'y')
            {
                printer.PrintLine("Cancelled saving settings.");
                return;
            }

            printer.PrintLine();
        }

        var options = new JsonSerializerOptions { WriteIndented = true };
        var jsonString = JsonSerializer.Serialize(settings, options);

        File.WriteAllText(fileName, jsonString, System.Text.Encoding.UTF8);

        AnsiConsole.WriteLine("[green]Settings saved to file.[/]");

        // Display the requested settings.
        var table = new Table();
        table.AddColumn("Setting");
        table.AddColumn("Value");
        table.AddRow("High-res mode", settings.UseHighResMode ? "On" : "Off");
        table.AddRow("Width", settings.Width == -1 ? "Fit" : settings.Width.ToString() + " rows");
        table.AddRow("Height", settings.Height == -1 ? "Fit" : settings.Height.ToString() + " columns");
        table.AddRow("Population", settings.InitialPopulationRatio.ToString() + "%");
        table.AddRow("Iteration delay", settings.InitialIterationDelayMs.ToString() + "ms");
        AnsiConsole.Write(table);
    }

    public SettingsDto? GetFromFile(string settingsPath)
    {
        ArgumentNullException.ThrowIfNull(settingsPath);

        if (!File.Exists(settingsPath))
            throw new FileNotFoundException("Settings file missing.", settingsPath);

        var json = File.ReadAllText(settingsPath);

        if (string.IsNullOrWhiteSpace(json))
            throw new InvalidDataException("The file was empty.");

        return JsonSerializer.Deserialize<SettingsDto>(json);
    }
}