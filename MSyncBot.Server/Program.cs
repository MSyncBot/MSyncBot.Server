using System.Net;
using System.Net.Security;
using System.Text.Json;
using MConfiguration;
using MLoggerService;
using MSyncBot.Types;
using MSyncBot.Types.Enums;

namespace MSyncBot.Server
{
    internal abstract class Program
    {
        private static void Main()
        {
            var logger = new MLogger();
            logger.LogProcess("Initializing logger...");
            logger.LogSuccess("Logger successfully initialized.");
            
            logger.LogProcess("Initializing program configuration...");
            var configManager = new ConfigManager();
            var programConfig = new ProgramConfiguration();
            foreach (var property in typeof(ProgramConfiguration).GetProperties())
            {
                var propertyName = property.Name;
                var data = configManager.Get(propertyName);

                if (string.IsNullOrEmpty(data))
                {
                    logger.LogInformation($"Enter value for {propertyName}:");
                    data = Console.ReadLine();
                }
            
                property.SetValue(programConfig, Convert.ChangeType(data, property.PropertyType));
            }
        
            configManager.Set(programConfig);
            var server = new Server(programConfig.IpAddress, programConfig.Port, logger);
            logger.LogSuccess("Program configuration has been initialized.");
            
            logger.LogProcess("Starting the server...");
            server.Start();
            logger.LogSuccess("The server successfully started.");
            logger.LogInformation($"Server address: {programConfig.IpAddress}");
            logger.LogInformation($"Server port: {programConfig.Port}");

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
                    new User("Administrator", 0),
                    new Chat("Server", 0))
                {
                    Text = message
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