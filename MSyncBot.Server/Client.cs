using System.Net.Sockets;
using Newtonsoft.Json;

namespace MSyncBot.Server
{
    public class Client
    {
        public string Name { get; }
        
        public TcpClient TcpClient { get; set; }
        public ClientType ClientType { get; }
        public string? Message { get; }

        [JsonConstructor]
        public Client(string name, TcpClient tcpClient, ClientType clientType, string? message = null)
        {
            Name = name;
            ClientType = clientType;
            Message = message;
            TcpClient = tcpClient;
        }

        public Client(Client client)
        {
            Name = client.Name;
            ClientType = client.ClientType;
            Message = client.Message;
            TcpClient = client.TcpClient;
        }
    }

    public enum ClientType
    {
        Telegram,
        Discord,
        VK,
        None
    }
}