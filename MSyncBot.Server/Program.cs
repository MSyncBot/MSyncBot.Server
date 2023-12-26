using System.Net;
using System.Net.Security;
using System.Text.Json;
using MLoggerService;
using MSyncBot.Types;
using MSyncBot.Types.Enums;

namespace MSyncBot.Server
{
    class Program
    {
        private static void Main()
        {
            var logger = new MLogger();
            logger.LogProcess("Starting the server...");
            
            const int port = 1689;
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

                var adminMessage = new Message(
                    new Messenger("Server", MessengerType.Server),
                    MessageType.Text,
                    new User("Administrator"),
                    new Chat("Server", 0))
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