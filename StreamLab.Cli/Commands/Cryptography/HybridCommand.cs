using System.CommandLine;
using System.Security.Cryptography;
using StreamLab.Common.Cli;

namespace StreamLab.Cli.Commands;
public class HybridCommand : CliCommand
{
    // see https://en.wikipedia.org/wiki/Hybrid_cryptosystem
    public HybridCommand() : base(
        "hybrid",
        "Use AES to encrypt data, and RSA to encrypt the symmetric key",
        new Func<string, string, Task>(Call),
        options: new()
        {
            new Option<string>(
                new string[] { "--file", "-f" },
                description: "File to encrypt",
                getDefaultValue: () => "../resources/cryptography/galaxy.jpg"
            ),
            new Option<string>(
                new string[] { "--destination", "--dest", "-d" },
                description: "Directory to store keys and data",
                getDefaultValue: () => "../resources/cryptography/result/"
            )            
        }
    )  
    { }

    static async Task Call(string file, string destination)
    {
        (FileInfo f, DirectoryInfo dest) = Initialize(file, destination);

        string publicPath = Path.Join(destination, "key.pub");
        string privatePath = Path.Join(destination, "key.pem");
        string aesKeyPath = Path.Join(destination, "key.aes");
        string aesIvPath = Path.Join(destination, "iv.aes");
        
        Console.WriteLine($"Generating encryption keys");
        await GenerateKeys(publicPath, privatePath, aesKeyPath, aesIvPath);

        string rsaKey = await RetrieveKey(privatePath);

        Console.WriteLine($"Encrypting file {file} to {destination}");
        FileInfo encrypted = new(await Encrypt(f, dest, rsaKey, aesKeyPath, aesIvPath));
        Console.WriteLine($"Successfully encrypted to {encrypted.FullName}");

        Console.WriteLine($"Decrypting {encrypted.FullName} to {destination}");
        FileInfo decrypted = new(await Decrypt(encrypted, dest, Path.GetExtension(file), rsaKey, aesKeyPath, aesIvPath));
        Console.WriteLine($"Encrypted data {encrypted.FullName} was successfully decrypted to {decrypted.FullName}");
    }

    static (FileInfo f, DirectoryInfo dest) Initialize(string file, string destination)
    {
        FileInfo f = new(file);
        DirectoryInfo dest = new(destination);

        if (!f.Exists)
            throw new FileNotFoundException($"File was not found: {file}");

        if (dest.Exists)
            dest.Delete(true);

        dest.Create();

        return (
            f,
            dest
        );
    }

    static async Task GenerateKeys(string publicKey, string privateKey, string aesKey, string aesIv)
    {
        using RSA rsa = RSA.Create();
        using Aes aes = Aes.Create();

        await StoreKey(publicKey, rsa.ToXmlString(false));
        await StoreKey(privateKey, rsa.ToXmlString(true));
        await StoreKey(aesKey, Encrypt(rsa, aes.Key));
        await StoreKey(aesIv, Encrypt(rsa, aes.IV));
    }

    static byte[] Encrypt(RSA rsa, byte[] data) =>
        rsa.Encrypt(data, RSAEncryptionPadding.Pkcs1);

    static byte[] Decrypt(RSA rsa, byte[] data) =>
        rsa.Decrypt(data, RSAEncryptionPadding.Pkcs1);

    static async Task<string> Encrypt(FileInfo file, DirectoryInfo target, string rsaKey, string keyPath, string ivPath)
    {
        string filename = $"{Path.GetFileNameWithoutExtension(file.FullName)}.aes";
        string path = Path.Join(target.FullName, filename);

        (byte[] key, byte[] iv) = await LoadKeys(rsaKey, keyPath, ivPath);

        using Aes aes = LoadAes(key, iv);

        using FileStream source = new(file.FullName, FileMode.Open);
        byte[] sourceData = new byte[source.Length];
        await source.ReadAsync(sourceData, 0, sourceData.Length);

        using FileStream output = new(path, FileMode.Create);

        using CryptoStream crypto = new(
            output,
            aes.CreateEncryptor(),
            CryptoStreamMode.Write
        );

        await crypto.WriteAsync(sourceData, 0, sourceData.Length);
        await crypto.FlushFinalBlockAsync();

        return path;
    }

    static async Task<string> Decrypt(FileInfo encrypted, DirectoryInfo target, string ext, string rsaKey, string keyPath, string ivPath)
    {
        string filename = $"{Path.GetFileNameWithoutExtension(encrypted.FullName)}{ext}";
        string path = Path.Join(target.FullName, filename);

        (byte[] key, byte[] iv) = await LoadKeys(rsaKey, keyPath, ivPath);

        using Aes aes = LoadAes(key, iv);

        using FileStream source = new(encrypted.FullName, FileMode.Open);
        byte[] sourceData = new byte[source.Length];
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

    static RSA LoadRSA(string rsaKey)
    {
        RSA rsa = RSA.Create();
        rsa.FromXmlString(rsaKey);
        return rsa;
    }

    static Aes LoadAes(byte[] key, byte[] iv)
    {
        Aes aes = Aes.Create();
        aes.Key = key;
        aes.IV = iv;
        return aes;
    }

    static async Task<byte[]> RetrieveKey(string path, RSA rsa) => await Task.Run(() =>
    {
        Console.WriteLine($"Retrieving AES key {path}");
        using FileStream fs = new(path, FileMode.Open);
        using BinaryReader reader = new(fs);
        return Decrypt(rsa, reader.ReadBytes((int)fs.Length));
    });

    static async Task<string> RetrieveKey(string path)
    {
        Console.WriteLine($"Retrieving RSA key {path}");
        using FileStream fs = new(path, FileMode.Open);
        using StreamReader reader = new(fs);
        return await reader.ReadToEndAsync();
    }

    static async Task StoreKey(string path, string key)
    {
        Console.WriteLine($"Storing key at {path}");
        using FileStream fs = new(path, FileMode.Create);
        using StreamWriter writer = new(fs);
        await writer.WriteAsync(key);
    }

    static async Task StoreKey(string path, byte[] key) => await Task.Run(() =>
    {
        Console.WriteLine($"Storing key at {path}");
        using FileStream fs = new(path, FileMode.Create);
        using BinaryWriter writer = new(fs);
        writer.Write(key);
    });

    static async Task<(byte[] key, byte[] iv)> LoadKeys(string rsaKey, string keyPath, string ivPath)
    {
        using RSA rsa = LoadRSA(rsaKey);

        byte[] key = await RetrieveKey(keyPath, rsa);
        byte[] iv = await RetrieveKey(ivPath, rsa);

        return (
            key,
            iv
        );
    }
}