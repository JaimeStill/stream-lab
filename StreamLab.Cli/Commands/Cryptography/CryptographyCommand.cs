using StreamLab.Common.Cli;

namespace StreamLab.Cli.Commands;
public class CryptographyCommand : CliCommand
{
    public CryptographyCommand() : base(
        "cryptography",
        "Lab for experimenting with Cryptography concepts",
        commands: new()
        {
            new AsymmetricCommand(),
            new GenerateKeysCommand(),
            new HybridCommand(),
            new SymmetricCommand()
        }
    ) { }
}