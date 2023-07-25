using System.CommandLine;
using StreamLab.Common.Cli;

namespace StreamLab.Cli.Commands;
public class CopyFileStreamCommand : CliCommand
{
    public CopyFileStreamCommand() : base(
        "copyfilestream",
        "Use FileStream to copy files between directories",
        new Func<string, string, Task>(Call),
        options: new()
        {
            new Option<string>(
                new string[] { "--source", "--src", "-s" },
                description: "Source directory",
                getDefaultValue: () => "../resources/source/"
            ),
            new Option<string>(
                new string[] { "--destination", "--dest", "-d" } ,
                description: "Destination directory",
                getDefaultValue: () => "../resources/destination/"
            )
        }
    ) { }

    static async Task Call(string source, string destination)
    {
        DirectoryInfo dest = new(destination);

        if (dest.Exists)
            dest.Delete(true);

        dest.Create();

        Console.WriteLine($"Copying files from {source} to {destination}");

        foreach (string filename in Directory.EnumerateFiles(source))
        {
            FileInfo file = new(filename);

            Console.WriteLine($"Copying {file.Name} from {source} to {destination}");

            using FileStream start = File.Open(filename, FileMode.Open);
            using FileStream end = File.Create(Path.Combine(destination, file.Name));

            await start.CopyToAsync(end);
        }
        Console.WriteLine("Copying completed successfully");
    }
}