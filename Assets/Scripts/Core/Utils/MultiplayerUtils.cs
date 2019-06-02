using UdpKit;

namespace Core
{
    public static class MultiplayerUtils
    {
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
