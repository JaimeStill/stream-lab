using System.Text;
using StreamLab.Common.Cli;

namespace StreamLab.Cli.Commands;
public class MemoryStreamCommand : CliCommand
{
    public MemoryStreamCommand() : base(
        "memorystream",
        "Demonstrate using MemoryStream",
        new Func<Task>(Call)
    )
    { }

    static async Task Call()
    {
        int count;
        byte[] byteArray;
        char[] charArray;
        UnicodeEncoding encoding = new();

        byte[] firstString = encoding.GetBytes(
            "Invalid file path characters are: "
        );

        byte[] secondString = encoding.GetBytes(
            Path.GetInvalidPathChars()
        );

        using MemoryStream ram = new(100);
        await ram.WriteAsync(firstString, 0, firstString.Length);

        count = 0;

        while (count < secondString.Length)
            ram.WriteByte(secondString[count++]);

        Console.WriteLine(
            $"Capacity = {ram.Capacity}, Length = {ram.Length}, Position = {ram.Position}"
        );

        ram.Seek(0, SeekOrigin.Begin);

        byteArray = new byte[ram.Length];
        count = await ram.ReadAsync(byteArray, 0, 20);

        while (count < ram.Length)
            byteArray[count++] = (byte)ram.ReadByte();

        charArray = new char[encoding.GetCharCount(
            byteArray, 0, count
        )];

        encoding.GetDecoder().GetChars(
            byteArray, 0, count, charArray, 0
        );

        Console.WriteLine(charArray);
    }
}