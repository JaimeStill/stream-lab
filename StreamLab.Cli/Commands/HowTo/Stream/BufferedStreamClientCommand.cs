using System.CommandLine;
using System.Net;
using System.Net.Sockets;
using StreamLab.Common.Cli;

namespace StreamLab.Cli.Commands;
public class BufferedStreamClientCommand : CliCommand
{
    public BufferedStreamClientCommand() : base(
        "bufferedstreamclient",
        "Demonstrate using BufferedStream",
        new Func<Task>(Call)
    )
    { }

    static async Task Call()
    {
        const int WSAETIMEDOUT = 10060;

        Socket socket;
        int bytesReceived, totalReceived = 0;
        byte[] receivedData = new byte[2000000];

        byte[] dataToSend = new byte[2000000];
        new Random().NextBytes(dataToSend);

        IPAddress ipAddress =
            Dns.GetHostEntry(Dns.GetHostName())
                .AddressList
                .First(x => x.AddressFamily == AddressFamily.InterNetwork);

        IPEndPoint ipEndpoint = new(ipAddress, 1800);

        using (Socket listener = new(
            AddressFamily.InterNetwork,
            SocketType.Stream,
            ProtocolType.Tcp
        ))
        {
            listener.Bind(ipEndpoint);
            listener.Listen(1);
            socket = await listener.AcceptAsync();
            Console.WriteLine("Server is connected.");
        }

        try
        {
            Console.WriteLine("Sending data...");

            int bytesSent = socket.Send(
                dataToSend, 0, dataToSend.Length, SocketFlags.None
            );

            Console.WriteLine("{bytesSent} bytes sent.");

            socket.SetSocketOption(
                SocketOptionLevel.Socket,
                SocketOptionName.ReceiveTimeout,
                2000
            );

            Console.Write("Receiving data...");

            try
            {
                do
                {
                    bytesReceived = socket.Receive(
                        receivedData, 0,
                        receivedData.Length,
                        SocketFlags.None
                    );

                    totalReceived += bytesReceived;

                } while (bytesReceived != 0);
            }
            catch(SocketException e)
            {
                if (e.ErrorCode == WSAETIMEDOUT)
                    Console.WriteLine("Data not received within the appropriate time");
                else
                    Console.WriteLine($"{e.GetType().Name}: {e.Message}");
            }
            finally
            {
                Console.WriteLine("{totalReceived} bytes received.");
            }
        }
        finally
        {
            socket.Shutdown(SocketShutdown.Both);
            Console.WriteLine("Connection shut down.");
            socket.Close();
        }
    }
}