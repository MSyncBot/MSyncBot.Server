using System.Net;
using MLoggerService;

namespace MSyncBot.Server;

class Program
{
    static void Main()
    {
        var logger = new MLogger();
        logger.LogInformation("Enter server ip:");
        var ipAddress = Console.ReadLine();
        var server = new Server(IPAddress.Parse(ipAddress), 1689, logger);
        
        try
        {
            server.StartAsync();
            logger.LogInformation("Press any key for close program...");
            Console.ReadKey();
        }
        finally
        {
            server.Stop();
        }
        
    }
}