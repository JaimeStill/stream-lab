using System.Security.Cryptography;
using StreamLab.Common.Cli;

namespace StreamLab.Cli.Commands;
public class CryptoStreamCommand : CliCommand
{
    public CryptoStreamCommand() : base(
        "cryptostream",
        "Demonstrate using CryptoStream",
        new Func<Task>(Call)
    )
    { }

    static async Task Call()
    {
        string original = "It's time you realized that you have something in you more powerful and miraculous than the things that affect you and make you dance like a puppet. - Marcus Aurelius";

        using Aes aes = Aes.Create();
        byte[] encrypted = await EncryptStringToBytes(original, aes.Key, aes.IV);
        string decrypted = await DecryptStringFromBytes(encrypted, aes.Key, aes.IV);

        Console.WriteLine($"Decrypted: {decrypted}");
    }

    static async Task<byte[]> EncryptStringToBytes(string plaintext, byte[] key, byte[] iv)
    {
        if (string.IsNullOrWhiteSpace(plaintext))
            throw new ArgumentNullException(nameof(plaintext));

        if (key.Length <= 0)
            throw new ArgumentNullException(nameof(key));

        if (iv.Length <= 0)
            throw new ArgumentNullException(nameof(iv));

        byte[] encrypted;

        using (Aes aes = Aes.Create())
        {
            aes.Key = key;
            aes.IV = iv;

            ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

            using MemoryStream ram = new();
            using CryptoStream crypt = new(ram, encryptor, CryptoStreamMode.Write);
            using (StreamWriter writer = new(crypt))
            {
                await writer.WriteAsync(plaintext);
            }

            encrypted = ram.ToArray();
        }

        return encrypted;
    }

    static async Task<string> DecryptStringFromBytes(byte[] cipher, byte[] key, byte[] iv)
    {
        if (cipher.Length <= 0)
            throw new ArgumentNullException(nameof(cipher));

        if (key.Length <= 0)
            throw new ArgumentNullException(nameof(key));

        if (iv.Length <= 0)
            throw new ArgumentNullException(nameof(iv));

        string plaintext = string.Empty;

        using (Aes aes = Aes.Create())
        {
            aes.Key = key;
            aes.IV = iv;

            ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

            using MemoryStream ram = new(cipher);
            using CryptoStream crypt = new(ram, decryptor, CryptoStreamMode.Read);
            using StreamReader reader = new(crypt);

            plaintext = await reader.ReadToEndAsync();
        }

        return plaintext;
    }
}