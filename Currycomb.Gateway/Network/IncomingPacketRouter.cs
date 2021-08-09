using System;
using System.Threading.Tasks;
using Currycomb.Gateway.Network.Services;

namespace Currycomb.Gateway.Network
{
    public class IncomingPacketRouter
    {
        private readonly AuthService _authService;
        private readonly PlayService _playService;

        public IncomingPacketRouter(AuthService authService, PlayService playService)
        {
            this._authService = authService;
            this._playService = playService;
        }

        public async Task DispatchAsync(Guid id, bool isInPlayState, Memory<byte> data)
        {
            if (isInPlayState)
            {
                await _playService.HandleAsync(id, data);
            }
            else
            {
                await _authService.HandleAsync(id, data);
            }
        }
    }
}
