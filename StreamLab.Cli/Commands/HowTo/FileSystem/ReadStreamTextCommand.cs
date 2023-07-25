using System.CommandLine;
using StreamLab.Common.Cli;

namespace StreamLab.Cli.Commands;
public class ReadStreamTextCommand : CliCommand
{
    public ReadStreamTextCommand() : base(
        "readstreamtext",
        "Use StreamReader to read a text file",
        new Func<string, Task>(Call),
        options: new()
        {
            new Option<string>(
                new string[] { "--source", "--src", "-s" },
                description: "Source text file",
                getDefaultValue: () => "../resources/source/readme.md"
            )
        }
    )
    { }

    static async Task Call(string source)
    {
        FileInfo file = new(source);

        if (!file.Exists)
            throw new FileNotFoundException($"File was not found: {source}");
            
        using StreamReader reader = new(source);
        Console.WriteLine(await reader.ReadToEndAsync());
    }
}