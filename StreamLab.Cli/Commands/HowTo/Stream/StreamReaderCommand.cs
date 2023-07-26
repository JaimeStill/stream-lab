using System.CommandLine;
using StreamLab.Common.Cli;

namespace StreamLab.Cli.Commands;
public class StreamReaderCommand : CliCommand
{
    public StreamReaderCommand() : base(
        "streamreader",
        "Use StreamReader to read data",
        new Func<string, Task>(Call),
        options: new()
        {
            new Option<string>(
                new string[] { "--filename", "--file", "-f" },
                description: "File to read from",
                getDefaultValue: () => "../resources/howto/stream/data.json"
            )
        }
    )
    { }

    static async Task Call(string filename)
    {
        FileInfo file = new(filename);

        if (!file.Exists)
            throw new FileNotFoundException($"File was not found: {filename}");

        using FileStream fs = new(filename, FileMode.Open, FileAccess.Read, FileShare.Read);
        using StreamReader reader = new(fs);

        string input;

        while (reader.Peek() > -1)
        {
            input = await reader.ReadLineAsync() ?? string.Empty;
            Console.WriteLine(input);
        }
    }
}