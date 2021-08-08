using System;

namespace Currycomb.Common.Network.Broadcast
{
    public enum EventType
    {
        ChangedState,
        ChangedId,
        Disconnect
    }

    public static class EventTypeExtensions
    {
        public static string ToSubject(this EventType @event) => @event switch
        {
            EventType.ChangedState => "client::changed_state",
            EventType.ChangedId => "client::changed_id",
            EventType.Disconnect => "client::disconnect",
            _ => throw new InvalidOperationException($"Unknown event type {@event}."),
        };
    }
}
