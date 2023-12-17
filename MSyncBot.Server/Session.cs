using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using MLoggerService;
using MSyncBot.Server.Types;
using NetCoreServer;

namespace MSyncBot.Server;

class Session(WsServer server, MLogger logger) : WsSession(server)
{
    private MLogger Logger { get; set; } = logger;
    
    public override void OnWsConnected(HttpRequest request)
    {
        Logger.LogSuccess($"Chat WebSocket session with Id {Id} connected!");
        
        //string message = "Hello from WebSocket chat! Please send a message or '!' to disconnect the client!";
        //SendTextAsync(message);
    }

    public override void OnWsDisconnected()
    {
        Logger.LogError($"Chat WebSocket session with Id {Id} disconnected!");
    }

    public override void OnWsReceived(byte[] buffer, long offset, long size)
    {
        string jsonMessage = Encoding.UTF8.GetString(buffer, (int)offset, (int)size);
        Logger.LogInformation("Incoming: " + jsonMessage);

        var message = JsonSerializer.Deserialize<Message>(jsonMessage);
        
        // Multicast message to all connected sessions
        ((WsServer)Server).MulticastText(jsonMessage);

        // If the buffer starts with '!' the disconnect the current session
        if (message.Content == "!")
            Close(1000);
    }

    protected override void OnError(SocketError error)
    {
        Logger.LogError($"Chat WebSocket session caught an error with code {error}");
    }
}