using System.Net;
using System.Net.Sockets;
using NetCoreServer;

namespace MSyncBot.Server;

class HttpCacheServer : NetCoreServer.HttpServer
{
    public HttpCacheServer(IPAddress address, int port) : base(address, port) {}

    protected override TcpSession CreateSession() { return new HttpCacheSession(this); }

    protected override void OnError(SocketError error)
    {
        Console.WriteLine($"HTTP session caught an error: {error}");
    }
}