using System.Net.Sockets;
using System.Text;
using MLoggerService;
using NetCoreServer;

namespace MSyncBot.Server;

class Session(TcpServer server, MLogger logger) : TcpSession(server)
{
    private MLogger Logger { get; } = logger;

    protected override void OnConnected()
    {
        Logger.LogInformation($"TCP client session with Id {Id} connected!");
        
        const string welcomeMessage = "You successfully connected to the server.";
        SendAsync(welcomeMessage);
    }

    protected override void OnDisconnected()
    {
        Logger.LogInformation($"TCP client session with Id {Id} disconnected!");
    }

    protected override void OnReceived(byte[] buffer, long offset, long size)
    {
        string message = Encoding.UTF8.GetString(buffer, (int)offset, (int)size);
        Console.WriteLine("Incoming: " + message);

        Logger.LogInformation($"Received message from {receivedMessage.SenderName}: {receivedMessage.Content}");
        
        if (message == "!")
            Disconnect();
    }

    protected override void OnError(SocketError error)
    {
        Logger.LogError($"Chat TCP session caught an error with code {error}");
    }
}