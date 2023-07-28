using StreamLab.Common.Cli;

namespace StreamLab.Cli.Commands;
public class ExperimentCommand : CliCommand
{
    public ExperimentCommand() : base(
        "experiment",
        "Test out ideas and discover the details of APIs",
        commands: new()
        {
            new EncodingCommand()
        }
    ) { }
}