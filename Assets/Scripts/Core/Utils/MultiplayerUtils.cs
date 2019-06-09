using System.Collections.Generic;
using UdpKit;

namespace Core
{
    public static class MultiplayerUtils
    {
        public static readonly IReadOnlyList<string> FullAvailableRegions = new List<string>
        {
            "eu",
            "us",
            "asia",
            "jp",
            "au",
            "usw",
            "sa",
            "cae",
            "kr",
            "in",
            "ru",
            "rue",
        };

        public static DisconnectReason ToDisconnectReason(this UdpConnectionDisconnectReason udpReason)
        {
            switch (udpReason)
            {
                case UdpConnectionDisconnectReason.Unknown:
                    return DisconnectReason.Unknown;
                case UdpConnectionDisconnectReason.Timeout:
                    return DisconnectReason.Timeout;
                case UdpConnectionDisconnectReason.Error:
                    return DisconnectReason.Error;
                case UdpConnectionDisconnectReason.Disconnected:
                    return DisconnectReason.Disconnected;
                default:
                    return DisconnectReason.Unknown;
            }
        }
    }
}
