using System.CommandLine;
using System.Security.Cryptography;
using StreamLab.Common.Cli;

namespace StreamLab.Cli.Commands;
public class SymmetricCommand : CliCommand
{
    public SymmetricCommand() : base(
        "symmetric",
        "Encrypt and Decrypt using AES symmetric encryption algorithm",
        new Func<string, string, Task>(Call),
        options: new()
        {
            new Option<string>(
                new string[] { "--file", "-f" },
                description: "File to encrypt / decrypt",
                getDefaultValue: () => "../resources/cryptography/data.json"
            ),
            new Option<string>(
                new string[] { "--destination", "--dest", "-d" },
                description: "Location to store encrypted / decrypted files",
                getDefaultValue: () => "../resources/cryptography/result/"
            )
        }
    )
    { }

    static readonly byte[] key =
    {
        0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08,
        0x09, 0x10, 0x11, 0x12, 0x13, 0x14, 0x15, 0x16
    };

    static async Task Call(string file, string destination)
    {
        FileInfo f = new(file);
        DirectoryInfo dest = new(destination);

        if (!f.Exists)
            throw new FileNotFoundException($"File was not found: {file}");

        if (dest.Exists)
            dest.Delete(true);

        dest.Create();
        
        Console.WriteLine($"Encrypting {f.FullName} to {dest.FullName}");
        FileInfo encrypted = new(await Encrypt(f, dest));
        Console.WriteLine($"Successfully encrypted to {encrypted.FullName}");

        Console.WriteLine($"Decrypting {encrypted.FullName} to {dest.FullName}");
        FileInfo decrypted = new(await Decrypt(encrypted, dest, Path.GetExtension(file)));
        Console.WriteLine($"Encrytped data {encrypted.FullName} was successfully decrypted to {decrypted.FullName}");
    }

    static async Task<string> Encrypt(FileInfo file, DirectoryInfo target)
    {
        string filename = $"{Path.GetFileNameWithoutExtension(file.FullName)}.aes";
        string path = Path.Join(target.FullName, filename);

        using FileStream source = new(file.FullName, FileMode.Open);
        byte[] sourceData = new byte[source.Length];
        await source.ReadAsync(sourceData, 0, sourceData.Length);

        using Aes aes = Aes.Create();
        aes.Key = key;

        using FileStream output = new(path, FileMode.Create);
        await output.WriteAsync(aes.IV, 0, aes.IV.Length);
        
        using CryptoStream crypto = new(
            output,
            aes.CreateEncryptor(),
            CryptoStreamMode.Write
        );

        await crypto.WriteAsync(sourceData, 0, sourceData.Length);
        await crypto.FlushFinalBlockAsync();

        return path;
    }

    static async Task<string> Decrypt(FileInfo encrypted, DirectoryInfo target, string ext)
    {
        string filename = $"{Path.GetFileNameWithoutExtension(encrypted.FullName)}{ext}";
        string path = Path.Join(target.FullName, filename);

        using FileStream source = new(encrypted.FullName, FileMode.Open);
        using Aes aes = Aes.Create();

        byte[] iv = new byte[aes.IV.Length];
        await source.ReadAsync(iv, 0, iv.Length);

        byte[] sourceData = new byte[source.Length - iv.Length];
        await source.ReadAsync(sourceData, 0, sourceData.Length);

        using FileStream output = new(path, FileMode.Create);

        using CryptoStream crypt = new(
            output,
            aes.CreateDecryptor(key, iv),
            CryptoStreamMode.Write
        );

        await crypt.WriteAsync(sourceData, 0, sourceData.Length);
        await crypt.FlushFinalBlockAsync();

        return path;
    }
}