using System.Net.Sockets;

namespace MSyncBot.Server;

internal abstract class Client
{
    public string Name { get; }
    public int Id { set; get; }
    public TcpClient TcpClient { get; }

    protected Client(string name, TcpClient client)
    {
        Name = name;
        TcpClient = client;
    }
}