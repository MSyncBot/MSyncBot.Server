﻿using System.Diagnostics.CodeAnalysis;
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

        var file = string.Empty;
        if (message.Files.Count > 0)
            file = $"{message.Files[0].Name}{message.Files[0].Extension}";
        
        var messageInfo = message.Type switch
        {
            MessageType.Text => $"text: {message.Text}",
            MessageType.Sticker => $"sticker: {file}",
            MessageType.Photo => $"photo: {file}",
            MessageType.Video => $"video: {file}",
            MessageType.Voice => $"voice: {file}",
            MessageType.Album => $"album, number of media: {message.Files.Count}",
            MessageType.Audio => $"audio: {file}",
            MessageType.Animation => $"animation: {file}",
            MessageType.Document => $"document or file: {file}",
            MessageType.VideoNote => $"video note: {file}",
            MessageType.Unknown => "unknown info",
            _ => throw new ArgumentOutOfRangeException()
        };
        
        logger.LogInformation($"Received from {message.Messenger.Name} with {messageInfo}");
        
        if (message.Text == "!")
            Close(1000);
    }

    protected override void OnError(SocketError error)
    {
        Logger.LogError($"Chat WebSocket session caught an error with code {error}");
    }
}