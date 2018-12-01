using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using Core.Packets;
using ExitGames.Client.Photon;
using UnityEngine;
using JetBrains.Annotations;
using Bolt;
using Bolt.photon;
using ExitGames.Client.Photon.LoadBalancing;

namespace Core
{
    public class PhotonNetworkingManager : MonoBehaviour, ISingletonGameObject
    {
        [SerializeField, UsedImplicitly] private string gameVersion = "0.0.1";
        [SerializeField, UsedImplicitly] private byte quickResends = 3;
        [SerializeField, UsedImplicitly] private byte maxResendsBeforeDisconnect = 10;
        [SerializeField, UsedImplicitly] private byte maxPlayersPerRoom = 4;

        private readonly Dictionary<int, PhotonWorldSession> photonSessions = new Dictionary<int, PhotonWorldSession>();

        private bool isRandomConnecting;
        private bool isNamedConnecting;
        private string targetRoomName;

        public void Initialize()
        {
            MultiplayerManager.Instance.EventSendToTargets += OnMultiplayerManagerSendToTargets;
            MultiplayerManager.Instance.EventSendToTarget += OnMultiplayerManagerSendToTarget;
        }

        public void Deinitialize()
        {
            MultiplayerManager.Instance.EventSendToTargets -= OnMultiplayerManagerSendToTargets;
            MultiplayerManager.Instance.EventSendToTarget -= OnMultiplayerManagerSendToTarget;

            DisconnectAll();

            if (BoltNetwork.isConnected)
                BoltNetwork.ShutdownImmediate();
        }

        public void DoUpdate(int deltaTime)
        {
        }

        #region Connection and Rooms

        /// <summary>
        /// Start the connection process. If already connected, we attempt joining a random room, else connect to Photon Cloud Network
        /// </summary>
        public void RandomConnect()
        {
            // keep track of the will to join a room, because when we come back from the game
            // we will get a callback that we are connected, so we need to know what to do then
            isRandomConnecting = true;

            // We check if we are connected or not, we join if we are , else we initiate the connection to the server.
            if (BoltNetwork.isConnected)
            {
                // #Critical: We need at this point to attempt joining a Random Room.
            }
            else
            {
                // #Critical, we must first and foremost connect to Photon Online Server.
            }
        }

        /// <summary>
        /// Start the connection process, if already connected, we attempt joining target room, else connect to Photon Cloud Network
        /// </summary>
        public void Connect(RoomInfo targetRoom)
        {
            // keep track of the will to join a room, because when we come back from the game
            // we will get a callback that we are connected, so we need to know what to do then
            isNamedConnecting = true;
            targetRoomName = targetRoom.Name;

        }

        private void DisconnectAll()
        {
            foreach (var sessionEntry in photonSessions)
                MultiplayerManager.Instance.PlayerDisconnected(sessionEntry.Value);

            photonSessions.Clear();
        }

        #endregion

        #region Events

        private void OnMultiplayerManagerSendToTargets(WorldPacket packet, PacketTargets targets)
        {
        }

        private void OnMultiplayerManagerSendToTarget(WorldPacket packet, WorldSession target)
        {
        }

        [UsedImplicitly]
        private void OnWorldPacketReceived(PhotonPacketContainer packetContainer)
        {
            if (packetContainer.Packet is ServerPacket)
                MultiplayerManager.Instance.ReceivedFromServer((ServerPacket)packetContainer.Packet);
        }

        #endregion
    }
}
