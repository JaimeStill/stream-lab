using System.CommandLine;
using System.Security.Cryptography;
using StreamLab.Common.Cli;

namespace StreamLab.Cli.Commands;

public class AsymmetricCommand : CliCommand
{
    public AsymmetricCommand() : base(
        "asymmetric",
        "Encrypt and Decrypt using RSA asymmetric encryption algorithm",
        new Func<string, Task>(Call),
        options: new()
        {
            new Option<string>(
                new string[] { "--destination", "--dest", "-d" },
                description: "Directory to store RSA keys",
                getDefaultValue: () => "../resources/cryptography/result/"
            )
        }
    )
    { }

    static string Render(byte[] data) => string.Join(',', data);

    static async Task Call(string destination)
    {
        string publicKey = Path.Join(destination, "key.pub");
        string privateKey = Path.Join(destination, "key.pem");

        DirectoryInfo dest = new(destination);

        if (dest.Exists)
            dest.Delete(true);

        dest.Create();

        using RSA rsa = RSA.Create(2048);

        Console.WriteLine($"Storing public key at {publicKey}");
        await StoreKey(publicKey, rsa.ToXmlString(false));

        Console.WriteLine($"Storing private key at {privateKey}");
        await StoreKey(privateKey, rsa.ToXmlString(true));

        Console.WriteLine($"Encrypting AES Key and IV");
        (byte[] ekey, byte[] eiv) = Encrypt(rsa);

        Console.WriteLine($"Decrypting AES Key and IV");
        (string key, string iv) = Decrypt(rsa, ekey, eiv);

        Console.WriteLine($"Key: {key}");
        Console.WriteLine($"IV: {iv}");
    }

    static async Task StoreKey(string path, string key)
    {
        using FileStream fs = new(path, FileMode.Create);
        using StreamWriter writer = new(fs);
        await writer.WriteAsync(key);
    }

    static (byte[] ekey, byte[] eiv) Encrypt(RSA rsa)
    {
        using Aes aes = Aes.Create();

        byte[] key = rsa.Encrypt(aes.Key, RSAEncryptionPadding.Pkcs1);
        byte[] iv = rsa.Encrypt(aes.IV, RSAEncryptionPadding.Pkcs1);

        return (
            key,
            iv
        );
    }

    static (string key, string iv) Decrypt(RSA rsa, byte[] ekey, byte[] eiv)
    {
        string key = Render(rsa.Decrypt(ekey, RSAEncryptionPadding.Pkcs1));
        string iv = Render(rsa.Decrypt(eiv, RSAEncryptionPadding.Pkcs1));

        return (
            key,
            iv
        );
    }
}