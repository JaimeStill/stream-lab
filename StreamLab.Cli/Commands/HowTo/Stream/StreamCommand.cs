using StreamLab.Common.Cli;

namespace StreamLab.Cli.Commands;
public class StreamCommand : CliCommand
{
    public StreamCommand() : base(
        "stream",
        "Stream examples inspired by Microsoft docs",
        commands: new()
        {
            new BinaryReaderCommand(),
            new BinaryStreamCommand(),
            new MemoryStreamCommand(),
            new StreamReaderCommand()
        }
    ) { }
}