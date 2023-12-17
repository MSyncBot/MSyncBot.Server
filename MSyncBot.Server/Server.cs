using System.Net;
using System.Net.Sockets;
using MLoggerService;
using NetCoreServer;

namespace MSyncBot.Server;

class Server(IPAddress address, int port, MLogger logger) : WsServer(address, port)
{
    private MLogger Logger { get; set; } = logger;
    
    protected override TcpSession CreateSession() { return new Session(this, logger); }

    protected override void OnError(SocketError error)
    {
        Logger.LogError($"Chat TCP server caught an error with code {error}");
    }
}