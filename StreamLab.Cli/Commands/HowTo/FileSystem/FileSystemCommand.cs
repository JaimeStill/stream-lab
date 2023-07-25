using StreamLab.Common.Cli;

namespace StreamLab.Cli.Commands;
public class FileSystemCommand : CliCommand
{
    public FileSystemCommand() : base(
        "filesystem",
        "File System stream examples inspired by Microsoft docs",
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