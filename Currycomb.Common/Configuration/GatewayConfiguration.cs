using System;

namespace Currycomb.Common.Configuration
{
    public class GatewayConfiguration
    {
        public string Host { get; set; }
        public ushort Port { get; set; }
        public TimeSpan ReconnectDelay { get; set; }
    }
}
