using System.CommandLine;
using StreamLab.Common.Cli;

namespace StreamLab.Cli.Commands;
public class CopyStreamTextCommand : CliCommand
{
    public CopyStreamTextCommand() : base(
        "copystreamtext",
        "Use StreamReader and StreamWriter to copy a text file",
        new Func<string, string, Task>(Call),
        options: new()
        {
            new Option<string>(
                new string[] { "--source", "--src", "-s" },
                description: "Source text file",
                getDefaultValue: () => "../resources/source/readme.md"
            ),
            new Option<string>(
                new string[] { "--destination", "--dest", "-d" } ,
                description: "Destination text file",
                getDefaultValue: () => "../resources/destination/readme.md"
            )
        }
    )
    { }

    static async Task Call(string source, string destination)
    {
        FileInfo file = new(source);

        if (!file.Exists)
            throw new FileNotFoundException($"File was not found: {source}");

        Console.WriteLine($"Copying from {source} to {destination}");

        using StreamReader src = File.OpenText(source);
        using StreamWriter dest = File.CreateText(destination);
        char[] buffer = new char[0x1000];
        int numRead;
        while ((numRead = await src.ReadAsync(buffer, 0, buffer.Length)) != 0)
        {
            Console.WriteLine("Buffer:");
            Console.WriteLine(string.Join(string.Empty, buffer));
            Console.WriteLine($"Num Read: {numRead}");

            await dest.WriteAsync(buffer, 0, numRead);
        }

        Console.WriteLine("Copying completed successfully");
    }
}