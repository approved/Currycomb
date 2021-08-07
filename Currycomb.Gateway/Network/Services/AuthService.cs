using Serilog;
using System;

namespace Currycomb.Gateway.Network.Services
{
    public class AuthService
    {
        public void Handle(Guid id, PacketReader reader)
        {
            Log.Warning($"{id} attempting to complete handshake");

            // TODO: Broadcast Event Server
        }
    }
}
