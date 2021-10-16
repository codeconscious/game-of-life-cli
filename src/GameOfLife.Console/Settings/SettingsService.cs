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
                        < -1 => ValidationResult.Error("[red]That's an invalid width.[/]"),
                        _ => ValidationResult.Success(),
                    };
                }));

        var height = AnsiConsole.Prompt(
            new TextPrompt<short>("Grid height? (Enter [green]-1[/] for screen height.)")
                .Validate(height =>
                {
                    return height switch
                    {
                        < -1 => ValidationResult.Error("[red]That's an invalid height.[/]"),
                        _ => ValidationResult.Success(),
                    };
                }));

        var ratio = AnsiConsole.Prompt(
            new TextPrompt<byte>("Population ratio? ([green]0-99[/], or else [green]-1[/] for a random one.)")
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
            new TextPrompt<ushort>("Iteration delay in ms? (Enter 0 or higher.)"));

        return new SettingsDto(useHighResMode, width, height, ratio, delay);
    }

    public void SaveToFile(IGridSettings settings, string fileName, IPrinter printer)
    {
        ArgumentNullException.ThrowIfNull(settings);
        ArgumentNullException.ThrowIfNull(fileName);
        ArgumentNullException.ThrowIfNull(printer);

        if (File.Exists(fileName))
        {
            printer.PrintLine($"The file \"{fileName}\" already exists. Overwrite it?");
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

        // printer.PrintLine(jsonString);
        File.WriteAllText(fileName, jsonString, System.Text.Encoding.UTF8);

        printer.PrintLine("Settings saved to file.");
    }

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