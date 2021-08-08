using System;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Serilog;
using Currycomb.Gateway.Network;
using Currycomb.Gateway.Network.Services;
using Currycomb.Common.Network;

namespace Currycomb.Gateway
{
    public class GatewayServer
    {
        private static readonly string LogFileName = $"logs/gateway/gateway_{DateTime.Now:yyyy-MM-dd-HH-mm-ss}_{Environment.ProcessId}.txt";

        private Thread InputThread;
        public bool ShuttingDown;

        public static async Task Main()
        {
            Log.Logger = new LoggerConfiguration()
                   .MinimumLevel.Verbose()
                   .WriteTo.Async(x => x.Console())
                   .WriteTo.Async(x => x.File(LogFileName))
                   .CreateLogger();

            await new GatewayServer().Run();
        }

        public GatewayServer()
        {
            InputThread = new Thread(ProcessInput)
            {
                Name = "CommandInputThread",
                IsBackground = true,
            };
            InputThread.Start();
        }

        private async Task Run()
        {
            // TODO: Move to configuration file
            // Open a listener on port 25565 (default MC port)
            TcpListener listener = new(IPAddress.Any, 25565);

            using TcpClient authClient = new TcpClient();
            await authClient.ConnectAsync("localhost", 10001);

            using WrappedPacketStream authStream = new(authClient.GetStream());
            // using WrappedPacketStream playStream = new(playClient.GetStream());

            AuthService authService = new(authStream);
            PlayService playService = new();

            IncomingPacketDispatcher incomingPacketDispatcher = new(authService, playService);

            await Task.WhenAll(
                ServerListener.StartListener(listener, incomingPacketDispatcher, authStream),
                authStream.RunAsync()
            );
        }

        private void ProcessInput()
        {   
            string input;
            do
            {
                input = Console.ReadLine() ?? throw new NullReferenceException("Stdin returned null string");
            }
            while (!ShuttingDown || input.ToLower() == "quit");
        }
    }
}
