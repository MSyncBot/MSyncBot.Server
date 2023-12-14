using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using MLoggerService;
using MSyncBot.Server.Types;
using MSyncBot.Server.Types.Enums;
using NetCoreServer;

namespace MSyncBot.Server;

class Session(TcpServer server, MLogger logger) : TcpSession(server)
{
    private MLogger Logger { get; } = logger;
    
    protected override void OnConnected()
    {
        Logger.LogInformation($"TCP client session with Id {Id} connected!");
        var welcomeMessage = new Message("MSyncBot.Server", 
            0, 
            SenderType.Server,
            "You successfully connected to the server.",
            new User("Server"));
        var jsonWelcomeMessage = JsonSerializer.Serialize(welcomeMessage);
        SendAsync(jsonWelcomeMessage);
    }

    protected override void OnDisconnected()
    {
        Logger.LogInformation($"TCP client session with Id {Id} disconnected!");
    }

    protected override void OnReceived(byte[] buffer, long offset, long size)
    {
        var jsonMessage = Encoding.UTF8.GetString(buffer, (int)offset, (int)size);
        var receivedMessage = JsonSerializer.Deserialize<Message>(jsonMessage);

        Logger.LogInformation($"Received message from {receivedMessage.SenderName}: {receivedMessage.Content}");
        server.Multicast(jsonMessage);
    }

    protected override void OnError(SocketError error)
    {
        Logger.LogError($"Chat TCP session caught an error with code {error}");
    }
}