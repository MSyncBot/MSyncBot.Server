using System.Net;
using System.Net.Sockets;
using MLoggerService;
using NetCoreServer;

namespace MSyncBot.Server;

class Server(IPAddress address, int port, MLogger logger) : TcpServer(address, port)
{
    private MLogger Logger { get; } = logger;

    protected override TcpSession CreateSession() { return new Session(this, Logger); }

    protected override void OnError(SocketError error)
    {
        Logger.LogError($"Chat TCP server caught an error with code {error}");
    }
}