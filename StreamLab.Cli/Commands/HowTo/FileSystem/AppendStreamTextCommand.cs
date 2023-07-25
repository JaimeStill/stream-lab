using System.CommandLine;
using StreamLab.Common.Cli;

namespace StreamLab.Cli.Commands;
public class AppendStreamTextCommand : CliCommand
{
    public AppendStreamTextCommand() : base(
        "appendstreamtext",
        "Use StreamWriter to append text to a file",
        new Func<string, Task>(Call),
        options: new()
        {
            new Option<string>(
                new string[] { "--file", "-f" },
                description: "Destination text file",
                getDefaultValue: () => "../resources/destination/append-stream.txt"
            )
        }
    )
    { }

    static async Task Call(string file)
    {
        FileInfo info = new(file);
        
        if (info.Exists)
            info.Delete();

        string[] lines = {
            "First line",
            "Second line",
            "Third line"
        };

        using StreamWriter writer = new(file);

        foreach (string line in lines)
            await writer.WriteLineAsync(line);

        await writer.DisposeAsync();

        using StreamWriter appender = new(file, true);
        await appender.WriteLineAsync("Fourth line");
        await appender.DisposeAsync();

        using StreamReader reader = new(file);
        Console.WriteLine($"Contents of {file}:");
        Console.WriteLine(await reader.ReadToEndAsync());
    }
}