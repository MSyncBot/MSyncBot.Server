using System.Net;
using System.Net.Sockets;
using System.Text;
using MLoggerService;
using Newtonsoft.Json;

namespace MSyncBot.Server
{
    internal class Server
    {
        private IPAddress IpAddress { get; }
        private const int Port = 1689;
        private TcpListener TcpServer { get; }
        private bool IsRunning { get; set; }
        private MLogger Logger { get; set; }

        public Server(IPAddress ipAddress, MLogger logger)
        {
            IpAddress = ipAddress;
            Logger = logger;
            TcpServer = new TcpListener(IpAddress, Port);
            IsRunning = false;
        }

        private readonly Dictionary<Guid, Client> Clients = new();

        public async Task StartAsync()
        {
            try
            {
                if (IsRunning)
                {
                    Logger.LogError("The server already running!");
                    return;
                }

                Logger.LogProcess($"Starting server on {IpAddress}:{Port}...");
                TcpServer.Start();
                IsRunning = true;
                Logger.LogSuccess($"Server successfully started on {IpAddress}:{Port}");

                while (IsRunning)
                {
                    var tcpClient = await TcpServer.AcceptTcpClientAsync();
                    _ = HandleClientAsync(tcpClient);
                }
            }
            catch (Exception e)
            {
                Logger.LogError(e.Message);
            }
        }

        private async Task HandleClientAsync(TcpClient tcpClient)
        {
            var clientId = Guid.NewGuid();
            try
            {
                RegisterClient(clientId, tcpClient);
                var isFirstTime = true;

                var stream = tcpClient.GetStream();
                while (tcpClient.Connected)
                {
                    var client = await ReceiveMessageAsync(stream);
                    if (isFirstTime)
                    {
                        client.TcpClient = tcpClient;
                        UpdateClientData(clientId, new Client(client));
                        isFirstTime = false;
                        Logger.LogSuccess($"{client.Name} connected.");
                        continue;
                    }

                    Logger.LogInformation(client.Name + ": " + client.Message);
                    await SendMessageAsync(client);
                }
            }
            catch (Exception)
            {
                tcpClient.Close();
                if (Clients.TryGetValue(clientId, out var client))
                {
                    Clients.Remove(clientId);
                    Logger.LogError($"{client.Name} disconnected.");
                }
            }
        }

        private async Task<Client?> ReceiveMessageAsync(Stream stream)
        {
            try
            {
                var completeMessage = new byte[1024];
                var bytesRead = await stream.ReadAsync(completeMessage);

                if (bytesRead > 0)
                {
                    var client = Encoding.UTF8.GetString(completeMessage, 0, bytesRead);
                    return JsonConvert.DeserializeObject<Client>(client);
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.Message);
            }

            return null;
        }

        private async Task SendMessageAsync(Client sender)
        {
            try
            {
                var senderId = Clients.FirstOrDefault(x => x.Value.Name == sender.Name).Key;
                switch (sender.ClientType)
                {
                    case ClientType.Telegram:
                        foreach (var (id, cl) in Clients)
                        {
                            if (id == senderId ||
                                (cl.ClientType != ClientType.Discord && cl.ClientType != ClientType.VK)) continue;
                            var recipientData = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(sender));
                            await cl.TcpClient.GetStream().WriteAsync(recipientData);
                        }

                        return;

                    case ClientType.Discord:
                        foreach (var (id, cl) in Clients)
                        {
                            if (id == senderId ||
                                (cl.ClientType != ClientType.Telegram && cl.ClientType != ClientType.VK)) continue;
                            var recipientData = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(sender));
                            await cl.TcpClient.GetStream().WriteAsync(recipientData);
                        }

                        return;

                    case ClientType.VK:
                        foreach (var (id, cl) in Clients)
                        {
                            if (id == senderId || (cl.ClientType != ClientType.Telegram &&
                                    cl.ClientType != ClientType.Discord)) continue;
                            var recipientData = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(sender));
                            await cl.TcpClient.GetStream().WriteAsync(recipientData);
                        }

                        return;

                    case ClientType.None:
                    default:
                        Logger.LogError("Unknown client type.");
                        return;
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.Message);
            }
        }
        
        private void RegisterClient(Guid clientId, TcpClient tcpClient)
        {
            try
            {
                Clients.Add(clientId, new Client("Unknown", tcpClient, ClientType.None));
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.Message);
            }
        }

        private void UpdateClientData(Guid clientId, Client updatedClient)
        {
            try
            {
                if (!Clients.ContainsKey(clientId)) return;
                Clients[clientId] = updatedClient;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.Message);
            }
        }

        public void Stop()
        {
            Logger.LogProcess("Stopping server...");
            TcpServer.Stop();
            IsRunning = false;
            Logger.LogSuccess("Server successfully stopped.");
        }
    }
}