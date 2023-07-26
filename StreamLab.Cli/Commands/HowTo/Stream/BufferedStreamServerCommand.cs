using System.CommandLine;
using System.Net;
using System.Net.Sockets;
using StreamLab.Common.Cli;

namespace StreamLab.Cli.Commands;
public class BufferedStreamServerCommand : CliCommand
{
    public BufferedStreamServerCommand() : base(
        "bufferedstreamserver",
        "Demonstrate using BufferedStream",
        new Func<string?, Task>(Call),
        options: new()
        {
            new Option<string?>(
                new string[] { "--remote", "-r" },
                description: "Client machine name",
                getDefaultValue: () => "localhost"
            )
        }
    )
    { }

    static readonly int dataArraySize = 100;
    static readonly int streamBufferSize = 1000;
    static readonly int numberOfLoops = 10000;

    static string SupportsSeeking(bool canSeek) =>
        canSeek
            ? "supports"
            : "does not support";

    static string IsFaster(double a, double b) =>
        a < b
            ? "faster"
            : "slower";

    static async Task Call(string? remote)
    {
        Socket socket = new(
            AddressFamily.InterNetwork,
            SocketType.Stream,
            ProtocolType.Tcp
        );

        string hostName = remote ?? Dns.GetHostName();

        IPAddress address = (await Dns.GetHostEntryAsync(hostName))
            .AddressList
            .First(x => x.AddressFamily == AddressFamily.InterNetwork);

        await socket.ConnectAsync(new IPEndPoint(
            address, 1800
        ));

        using Stream net = new NetworkStream(socket, true);
        using BufferedStream buffer = new(net, streamBufferSize);

        Console.WriteLine($"NetworkStream {SupportsSeeking(buffer.CanSeek)} seeking.");

        if (buffer.CanWrite)
            await SendData(net, buffer);

        if (buffer.CanRead)
            await ReceiveData(net, buffer);

        Console.WriteLine("Shutting down the connection.");
        buffer.Close();
    }

    static async Task SendData(Stream net, Stream buffer)
    {
        DateTime startTime;
        double networkTime, bufferedTime;

        byte[] dataToSend = new byte[dataArraySize];
        new Random().NextBytes(dataToSend);

        Console.WriteLine("Sending data using NetworkStream.");
        startTime = DateTime.Now;

        for (int i = 0; i < numberOfLoops; i++)
            await net.WriteAsync(dataToSend, 0, dataToSend.Length);

        networkTime = (DateTime.Now - startTime).TotalSeconds;
        Console.WriteLine($"{numberOfLoops * dataToSend.Length} bytes sent in {networkTime:F1} seconds.");

        Console.WriteLine("Sending data using BufferedStream.");
        startTime = DateTime.Now;

        for (int i = 0; i < numberOfLoops; i++)
            await buffer.WriteAsync(dataToSend, 0, dataToSend.Length);

        await buffer.FlushAsync();

        bufferedTime = (DateTime.Now - startTime).TotalSeconds;
        Console.WriteLine($"{numberOfLoops * dataToSend.Length} bytes sent in {bufferedTime:F1} seconds.");

        Console.WriteLine($"Sending data using the buffered network stream was {networkTime / bufferedTime:P0} {IsFaster(bufferedTime, networkTime)} than using the network stream alone.");
    }

    static async Task ReceiveData(Stream net, Stream buffer)
    {
        DateTime startTime;
        double networkTime, bufferedTime;
        int bytesReceived = 0;
        byte[] receivedData = new byte[dataArraySize];

        Console.WriteLine("Receiving data using NetworkStream.");
        startTime = DateTime.Now;

        while (bytesReceived < numberOfLoops * receivedData.Length)
            bytesReceived += await net.ReadAsync(
                receivedData, 0, receivedData.Length
            );

        networkTime = (DateTime.Now - startTime).TotalSeconds;
        Console.WriteLine($"{bytesReceived} bytes received in {networkTime:F1} seconds.");

        Console.WriteLine("Receiving data using BufferedStream.");
        bytesReceived = 0;
        startTime = DateTime.Now;
        int numBytesToRead = receivedData.Length;

        while (numBytesToRead > 0)
        {
            int n = await buffer.ReadAsync(receivedData, 0, receivedData.Length);

            if (n == 0)
                break;

            bytesReceived += n;
            numBytesToRead -= n;
        }

        bufferedTime = (DateTime.Now - startTime).TotalSeconds;
        Console.WriteLine($"{bytesReceived} bytes received in {bufferedTime:F1} seconds.");

        Console.WriteLine($"Receiving data using the buffered network stream was {networkTime / bufferedTime:P0} {IsFaster(bufferedTime, networkTime)} than using the network stream alone");
    }
}