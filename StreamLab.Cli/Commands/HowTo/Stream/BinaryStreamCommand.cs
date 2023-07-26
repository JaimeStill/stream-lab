using System.CommandLine;
using StreamLab.Common.Cli;

namespace StreamLab.Cli.Commands;
public class BinaryStreamCommand : CliCommand
{
    public BinaryStreamCommand() : base(
        "binarystream",
        "Use BinaryReader and BinaryWriter to read and write data",
        new Func<string, Task>(Call),
        options: new()
        {
            new Option<string>(
                new string[] { "--filename", "--file", "-f" },
                description: "File to read from",
                getDefaultValue: () => "../resources/howto/stream/binary.data"
            )
        }
    )
    { }

    static async Task Call(string filename)
    {
        FileInfo file = new(filename);

        if (file.Exists)
            file.Delete();
            
        await WriteBinary(filename);
        await ReadBinary(filename);
    }

    static async Task WriteBinary(string file) => await Task.Run(() =>
    {
        using FileStream fs = new(file, FileMode.CreateNew);
        using BinaryWriter writer = new(fs);

        for (int i = 0; i < 11; i++)
            writer.Write(i);
    });

    static async Task ReadBinary(string file) => await Task.Run(() =>
    {
        using FileStream fs = new(file, FileMode.Open, FileAccess.Read);
        using BinaryReader reader = new(fs);

        while (reader.PeekChar() > -1)
            Console.WriteLine(reader.ReadInt32());
    });
}