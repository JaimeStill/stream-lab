using StreamLab.Common.Cli;

namespace StreamLab.Cli.Commands;
public class HowToCommand : CliCommand
{
    public HowToCommand() : base(
        "howto",
        "IO examples inspired by Microsoft docs",
        commands: new()
        {
            new CompressionCommand(),
            new FileSystemCommand()
        }
    ) { }
}