using System;

namespace Currycomb.AuthService.Configuration
{
    public class GatewayConfiguration
    {
        public Uri Uri { get; set; }
        public TimeSpan ReconnectDelay { get; set; }
    }

    public class BroadcastConfiguration
    {
        public Uri Uri { get; set; }
        public TimeSpan ReconnectDelay { get; set; }
    }

    public class AppConfiguration
    {
        public GatewayConfiguration Gateway { get; set; }
        public BroadcastConfiguration Broadcast { get; set; }
    }
}
