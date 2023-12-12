using System.ComponentModel.Design.Serialization;
using System.Net;
using MLoggerService;

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
            var line = Console.ReadLine();
            if (string.IsNullOrEmpty(line))
                break;
            
            if (line == "!")
            {
                logger.LogProcess("Server restarting...");
                server.Restart();
                logger.LogSuccess("Done!");
                continue;
            }
            
            line = "(admin) " + line;
            server.Multicast(line);
        }
        
        logger.LogProcess("Server stopping...");
        server.Stop();
        logger.LogSuccess("Done!");
    }
}