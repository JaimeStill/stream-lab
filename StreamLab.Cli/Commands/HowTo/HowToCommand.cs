using StreamLab.Common.Cli;

namespace StreamLab.Cli.Commands;
public class HowToCommand : CliCommand
{
    public HowToCommand() : base(
        "howto",
        "IO examples inspired by Microsoft docs",
        commands: new()
        {
            new AppendStreamTextCommand(),
            new CopyDirectoriesCommand(),
            new CopyFileStreamCommand(),
            new CopyStreamTextCommand(),
            new ReadStreamTextCommand(),
            new WriteStreamTextCommand()
        }
    ) { }
}