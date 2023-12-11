using System.Net;
using System.Net.Sockets;
using NetCoreServer;

namespace MSyncBot.Server;

class Server : TcpServer
{
    public Server(IPAddress address, int port) : base(address, port) {}

    protected override TcpSession CreateSession() { return new Session(this); }

    protected override void OnError(SocketError error)
    {
        Console.WriteLine($"Chat TCP server caught an error with code {error}");
    }
}