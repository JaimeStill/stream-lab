using System.Security.Cryptography;
using StreamLab.Common.Cli;

namespace StreamLab.Cli.Commands;
public class GenerateKeysCommand : CliCommand
{
    public GenerateKeysCommand() : base(
        "generatekeys",
        "Generate symmetric and asymmetric keys",
        new Action(Call)
    )
    { }

    static string Render(byte[] data) => string.Join(',', data);

    static void Call()
    {
        using Aes aes = Aes.Create();
        aes.GenerateKey();
        aes.GenerateIV();

        Console.WriteLine("AES Symmetric Keys:");
        Console.WriteLine($"Key: {Render(aes.Key)}");
        Console.WriteLine($"IV: {Render(aes.IV)}");
        Console.WriteLine();

        using RSA rsa = RSA.Create();
        RSAParameters rsaKey = rsa.ExportParameters(true);

        Console.WriteLine("RSA Asymmetric Key Info:");

        if (rsaKey.D is not null)
            Console.WriteLine($"D: {Render(rsaKey.D)}");

        if (rsaKey.P is not null)
            Console.WriteLine($"P: {Render(rsaKey.P)}");

        if (rsaKey.Q is not null)
            Console.WriteLine($"Q: {Render(rsaKey.Q)}");

        if (rsaKey.DP is not null)
            Console.WriteLine($"DP: {Render(rsaKey.DP)}");

        if (rsaKey.DQ is not null)
            Console.WriteLine($"DQ: {Render(rsaKey.DQ)}");

        if (rsaKey.Exponent is not null)
            Console.WriteLine($"Exponent: {Render(rsaKey.Exponent)}");

        if (rsaKey.InverseQ is not null)
            Console.WriteLine($"InverseQ: {Render(rsaKey.InverseQ)}");

        if (rsaKey.Modulus is not null)
            Console.WriteLine($"Modulus: {Render(rsaKey.Modulus)}");
    }
}