using System.Net;
using System.Text;

namespace MSyncBot.Server;

class Server
{
    private static readonly List<Client> Clients = new();

    private static void HandleClient(object obj)
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
                Console.WriteLine($"Получено от {client.Name} {client.Id}: {message}");

                BroadcastMessage(message, client);
                builder.Clear();
            }
        }
        catch (Exception)
        {
            Clients.Remove(client);
            client.TcpClient.Close();
            Console.WriteLine($"{client.Name} {client.Id} отключился.");
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