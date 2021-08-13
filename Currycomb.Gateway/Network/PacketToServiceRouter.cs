using System.Threading.Tasks;
using Currycomb.Common.Network;

namespace Currycomb.Gateway.Network
{
    public class PacketToServiceRouter
    {
        private readonly AuthServiceManager _auth;
        private readonly PlayServiceManager _play;

        public readonly IService[] Services;

        public PacketToServiceRouter(AuthServiceManager auth, PlayServiceManager play)
        {
            _auth = auth;
            _play = play;

            Services = new IService[] { auth, play };
        }

        // TODO: In the future at some point we'll probably want services to themselves subscribe to game and meta packets.
        // That'll also allow us to not treat different services differently, but rather just have multiple `Service` instances with self-reported capabilities.

        // Meta packets go to all services that support them
        public Task HandleMetaAsync(WrappedPacket packet) => Task.WhenAll(
            _play.HandleAsync(requireDelivery: false, isMeta: true, packet).AsTask(),
            _auth.HandleAsync(requireDelivery: false, isMeta: true, packet).AsTask()
        );

        public ValueTask HandleGameAsync(bool authenticated, WrappedPacket packet) => authenticated switch
        {
            true => _play.HandleAsync(requireDelivery: true, isMeta: false, packet),
            false => _auth.HandleAsync(requireDelivery: true, isMeta: false, packet),
        };
    }
}
