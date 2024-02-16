using System.Net;
using System.Net.Sockets;
using MLoggerService;
using NetCoreServer;

namespace MSyncBot.Server;

internal class Server(string address, int port, MLogger logger) : WsServer(address, port)
{
    private MLogger Logger { get; } = logger;
    
    protected override TcpSession CreateSession() { return new Session(this, Logger); }

    protected override void OnError(SocketError error)
    {
        Logger.LogError($"Chat WebSocket server caught an error with code {error}");
    }
}