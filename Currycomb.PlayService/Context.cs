﻿using Currycomb.Common.Network;
using Currycomb.Common.Network.Broadcast;
using Currycomb.Common.Network.Minecraft;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;

namespace Currycomb.PlayService
{
    public class Context : IPacketRouterContext
    {
        public readonly Guid ClientId;

        private WrappedPacketStream _wps;
        private ClientWebSocket _events;

        public State State => State.Play;

        public Context(Guid clientId, WrappedPacketStream wps, ClientWebSocket events)
            => (ClientId, _wps, _events) = (clientId, wps, events);

        public Task SetState(State state)
            => Broadcast(EventType.ChangedState, new PayloadStateChange(ClientId, state));

        public async Task SendPacket<T>(T packet) where T : IPacket
            => await _wps.SendAsync(new WrappedPacket(ClientId, packet.ToBytes()));

        public async Task Broadcast(ComEvent ev)
            => await _events.SendAsync(Encoding.UTF8.GetBytes(ev.Serialize()), WebSocketMessageType.Text, true, default);

        public Task Broadcast<T>(EventType evt, T data)
            => Broadcast(ComEvent.Create(evt, data));
    }
}