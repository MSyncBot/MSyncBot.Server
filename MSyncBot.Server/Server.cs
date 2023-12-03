using System.Net;
using System.Net.Sockets;
using System.Text;
using MLoggerService;

namespace MSyncBot.Server;

internal class Server
{
    private IPAddress IpAddress { get; set; }
    private int Port { get; set; }
    private TcpListener TcpServer { get; set; }
    private MLogger Logger { get; set; }
    
    public Server(IPAddress ipAddress, int port, MLogger logger)
    {
        IpAddress = ipAddress;
        Port = port;
        Logger = logger;
    }

    private static readonly List<Client> Clients = new();

    public void Start()
    {
        try
        {
            Logger.LogProcess($"Starting server on {IpAddress}:{Port}...");
            
            TcpServer = new TcpListener(IpAddress, Port);
            TcpServer.Start();

            Logger.LogSuccess($"Server successfully started on {IpAddress}:{Port}");
            
            while (true)
            {
                var tcpClient = TcpServer.AcceptTcpClient();
                var client = new Client("name", tcpClient);
                Clients.Add(client);

                var clientThread = new Thread(HandleClient);
                clientThread.Start(client);

                Logger.LogInformation($"{client.Name} {client.Id} connected.");
            }
        }
        catch (Exception e)
        {
            Logger.LogError(e.Message);
        }
    }

    public void Stop()
    {
        Logger.LogProcess("Stopping server...");
        TcpServer.Stop();
        Logger.LogSuccess("Server successfully stopped.");
    }

    private void HandleClient(object obj)
    {
        var client = (Client)obj;
        var stream = client.TcpClient.GetStream();

        var data = new byte[256];
        var builder = new StringBuilder();

        try
        {
            while (true)
            {
                var bytes = stream.Read(data, 0, data.Length);
                builder.Append(Encoding.UTF8.GetString(data, 0, bytes));
                var message = builder.ToString();
                Logger.LogInformation($"Received from {client.Name} {client.Id}: {message}");

                BroadcastMessage(message, client);
                builder.Clear();
            }
        }
        catch (Exception)
        {
            Clients.Remove(client);
            client.TcpClient.Close();
            Logger.LogInformation($"{client.Name} {client.Id} disconnected.");
        }
    }

    private static void BroadcastMessage(string message, Client senderClient)
    {
        foreach (var clientInfo in Clients)
        {
            if (clientInfo == senderClient) continue;
            var stream = senderClient.TcpClient.GetStream();
            var responseData = Encoding.UTF8.GetBytes($"{senderClient.Name} {senderClient.Id}: {message}");
            stream.Write(responseData, 0, responseData.Length);
        }
    }
}