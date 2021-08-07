using Currycomb.Gateway.ClientData;
using Currycomb.Gateway.Network.Services;
using Serilog;
using System;

namespace Currycomb.Gateway.Network
{
    public class IncomingPacketDispatcher
    {
        private AuthService _authService;
        private PlayService _playService;

        public IncomingPacketDispatcher(AuthService authService, PlayService playService)
        {
            this._authService = authService;
            this._playService = playService;
        }

        public void Dispatch(Guid id, bool isInPlayState, PacketReader reader)
        {
            int packetId = reader.Read7BitEncodedInt();
            Log.Debug($"Received packet: ClientId {id}, {nameof(isInPlayState)} {isInPlayState}, PacketId {packetId}");

            if (isInPlayState)
            {
                _playService.Handle(id, reader);
            }
            else
            {
                _authService.Handle(id, reader);
            }
        }
    }
}
