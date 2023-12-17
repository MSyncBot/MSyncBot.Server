using System.Net;
using System.Net.Security;
using System.Text.Json;
using MLoggerService;
using MSyncBot.Server.Types;
using MSyncBot.Server.Types.Enums;

namespace MSyncBot.Server
{
    class Program
    {
        static void Main(string[] args)
        {
            var logger = new MLogger();
            logger.LogProcess("Starting the server...");
            
            const int port = 8080;
            var ipAddress = IPAddress.Any;
            var server = new Server(ipAddress, port, logger);
            
            server.Start();
            logger.LogSuccess("The server successfully started.");
            logger.LogInformation($"Server address: {ipAddress}");
            logger.LogInformation($"Server port: {port}");

            logger.LogInformation("Press Enter to stop the server or '!' to restart the server...");
            
            for (;;)
            {
                var message = Console.ReadLine();
                if (string.IsNullOrEmpty(message))
                    break;

                if (message == "!")
                {
                    logger.LogProcess("Restarting the server...");
                    server.Restart();
                    logger.LogSuccess("The server successfully restarted.");
                }

                var adminMessage = new Message("Server",
                    0,
                    SenderType.Server,
                    MessageType.Text,
                    new User("Administrator"))
                {
                    Content = message
                };

                var adminJsonMessage = JsonSerializer.Serialize(adminMessage);
                server.MulticastText(adminJsonMessage);
            }

            logger.LogProcess("Stopping the server...");
            server.Stop();
            logger.LogSuccess("Done!");
        }
    }
}