using StreamLab.Cli.Commands;
using StreamLab.Common.Cli;

await new CliApp(
    "Stream Lab",
    new()
    {
        new CryptographyCommand(),
        new DataProtectionCommand(),
        new HowToCommand()
    }
).InvokeAsync(args);