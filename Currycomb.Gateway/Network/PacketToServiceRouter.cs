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

        public async Task DispatchAsync(bool authenticated, WrappedPacket packet)
        {
            await (authenticated switch
            {
                true => _play.HandleAsync(packet),
                false => _auth.HandleAsync(packet),
            });
        }
    }
}
