using System.CommandLine;
using StreamLab.Common.Cli;

namespace StreamLab.Cli.Commands;
public class CopyDirectoriesCommand : CliCommand
{
    public CopyDirectoriesCommand() : base(
        "copydirectories",
        "Use I/O classes to copy the contents of a directory to another location",
        new Func<string, string, bool, Task>(Call),
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
            ),
            new Option<bool>(
                new string[] { "--recurse", "-r" },
                description: "Copy full sub-directory hierarchy"
            )
        }
    )
    { }

    static async Task Call(string source, string destination, bool recurse)
    {
        DirectoryInfo dest = new(destination);

        if (dest.Exists)
            dest.Delete(true);

        Console.WriteLine($"Copying {source} to {destination}");
        await CopyDirectory(source, destination, recurse);
        Console.WriteLine("Copying completed successfully");
    }

    static async Task CopyDirectory(string source, string destination, bool recurse)
    {
        DirectoryInfo src = new(source);

        if (!src.Exists)
            throw new DirectoryNotFoundException($"Source directory not found: {src.FullName}");

        DirectoryInfo[] dirs = src.GetDirectories();
        Directory.CreateDirectory(destination);

        foreach (FileInfo file in src.GetFiles())
        {
            Console.WriteLine($"Copying {file.Name} to {destination}");
            string targetFile = Path.Combine(destination, file.Name);
            file.CopyTo(targetFile, true);
        }

        if (recurse)
        {
            foreach (DirectoryInfo sub in dirs)
            {
                Console.WriteLine($"Copying {sub.Name} to {destination}");
                string dir = Path.Combine(destination, sub.Name);
                await CopyDirectory(sub.FullName, dir, true);
            }
        }
    }
}