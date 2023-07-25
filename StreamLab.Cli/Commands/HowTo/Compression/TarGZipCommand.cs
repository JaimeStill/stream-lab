using System.CommandLine;
using System.Formats.Tar;
using System.IO.Compression;
using StreamLab.Common.Cli;

namespace StreamLab.Cli.Commands;
public class TarGZipCommand : CliCommand
{
    public TarGZipCommand() : base(
        "targzip",
        "Use GZipStream to compress and decompress .gz files",
        new Func<string, string, Task>(Call),
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
            )
        }
    )
    { }

    static async Task Call(string source, string destination)
    {
        DirectoryInfo src = new(source);
        DirectoryInfo dest = new(destination);

        if (!src.Exists)
            throw new DirectoryNotFoundException($"Directory not found: {source}");

        if (dest.Exists)
            dest.Delete(true);

        dest.Create();

        /*
            Efficient way of transforming streams in memory
            and only writing the final resulting files
        */

        Console.WriteLine($"Compressing {src.FullName} to {dest.FullName}{src.Name}.tar.gz");
        FileInfo gzip = await Compress(src, dest);
        Console.WriteLine($"{dest.FullName}{src.Name}.tar.gz successfully created");

        Console.WriteLine($"Decompressing {dest.FullName}{src.Name}.tar.gz to {dest.FullName}{src.Name}");
        await Decompress(gzip, dest);
        Console.WriteLine($"{dest.FullName}{src.Name}.tar.gz successfully extracted to {dest.FullName}{src.Name}");

        /*
            Inefficient way of writing / deleting each
            stream at each step of the process
        */

        // Console.WriteLine($"Archiving {source} to {destination}");
        // FileInfo tar = await CreateTar(src, dest);
        // Console.WriteLine($"Archive created at {tar.FullName}");

        // Console.WriteLine($"GZipping {tar.Name} to {destination}");
        // FileInfo gzip = await CreateGZip(tar, dest);
        // Console.WriteLine($"GZip created at {gzip.FullName}");

        // Console.WriteLine($"Unpacking GZip {gzip.FullName} to {destination}");
        // tar = await UnpackGZip(gzip, dest);
        // Console.WriteLine($"{tar.FullName} successfully unpacked");

        // Console.WriteLine($"Extracting archive {tar.FullName} to {destination}");
        // await TarFile.ExtractToDirectoryAsync(tar.FullName, destination, true);
        // tar.Delete();
        // Console.WriteLine($"Archive successfully extracted to {destination}");
    }

    static async Task<FileInfo> Compress(DirectoryInfo source, DirectoryInfo target)
    {
        string path = Path.Join(target.FullName, $"{source.Name}.tar.gz");

        using (MemoryStream ram = new())
        {
            await TarFile.CreateFromDirectoryAsync(source.FullName, ram, true);
            ram.Position = 0;
            using FileStream gzip = File.Create(path);
            using GZipStream gzipStream = new(gzip, CompressionMode.Compress);
            await ram.CopyToAsync(gzipStream);
        }

        return new(path);
    }

    static async Task Decompress(FileInfo gzip, DirectoryInfo target)
    {
        using FileStream fs = gzip.OpenRead();
        using GZipStream gzipStream = new(fs, CompressionMode.Decompress);

        using MemoryStream ram = new();
        await gzipStream.CopyToAsync(ram);
        ram.Position = 0;
        await TarFile.ExtractToDirectoryAsync(ram, target.FullName, true);
    }

    static async Task<FileInfo> CreateTar(DirectoryInfo source, DirectoryInfo target)
    {
        string tarPath = Path.Join(target.FullName, $"{source.Name}.tar");
        using FileStream tar = File.Create(tarPath);
        await TarFile.CreateFromDirectoryAsync(source.FullName, tar, true);
        return new(tarPath);
    }

    static async Task<FileInfo> CreateGZip(FileInfo tar, DirectoryInfo target)
    {
        string gzipPath = Path.Join(target.FullName, $"{tar.Name}.gz");
        using FileStream fs = tar.OpenRead();
        using FileStream gzip = File.Create(gzipPath);
        using GZipStream gzipStream = new(gzip, CompressionMode.Compress);
        await fs.CopyToAsync(gzipStream);

        tar.Delete();
        return new(gzipPath);
    }

    static async Task<FileInfo> UnpackGZip(FileInfo gzip, DirectoryInfo target)
    {
        string tarPath = Path.Join(target.FullName, gzip.Name[..^3]); // remove .gz from end of name
        using FileStream fs = gzip.OpenRead();
        using FileStream tar = File.Create(tarPath);
        using GZipStream gzipStream = new(fs, CompressionMode.Decompress);
        await gzipStream.CopyToAsync(tar);

        gzip.Delete();
        return new(tarPath);
    }
}