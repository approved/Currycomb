using System.Net;
using System.Threading.Tasks;
using Currycomb.Gateway.Network.Services;
using Currycomb.Common.Network;
using System.Threading.Channels;
using System.Threading;
using Serilog;
using Serilog.Core;

namespace Currycomb.Gateway
{
    public class GatewayServer
    {
        private static readonly ILogger log = Log.ForContext<GatewayServer>();

        public async Task Run()
        {
            CancellationTokenSource cts = new();
            CancellationToken ct = cts.Token;

            // TODO: Move to configuration file
            IPEndPoint authEndpoint = new(IPAddress.Any, 10001);
            IPEndPoint playEndpoint = new(IPAddress.Any, 10003);
            IPEndPoint gameEndpoint = new(IPAddress.Any, 25565);

            log.Information("Starting GatewayServer.");
            log.Information("Ports: ");
            log.Information("  Auth: {authPort}", authEndpoint.Port);
            log.Information("  Play: {playPort}", playEndpoint.Port);
            log.Information("  Game: {gamePort}", gameEndpoint.Port);

            AuthServiceManager authService = new();
            PlayServiceManager playService = new();

            ServiceListener<AuthServiceManager, AuthService> authListener = new(
                authService,
                authEndpoint,
                x => new(new(x.GetStream())));

            ServiceListener<PlayServiceManager, PlayService> playListener = new(
                playService,
                playEndpoint,
                x => new(new(x.GetStream())));

            ServiceCollection services = new(authService, playService);
            ClientCollection clients = new();

            ClientListener clientListener = new(
                clients,
                gameEndpoint);

            PacketRouter router = new(
                clients,
                new(authService, playService),
                new(),
                new());

            Channel<WrappedPacketContainer> servicePackets = Channel.CreateUnbounded<WrappedPacketContainer>();
            Channel<(bool Authed, WrappedPacket)> clientPackets = Channel.CreateUnbounded<(bool, WrappedPacket)>();

            Task routeServicePackets = Task.Run(async () =>
            {
                await foreach (var packet in servicePackets.Reader.ReadAllAsync(ct))
                    await router.RoutePacketFromService(packet, ct);
            });

            Task routeClientPackets = Task.Run(async () =>
            {
                await foreach (var (authenticated, packet) in clientPackets.Reader.ReadAllAsync(ct))
                    await router.RoutePacketFromClient(authenticated, packet, ct);
            });

            log.Information("Finished starting GatewayServer");

            await await Task.WhenAny(
                routeServicePackets,
                routeClientPackets,

                clients.ReadPacketsToChannel(clientPackets, ct),
                services.ReadPacketsToChannel(servicePackets, ct),

                authListener.AcceptConnections(ct),
                playListener.AcceptConnections(ct),

                clientListener.AcceptConnections(ct)
            );

            log.Information("GatewayServer is shutting down");
        }
    }
}
