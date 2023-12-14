using System.ComponentModel.Design.Serialization;
using System.Net;
using System.Text.Json;
using MLoggerService;
using MSyncBot.Server.Types;
using MSyncBot.Server.Types.Enums;

namespace MSyncBot.Server;

class Program
{
    static void Main(string[] args)
    {
        var logger = new MLogger();
        var server = new Server(IPAddress.Parse("127.0.0.1"), 1689, logger);
        logger.LogInformation($"TCP server address: {server.Address}");
        logger.LogInformation($"TCP server port: {server.Port}");
        
        logger.LogProcess("Server starting...");
        server.Start();
        logger.LogSuccess("Done!");

        logger.LogInformation("Press Enter to stop the server or '!' to restart the server...");
        
        for (;;)
        {
            var message = Console.ReadLine();
            if (string.IsNullOrEmpty(message))
                break;
            
            if (message == "!")
            {
                logger.LogProcess("Server restarting...");
                server.Restart();
                logger.LogSuccess("Done!");
                continue;
            }
            
            var serverMessage = new Message("MSyncBot.Server", 
                0, 
                SenderType.Server,
                message,
                new User("Server"));
            var jsonServerMessage = JsonSerializer.Serialize(serverMessage);
            
            server.Multicast(jsonServerMessage);
        }
        
        logger.LogProcess("Server stopping...");
        server.Stop();
        logger.LogSuccess("Done!");
    }
}