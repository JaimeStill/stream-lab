using System.CommandLine;
using System.IO.Compression;
using StreamLab.Common.Cli;

namespace StreamLab.Cli.Commands;
public class ZipAddCommand : CliCommand
{
    public ZipAddCommand() : base(
        "zipadd",
        "Add a file to a compressed archive",
        new Func<string, string, string, string, Task>(Call),
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
            ),
            new Option<string>(
                new string[] { "--file", "-f" },
                description: "File to add to archive",
                getDefaultValue: () => "../resources/source/images/galaxy.jpg"
            )
        }
    )
    { }

    static async Task Call(string source, string destination, string zip, string file)
    {
        DirectoryInfo srcInfo = new(source);
        DirectoryInfo destInfo = new(destination);
        FileInfo zipInfo = new(zip);
        FileInfo fileInfo = new(file);

        if (!srcInfo.Exists)
            throw new DirectoryNotFoundException($"Directory not found: {source}");

        if (destInfo.Exists)
            destInfo.Delete(true);

        if (zipInfo.Exists)
            zipInfo.Delete();

        destInfo.Create();

        Console.WriteLine($"Compressing {source} to {zip}");
        ZipFile.CreateFromDirectory(source, zip);

        using FileStream zipStream = new(zip, FileMode.Open);
        using ZipArchive archive = new(zipStream, ZipArchiveMode.Update);

        Console.WriteLine($"Adding {fileInfo.Name} to {zip}");
        archive.CreateEntryFromFile(file, fileInfo.Name);

        archive.Dispose();
        await zipStream.DisposeAsync();

        Console.WriteLine($"Extracting {zip} to {destination}");
        ZipFile.ExtractToDirectory(zip, destination);

        Console.WriteLine("Compression operations successfully completed");
    }
}