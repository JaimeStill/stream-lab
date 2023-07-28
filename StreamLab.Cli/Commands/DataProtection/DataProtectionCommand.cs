using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.DependencyInjection;
using StreamLab.Common.Cli;

namespace StreamLab.Cli.Commands;
public class DataProtectionCommand : CliCommand
{
    public DataProtectionCommand() : base(
        "dataprotection",
        "Explore ASP.NET Core Data Protection APIs",
        new Action(Call)
    )
    { }

    class DataVault
    {
        readonly IDataProtector protector;

        public DataVault(IDataProtectionProvider provider)
        {
            protector = provider.CreateProtector("StreamLab.DataVault.V1");
        }

        public void Execute()
        {
            Console.Write("Enter input: ");

            string? input;

            do
            {
                input = Console.ReadLine();
            }
            while (string.IsNullOrWhiteSpace(input));

            string encrypted = protector.Protect(input);
            Console.WriteLine($"Encrypted: {encrypted}");

            string decrypted = protector.Unprotect(encrypted);
            Console.WriteLine($"Decrypted: {decrypted}");
        }
    }

    static void Call()
    {
        ServiceCollection collection = new();
        collection.AddDataProtection();
        ServiceProvider services = collection.BuildServiceProvider();

        DataVault vault = ActivatorUtilities.CreateInstance<DataVault>(services);
        vault.Execute();
    }
}