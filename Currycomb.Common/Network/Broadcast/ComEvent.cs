using System;
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace Currycomb.Common.Network.Broadcast
{
    [DataContract(Name = "ComEvent")]
    public class ComEvent
    {
        [DataMember(Name = "ts")] public long Timestamp { get; set; }
        [DataMember(Name = "sub")] public string Subject { get; set; }
        [DataMember(Name = "msg")] public string Payload { get; set; }

        public ComEvent(EventType ev, string payload)
        {
            Timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            Subject = ev.ToSubject();
            Payload = payload;
        }

        public static ComEvent Create<T>(EventType ev, T payload)
            => new(ev, JsonConvert.SerializeObject(payload));

        public ComEvent(EventType ev)
        {
            Timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            Subject = ev.ToSubject();
            Payload = string.Empty;
        }

        public T DeserializePayload<T>()
            => JsonConvert.DeserializeObject<T>(Payload) ?? throw new NullReferenceException();

        public string Serialize()
            => JsonConvert.SerializeObject(this);

        public static ComEvent Deserialize(string serialized)
            => JsonConvert.DeserializeObject<ComEvent>(serialized) ?? throw new NullReferenceException();
    }
}
