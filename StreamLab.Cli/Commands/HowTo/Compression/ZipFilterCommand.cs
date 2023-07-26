using System.CommandLine;
using System.IO.Compression;
using StreamLab.Common.Cli;

namespace StreamLab.Cli.Commands;
public class ZipFilterCommand : CliCommand
{
    public ZipFilterCommand() : base(
        "zipfilter",
        "Extract files with a given extension from an archive",
        new Func<string, string, string, string, Task>(Call),
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
            ),
            new Option<string>(
                new string[] { "--extension", "--ext", "-e" },
                description: "File extension to filter",
                getDefaultValue: () => ".jpg"
            )
        }
    )
    { }

    static async Task Call(string source, string destination, string zip, string extension) => await Task.Run(() =>
    {
        string zipPath = Path.Join(destination, zip);
        DirectoryInfo srcInfo = new(source);
        DirectoryInfo destInfo = new(destination);
        FileInfo zipInfo = new(zipPath);

        if (!srcInfo.Exists)
            throw new DirectoryNotFoundException($"Directory not found: {source}");

        if (destInfo.Exists)
            destInfo.Delete(true);

        if (zipInfo.Exists)
            zipInfo.Delete();

        destInfo.Create();

        Console.WriteLine($"Compressing {source} to {zipPath}");
        ZipFile.CreateFromDirectory(source, zipPath);

        using ZipArchive archive = ZipFile.OpenRead(zipPath);

        foreach (ZipArchiveEntry entry in archive.Entries.Where(x => x.FullName.EndsWith(extension, StringComparison.OrdinalIgnoreCase)))
        {
            Console.WriteLine($"Extracting {entry.FullName} to {destination}");
            entry.ExtractToFile(Path.Combine(destination, entry.FullName), true);
        }

        Console.WriteLine("Compression operations successfully completed");
    });
}