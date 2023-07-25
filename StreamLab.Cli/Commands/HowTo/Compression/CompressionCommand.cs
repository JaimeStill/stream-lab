using StreamLab.Common.Cli;

namespace StreamLab.Cli.Commands;
public class CompressionCommand : CliCommand
{
    public CompressionCommand() : base(
        "compression",
        "Compression stream examples inspired by Microsoft docs",
        commands: new()
        {
            new TarGZipCommand(),
            new ZipCommand(),
            new ZipAddCommand(),
            new ZipFilterCommand()
        }
    ) { }
}