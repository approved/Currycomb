using System;

namespace Currycomb.Common.Network
{
    public class ComEvent
    {
        public long Timestamp { get; set; }
        public string Event { get; set; }
        public object Payload { get; set; }

        public ComEvent(EventType @event, object payload)
        {
            Timestamp = ((DateTimeOffset)DateTime.UtcNow).ToUnixTimeMilliseconds();
            Event = @event.ToString();
            Payload = payload;
        }

        public ComEvent(EventType @event)
        {
            Timestamp = ((DateTimeOffset)DateTime.UtcNow).ToUnixTimeMilliseconds();
            Event = @event.ToString();
            Payload = new();
        }
    }
}
