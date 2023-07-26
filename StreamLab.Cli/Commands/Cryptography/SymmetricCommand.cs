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
        
        FileInfo encrypted = await Encrypt(f, dest);
        await Decrypt(encrypted, dest, Path.GetExtension(file));
    }

    static async Task<FileInfo> Encrypt(FileInfo file, DirectoryInfo target)
    {
        string filename = $"{Path.GetFileNameWithoutExtension(file.FullName)}.aes";
        string path = Path.Join(target.FullName, filename);

        using FileStream fs = new(file.FullName, FileMode.Open);
        byte[] fileBinaries = new byte[fs.Length];
        await fs.ReadAsync(fileBinaries, 0, fileBinaries.Length);

        using FileStream output = new(path, FileMode.Create);

        using Aes aes = Aes.Create();
        aes.Key = key;

        await output.WriteAsync(aes.IV, 0, aes.IV.Length);
        
        using CryptoStream crypto = new(
            output,
            aes.CreateEncryptor(),
            CryptoStreamMode.Write
        );

        await crypto.WriteAsync(fileBinaries, 0, fileBinaries.Length);
        return new(path);
    }

    static async Task Decrypt(FileInfo encrypted, DirectoryInfo target, string ext)
    {
        string filename = $"{Path.GetFileNameWithoutExtension(encrypted.FullName)}{ext}";
        string path = Path.Join(target.FullName, filename);

        using FileStream fs = new(encrypted.FullName, FileMode.Open);
        using FileStream output = new(path, FileMode.Create);

        using Aes aes = Aes.Create();
        byte[] iv = new byte[aes.IV.Length];
        int numBytesToRead = aes.IV.Length;
        int numBytesRead = 0;

        while (numBytesToRead > 0)
        {
            int n = await fs.ReadAsync(iv, numBytesRead, numBytesToRead);
            if (n == 0) break;

            numBytesRead += n;
            numBytesToRead -= n;
        }

        using CryptoStream crypt = new(
            fs,
            aes.CreateDecryptor(key, iv),
            CryptoStreamMode.Read
        );
        
        byte[] data = new byte[fs.Length - iv.Length];
        await crypt.ReadAsync(data);

        await output.WriteAsync(data, 0, data.Length);
    }
}