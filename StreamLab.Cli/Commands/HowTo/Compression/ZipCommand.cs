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
                getDefaultValue: () => "../resources/howto/compress/"
            ),
            new Option<string>(
                new string[] { "--destination", "--dest", "-d" },
                description: "Destination extraction directory",
                getDefaultValue: () => "../resources/howto/result/"
            ),
            new Option<string>(
                new string[] { "--zip", "-z" },
                description: "Compressed file name",
                getDefaultValue: () => "result.zip"
            )
        }
    )
    { }

    static async Task Call(string source, string destination, string zip) => await Task.Run(() =>
    {
        string zipPath = Path.Join(destination, zip);
        DirectoryInfo srcInfo = new(source);
        DirectoryInfo destInfo = new(destination);
        FileInfo zipInfo = new(zipPath);

        if (!srcInfo.Exists)
            throw new DirectoryNotFoundException($"Directory not found: {source}");

        if (zipInfo.Exists)
            zipInfo.Delete();

        if (destInfo.Exists)
            destInfo.Delete(true);

        destInfo.Create();

        Console.WriteLine($"Compressing {source} to {zipPath}");
        ZipFile.CreateFromDirectory(source, zipPath);

        Console.WriteLine($"Extracting {zipPath} to {destination}");
        ZipFile.ExtractToDirectory(zipPath, destination);

        Console.WriteLine("Compression operations successfully completed");
    });
}