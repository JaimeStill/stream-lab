using System.Text;
using StreamLab.Common.Cli;

namespace StreamLab.Cli.Commands;
public class EncodingCommand : CliCommand
{
    public EncodingCommand() : base(
        "encoding",
        "Discover API details of System.Text.Encoding",
        new Action(Call)
    ) { }

    static void Call()
    {
        EncodingInfo[] encodingInfo = Encoding.GetEncodings();

        foreach (EncodingInfo info in encodingInfo)
        {
            Console.WriteLine("EncodingInfo Properties:");
            Console.WriteLine($"CodePage: {info.CodePage}");
            Console.WriteLine($"DisplayName: {info.DisplayName}");
            Console.WriteLine($"Name: {info.Name}");
            Console.WriteLine();

            Encoding encoding = Encoding.GetEncoding(info.CodePage);
            Console.WriteLine("Encoding Properties:");
            Console.WriteLine($"CodePage: {encoding.CodePage}");
            Console.WriteLine($"EncodingName: {encoding.EncodingName}");
            Console.WriteLine($"BodyName: {encoding.BodyName}");
            Console.WriteLine($"HeaderName: {encoding.HeaderName}");
            Console.WriteLine($"WebName: {encoding.WebName}");
            Console.WriteLine();
        }

        Encoding defaultEncoding = Encoding.GetEncoding(Encoding.Default.CodePage);
        Console.WriteLine($"Default Encoding: {defaultEncoding.EncodingName}");
    }
}