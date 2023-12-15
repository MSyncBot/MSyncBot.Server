using System.Net;

namespace MSyncBot.Server;

class Program
{
    static void Main(string[] args)
    {
        // HTTP server port
        int port = 8080;
        if (args.Length > 0)
            port = int.Parse(args[0]);
        // HTTP server content path
        string www = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "www", "api");
        Directory.CreateDirectory(www);
        if (args.Length > 1)
            www = args[1];

        Console.WriteLine($"HTTP server port: {port}");
        Console.WriteLine($"HTTP server static content path: {www}");
        Console.WriteLine($"HTTP server website: http://localhost:{port}/api/index.html");

        Console.WriteLine();

        // Create a new HTTP server
        var server = new HttpCacheServer(IPAddress.Any, port);
        server.AddStaticContent(www, "/api");

        // Start the server
        Console.Write("Server starting...");
        server.Start();
        Console.WriteLine("Done!");

        Console.WriteLine("Press Enter to stop the server or '!' to restart the server...");

        // Perform text input
        for (;;)
        {
            string line = Console.ReadLine();
            if (string.IsNullOrEmpty(line))
                break;

            // Restart the server
            if (line == "!")
            {
                Console.Write("Server restarting...");
                server.Restart();
                Console.WriteLine("Done!");
            }
        }

        // Stop the server
        Console.Write("Server stopping...");
        server.Stop();
        Console.WriteLine("Done!");
    }
}