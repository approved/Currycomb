using System.Net;
using System.Threading.Tasks;
using Currycomb.Common.Network;
using System.Threading.Channels;
using System.Threading;
using Serilog;
using Currycomb.Common.Network.Game;
using Currycomb.Gateway.Clients;
using Currycomb.Gateway.Routers;

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
            IPEndPoint wpktEndpoint = new(IPAddress.Any, 10000);
            IPEndPoint gameEndpoint = new(IPAddress.Any, 25565);

            log.Information("Starting GatewayServer.");
            log.Information("Ports: ");
            log.Information("  Wpkt: {wpktPort}", wpktEndpoint.Port);
            log.Information("  Game: {gamePort}", gameEndpoint.Port);

            ServiceCollection services = new();
            ClientCollection clients = new();

            ServiceListener serviceListener = new(wpktEndpoint, services);

            ClientListener clientListener = new(
                clients,
                gameEndpoint);

            PacketRouter router = new(
                clients,
                new(services),
                new());

            Channel<WrappedPacketContainer> servicePackets = Channel.CreateUnbounded<WrappedPacketContainer>();
            Channel<(State, WrappedPacket)> clientPackets = Channel.CreateUnbounded<(State, WrappedPacket)>();

            Task routeServicePackets = Task.Run(async () =>
            {
                while (!ct.IsCancellationRequested)
                    await router.RoutePacketFromService(await servicePackets.Reader.ReadAsync(ct), ct);
            });

            Task routeClientPackets = Task.Run(async () =>
            {
                while (!ct.IsCancellationRequested)
                {
                    var (state, packet) = await clientPackets.Reader.ReadAsync(ct);
                    await router.RoutePacketFromClient(state, packet, ct);
                }
            });

            log.Information("Finished starting GatewayServer");

            await await Task.WhenAny(
                routeServicePackets,
                routeClientPackets,

                services.ReadPacketsToChannel(servicePackets.Writer, ct),
                clients.ReadPacketsToChannel(clientPackets.Writer, ct),

                serviceListener.AcceptConnections(ct),
                clientListener.AcceptConnections(ct));

            log.Information("GatewayServer is shutting down");
        }
    }
}
