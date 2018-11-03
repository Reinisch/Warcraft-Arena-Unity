using Core;
using Core.Packets;
using JetBrains.Annotations;
using UnityEngine;

namespace Server
{
    [PacketHandlerContainer(NetworkingType.PhotonUnityNetworking)]
    public static class ConnectionHandlers
    {
        [UsedImplicitly, PacketHandler]
        private static void OnConnectionRequest(PhotonWorldSession session, ConnectionPacket packet)
        {
            Debug.Log($"Handling connection request! Player: {packet.PlayerName} connected!");
        }
    }
}