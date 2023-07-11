using System;
using System.Collections.Generic;
using UdpKit.Platform.Photon;
using UdpKit;

namespace Core
{
    public static class MultiplayerUtils
    {
        public static readonly IReadOnlyList<PhotonRegion> AvailableRegions = new List<PhotonRegion>
        {
            PhotonRegion.regions[PhotonRegion.Regions.EU],
            PhotonRegion.regions[PhotonRegion.Regions.ASIA],
            PhotonRegion.regions[PhotonRegion.Regions.CAE],
            PhotonRegion.regions[PhotonRegion.Regions.IN],
            PhotonRegion.regions[PhotonRegion.Regions.JP],
            PhotonRegion.regions[PhotonRegion.Regions.RU],
            PhotonRegion.regions[PhotonRegion.Regions.RUE],
            PhotonRegion.regions[PhotonRegion.Regions.SA],
            PhotonRegion.regions[PhotonRegion.Regions.KR],
            PhotonRegion.regions[PhotonRegion.Regions.US],
            PhotonRegion.regions[PhotonRegion.Regions.USW],
            PhotonRegion.regions[PhotonRegion.Regions.AU]
        };

        public static readonly List<string> AvailableRegionDescriptions = new List<string>
        {
            $"{PhotonRegion.regions[PhotonRegion.Regions.EU].Name} {PhotonRegion.regions[PhotonRegion.Regions.EU].City}",
            $"{PhotonRegion.regions[PhotonRegion.Regions.ASIA].Name} {PhotonRegion.regions[PhotonRegion.Regions.ASIA].City}",
            $"{PhotonRegion.regions[PhotonRegion.Regions.CAE].Name} {PhotonRegion.regions[PhotonRegion.Regions.CAE].City}",
            $"{PhotonRegion.regions[PhotonRegion.Regions.IN].Name} {PhotonRegion.regions[PhotonRegion.Regions.IN].City}",
            $"{PhotonRegion.regions[PhotonRegion.Regions.JP].Name} {PhotonRegion.regions[PhotonRegion.Regions.JP].City}",
            $"{PhotonRegion.regions[PhotonRegion.Regions.RU].Name} {PhotonRegion.regions[PhotonRegion.Regions.RU].City}",
            $"{PhotonRegion.regions[PhotonRegion.Regions.RUE].Name} {PhotonRegion.regions[PhotonRegion.Regions.RUE].City}",
            $"{PhotonRegion.regions[PhotonRegion.Regions.SA].Name} {PhotonRegion.regions[PhotonRegion.Regions.SA].City}",
            $"{PhotonRegion.regions[PhotonRegion.Regions.KR].Name} {PhotonRegion.regions[PhotonRegion.Regions.KR].City}",
            $"{PhotonRegion.regions[PhotonRegion.Regions.US].Name} {PhotonRegion.regions[PhotonRegion.Regions.US].City}",
            $"{PhotonRegion.regions[PhotonRegion.Regions.USW].Name} {PhotonRegion.regions[PhotonRegion.Regions.USW].City}",
            $"{PhotonRegion.regions[PhotonRegion.Regions.AU].Name} {PhotonRegion.regions[PhotonRegion.Regions.AU].City}",
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

        public static ClientConnectFailReason ToConnectFailReason(this ConnectRefusedReason refusedReason)
        {
            switch (refusedReason)
            {
                case ConnectRefusedReason.None:
                    return ClientConnectFailReason.ServerRefusedConnection;
                case ConnectRefusedReason.InvalidToken:
                    return ClientConnectFailReason.InvalidToken;
                case ConnectRefusedReason.InvalidVersion:
                    return ClientConnectFailReason.InvalidVersion;
                case ConnectRefusedReason.UnsupportedDevice:
                    return ClientConnectFailReason.UnsupportedDevice;
                default:
                    throw new ArgumentOutOfRangeException(nameof(refusedReason), refusedReason, "Unknown refuse reason!");
            }
        }
    }
}
