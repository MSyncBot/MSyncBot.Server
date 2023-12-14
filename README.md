# ️🔄️ MSyncBot.Server 🔄️

**MSyncBot.Server** - is a library for the MSyncBot project,
with which you can create a server for messaging 
in different social networks using MSyncBot.Telegram, MSyncBot.Discord.

## How to use ❓
```csharp
// Initialize MLogger
var logger = new MLogger();

// Initialize server, use your ip and port
var server = new Server(IPAddress.Parse("127.0.0.1"), 1689, logger);
logger.LogInformation($"TCP server address: {server.Address}");
logger.LogInformation($"TCP server port: {server.Port}");

logger.LogProcess("Server starting...");
server.Start(); // Starting the server
logger.LogSuccess("Done!");

logger.LogInformation("Press Enter to stop the server or '!' to restart the server...");

for (;;)
{
    var message = Console.ReadLine(); // You can send message to clients
    if (string.IsNullOrEmpty(message))
        break;
    
    if (message == "!")
    {
        logger.LogProcess("Server restarting...");
        server.Restart(); // Also you can restart the server
        logger.LogSuccess("Done!");
        continue;
    }
    
    // Generate your message
    var serverMessage = new Message("MSyncBot.Server", 0, SenderType.Server, message, new User("Server"));
    // Serialize your message
    var jsonServerMessage = JsonSerializer.Serialize(serverMessage);

    // And send to clients
    server.Multicast(jsonServerMessage);
}

logger.LogProcess("Server stopping...");    
server.Stop(); // Stop the server
logger.LogSuccess("Done!");
```

## Dependencies ⚡
- [NetCoreServer](https://github.com/chronoxor/NetCoreServer)
- [MLogger](https://github.com/mambastick/MLogger)