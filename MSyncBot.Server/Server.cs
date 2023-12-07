using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using MLoggerService;

namespace MSyncBot.Server
{
    internal class Server
    {
        private IPAddress IpAddress { get; }
        private const int Port = 1689;
        private TcpListener TcpServer { get; }
        private bool IsRunning { get; set; }
        private MLogger Logger { get; set; }

        public Server(IPAddress ipAddress, MLogger logger)
        {
            IpAddress = ipAddress;
            Logger = logger;
            TcpServer = new TcpListener(IpAddress, Port);
            IsRunning = false;
        }

        private readonly List<TcpClient> Clients = new();

        public async Task StartAsync()
        {
            try
            {
                if (IsRunning)
                {
                    Logger.LogError("The server already running!");
                    return;
                }

                Logger.LogProcess($"Starting server on {IpAddress}:{Port}...");
                TcpServer.Start();
                IsRunning = true;
                Logger.LogSuccess($"Server successfully started on {IpAddress}:{Port}");

                while (IsRunning)
                {
                    var tcpClient = await TcpServer.AcceptTcpClientAsync();
                    _ = HandleClientAsync(tcpClient);
                    Logger.LogSuccess($"client connected.");
                }
            }
            catch (Exception e)
            {
                Logger.LogError(e.Message);
            }
        }

        private async Task HandleClientAsync(TcpClient tcpClient)
        {
            try
            {
                var stream = tcpClient.GetStream();
                while (tcpClient.Connected)
                {
                    var message = await ReceiveMessageAsync(stream);
                    Logger.LogInformation(message);
                    await SendMessageAsync(stream, message);
                }
            }
            catch (Exception)
            {
                Clients.Remove(tcpClient);
                tcpClient.Close();
                Logger.LogError($"client disconnected.");
            }
        }

        private async Task<string> ReceiveMessageAsync(NetworkStream stream)
        {
            try
            {
                var completeMessage = new StringBuilder();
                var bufferSize = new byte[1024];
                var bytesRead = await stream.ReadAsync(bufferSize);
                completeMessage.Append(Encoding.UTF8.GetString(bufferSize, 0, bytesRead));
                return completeMessage.ToString();
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.Message);
                return string.Empty;
            }
        }

        private async Task SendMessageAsync(NetworkStream stream, string message)
        {
            try
            {
                var data = Encoding.UTF8.GetBytes(message);
                await stream.WriteAsync(data);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.Message);
            }
        }

        public void Stop()
        {
            Logger.LogProcess("Stopping server...");
            TcpServer.Stop();
            IsRunning = false;
            Logger.LogSuccess("Server successfully stopped.");
        }
    }
}