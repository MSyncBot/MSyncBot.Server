using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using MLoggerService;
using MSyncBot.Types;
using MSyncBot.Types.Enums;
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
        var jsonMessage = Encoding.UTF8.GetString(buffer, (int)offset, (int)size);
        ((WsServer)Server).MulticastText(jsonMessage);
        
        var message = JsonSerializer.Deserialize<Message>(jsonMessage);

        var messageInfo = message.MessageType switch
        {
            MessageType.Text => $"text: {message.Content}",
            MessageType.Photo => $"photo: {message.MediaFiles[0].Name}{message.MediaFiles[0].Extension}",
            MessageType.Video => $"video: {message.MediaFiles[0].Name}{message.MediaFiles[0].Extension}",
            MessageType.Voice => $"voice: {message.MediaFiles[0].Name}{message.MediaFiles[0].Extension}",
            MessageType.Album => $"album, number of media: {message.MediaFiles.Count}",
            MessageType.Audio => $"audio: {message.MediaFiles[0].Name}{message.MediaFiles[0].Extension}",
            MessageType.Animation => $"animation: {message.MediaFiles[0].Name}{message.MediaFiles[0].Extension}",
            MessageType.Document => $"document or file: {message.MediaFiles[0].Name}{message.MediaFiles[0].Extension}",
            MessageType.VideoNote => $"video note: {message.MediaFiles[0].Name}{message.MediaFiles[0].Extension}",
            MessageType.Unknown => $"unknown info",
            _ => throw new ArgumentOutOfRangeException()
        };
        
        logger.LogInformation($"Received from {message.SenderName} with {messageInfo}");
        
        if (message.Content == "!")
            Close(1000);
    }

    protected override void OnError(SocketError error)
    {
        Logger.LogError($"Chat WebSocket session caught an error with code {error}");
    }
}