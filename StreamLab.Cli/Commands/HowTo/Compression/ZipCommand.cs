using System.CommandLine;
using System.IO.Compression;
using StreamLab.Common.Cli;

namespace StreamLab.Cli.Commands;
public class ZipCommand : CliCommand
{
    public ZipCommand() : base(
        "zip",
        "Use ZipFile to create and extract a compressed directory",
        new Func<string, string, string, Task>(Call),
        options: new()
        {
            new Option<string>(
                new string[] { "--source", "--src", "-s" },
                description: "Source directory to compress",
                getDefaultValue: () => "../resources/compress/"
            ),
            new Option<string>(
                new string[] { "--destination", "--dest", "-d" },
                description: "Destination extraction directory",
                getDefaultValue: () => "../resources/result/"
            ),
            new Option<string>(
                new string[] { "--zip", "-z" },
                description: "Compressed file name",
                getDefaultValue: () => "../resources/result.zip"
            )
        }
    )
    { }

    static async Task Call(string source, string destination, string zip) => await Task.Run(() =>
    {
        DirectoryInfo srcInfo = new(source);
        DirectoryInfo destInfo = new(destination);
        FileInfo zipInfo = new(zip);

        if (!srcInfo.Exists)
            throw new DirectoryNotFoundException($"Directory not found: {source}");

        if (destInfo.Exists)
            destInfo.Delete(true);

        if (zipInfo.Exists)
            zipInfo.Delete();

        Console.WriteLine($"Compressing {source} to {zip}");
        ZipFile.CreateFromDirectory(source, zip);

        Console.WriteLine($"Extracting {zip} to {destination}");
        ZipFile.ExtractToDirectory(zip, destination);

        Console.WriteLine("Compression operations successfully completed");
    });
}