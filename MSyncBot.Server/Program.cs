using System.Net;
using MLoggerService;

namespace MSyncBot.Server;

class Program
{
    static void Main()
    {
        var logger = new MLogger();
        var server = new Server(IPAddress.Parse("127.0.0.1"), 8888, logger);
        try
        {
            server.Start();
            logger.LogInformation("Press any key for close program...");
        }
        finally
        {
            server.Stop();
        }
        
    }
}