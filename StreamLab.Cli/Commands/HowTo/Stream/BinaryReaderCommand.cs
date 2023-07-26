using System.CommandLine;
using StreamLab.Common.Cli;

namespace StreamLab.Cli.Commands;
public class BinaryReaderCommand : CliCommand
{
    public BinaryReaderCommand() : base(
        "binaryreader",
        "Use BinaryReader to read data",
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

    static async Task Call(string filename) => await Task.Run(() =>
    {
        FileInfo file = new(filename);

        if (!file.Exists)
            throw new FileNotFoundException($"File was not found: {filename}");

        using FileStream fs = new(filename, FileMode.Open, FileAccess.Read, FileShare.Read);
        using BinaryReader reader = new(fs);

        byte input;

        while (reader.PeekChar() > -1)
        {
            input = reader.ReadByte();
            Console.WriteLine(input);
        }
    });
}