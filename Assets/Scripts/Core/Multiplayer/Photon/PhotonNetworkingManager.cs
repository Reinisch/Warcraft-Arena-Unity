using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using Core.Packets;
using ExitGames.Client.Photon;
using UnityEngine;
using JetBrains.Annotations;
using Photon;

namespace Core
{
    public class PhotonNetworkingManager : PunBehaviour, ISingletonGameObject
    {
        [SerializeField, UsedImplicitly] private ServerSettings photonServerSettings;
        [SerializeField, UsedImplicitly] private string gameVersion = "0.0.1";
        [SerializeField, UsedImplicitly] private byte quickResends = 3;
        [SerializeField, UsedImplicitly] private byte maxResendsBeforeDisconnect = 10;
        [SerializeField, UsedImplicitly] private byte maxPlayersPerRoom = 4;
        [SerializeField, UsedImplicitly] private PhotonLogLevel logLevel = PhotonLogLevel.Informational;
        [SerializeField, UsedImplicitly] private List<CloudRegionCode> availableRegions = PhotonHelper.FullAvailableRegions;

        private readonly Dictionary<int, PhotonWorldSession> photonSessions = new Dictionary<int, PhotonWorldSession>();
        private static readonly ThreadLocal<Protocol16> PhotonProtocol = new ThreadLocal<Protocol16>(() => new Protocol16());

        private CloudRegionCode selectedRegion = CloudRegionCode.eu;
        private bool isRandomConnecting;
        private bool isNamedConnecting;
        private string targetRoomName;

        public void Initialize()
        {
            PhotonNetwork.PhotonServerSettings = photonServerSettings;
            PhotonNetwork.MaxResendsBeforeDisconnect = maxResendsBeforeDisconnect;
            PhotonNetwork.QuickResends = quickResends;

            PhotonNetwork.logLevel = logLevel;
            PhotonNetwork.autoJoinLobby = true;
            PhotonNetwork.automaticallySyncScene = false;

            PhotonPeer.RegisterType(typeof(PhotonPacketContainer), 255, SerializeWorldPacket, DeserializeWorldPacket);

            MultiplayerManager.Instance.EventSendToTargets += OnMultiplayerManagerSendToTargets;
            MultiplayerManager.Instance.EventSendToTarget += OnMultiplayerManagerSendToTarget;

            PhotonNetwork.ConnectToRegion(selectedRegion, gameVersion);
        }

        public void Deinitialize()
        {
            MultiplayerManager.Instance.EventSendToTargets -= OnMultiplayerManagerSendToTargets;
            MultiplayerManager.Instance.EventSendToTarget -= OnMultiplayerManagerSendToTarget;

            DisconnectAll();

            if (PhotonNetwork.connected)
                PhotonNetwork.Disconnect();
        }

        public void DoUpdate(int deltaTime)
        {
        }

        #region Callbacks

        public override void OnConnectedToMaster()
        {
            Debug.Log("Photon Networking Manager: Connected to Master!");
        }

        public override void OnConnectedToPhoton()
        {
            Debug.Log("Photon Networking Manager: Connected to Photon!");
        }

        public override void OnJoinedLobby()
        {
            Debug.Log("Photon Networking Manager: Joined Lobby!");

            if (GameManager.Instance.HasServerLogic)
            {
                if (!PhotonNetwork.CreateRoom("Development Room", new RoomOptions {MaxPlayers = maxPlayersPerRoom}, null))
                {
                    isRandomConnecting = false;
                    isNamedConnecting = false;
                }
            }
            else if (GameManager.Instance.HasClientLogic)
            {
                if (isNamedConnecting)
                    PhotonNetwork.JoinRoom(targetRoomName);
                else
                    PhotonNetwork.JoinRandomRoom();
            }
        }

        public override void OnFailedToConnectToPhoton(DisconnectCause cause)
        {
            OnConnectionFail(cause);
        }

        public override void OnDisconnectedFromPhoton()
        {
            Debug.Log("Photon Networking Manager: Disconected from Photon!");

            if (isNamedConnecting || isRandomConnecting)
            {
                isRandomConnecting = false;
                isNamedConnecting = false;
            }
        }

        public override void OnPhotonRandomJoinFailed(object[] codeAndMsg)
        {
            Debug.Log("Photon Networking Manager: Random join failed!");

            // #Critical: we failed to join a random room,
            // Maybe none exists or they are all full. No worries, we create a new room.
            if (!GameManager.Instance.HasServerLogic || !PhotonNetwork.CreateRoom("Development Room", new RoomOptions { MaxPlayers = maxPlayersPerRoom }, null))
            {
                isRandomConnecting = false;
                isNamedConnecting = false;
            }
        }

        public override void OnPhotonCreateRoomFailed(object[] codeAndMsg)
        {
            isRandomConnecting = false;
            isNamedConnecting = false;
        }

        public override void OnConnectionFail(DisconnectCause cause)
        {
            switch (cause)
            {
                case DisconnectCause.DisconnectByClientTimeout:
                case DisconnectCause.DisconnectByServerTimeout:
                    break;
                case DisconnectCause.MaxCcuReached:
                    break;
                case DisconnectCause.InvalidRegion:
                    break;
            }

            if (isNamedConnecting || isRandomConnecting)
            {
                targetRoomName = null;

                isRandomConnecting = false;
                isNamedConnecting = false;
            }

            DisconnectAll();
        }

        public override void OnPhotonJoinRoomFailed(object[] codeAndMsg)
        {
            Debug.Log("Photon Networking Manager: Target join failed!");
            isNamedConnecting = false;

            // #Critical: we failed to join a room
            isRandomConnecting = false;
            isNamedConnecting = false;
        }

        public override void OnJoinedRoom()
        {
            Debug.Log($"Photon Networking Manager: Joined room as player with Id: {PhotonNetwork.player.ID}!");

            if (GameManager.Instance.HasClientLogic)
                MultiplayerManager.Instance.SendToTargets(new ConnectionPacket { PlayerName = "Developer" }, PacketTargets.Server);
        }

        public override void OnLeftRoom()
        {
            base.OnLeftRoom();

            DisconnectAll();
        }

        public override void OnCreatedRoom()
        {
            Debug.Log($"Photon Networking Manager: Created room as player with Id: {PhotonNetwork.player.ID}!");

            if (GameManager.Instance.HasServerLogic)
            {
                photonSessions[PhotonNetwork.player.ID] = new PhotonWorldSession(PhotonNetwork.player);
                MultiplayerManager.Instance.PlayerConnected(photonSessions[PhotonNetwork.player.ID]);
            }
        }

        public override void OnPhotonPlayerConnected(PhotonPlayer newPlayer)
        {
            base.OnPhotonPlayerConnected(newPlayer);
            Debug.Log($"Player with Id: {newPlayer.ID} joined the game!");

            if (GameManager.Instance.HasServerLogic)
            {
                photonSessions[newPlayer.ID] = new PhotonWorldSession(newPlayer);
                MultiplayerManager.Instance.PlayerConnected(photonSessions[PhotonNetwork.player.ID]);
            }
        }

        public override void OnPhotonPlayerDisconnected(PhotonPlayer oldPlayer)
        {
            base.OnPhotonPlayerConnected(oldPlayer);
            Debug.Log($"Player with Id: {oldPlayer.ID} left the game!");

            if (GameManager.Instance.HasServerLogic)
            {
                MultiplayerManager.Instance.PlayerDisconnected(photonSessions[oldPlayer.ID]);
                photonSessions.Remove(oldPlayer.ID);
            }
        }

        #endregion

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
            if (PhotonNetwork.connected)
            {
                // #Critical: We need at this point to attempt joining a Random Room.
                // If it fails, we'll get notified in OnPhotonRandomJoinFailed() and we'll create one.
                PhotonNetwork.JoinRandomRoom();
            }
            else
            {
                // #Critical, we must first and foremost connect to Photon Online Server.
                //PhotonNetwork.ConnectUsingSettings(GameVersion);
                PhotonNetwork.ConnectToRegion(selectedRegion, gameVersion);
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

            // We check if we are connected or not, we join if we are , else we initiate the connection to the server.
            if (PhotonNetwork.connected)
            {
                // #Critical: We need at this point to attempt joining a Random Room.
                // If it fails, we'll get notified in OnPhotonRandomJoinFailed() and we'll create one.
                PhotonNetwork.JoinRoom(targetRoom.Name);
            }
            else
            {
                // #Critical, we must first and foremost connect to Photon Online Server.
                //PhotonNetwork.ConnectUsingSettings(GameVersion);
                PhotonNetwork.ConnectToRegion(selectedRegion, gameVersion);
            }
        }

        public void ConnectToMaster()
        {
            if (!PhotonNetwork.connected)
                PhotonNetwork.ConnectToRegion(selectedRegion, gameVersion);
        }

        public bool CreateNamedRoom(string roomName)
        {
            isNamedConnecting = true;
            targetRoomName = roomName;

            if (!PhotonNetwork.connected)
            {
                return PhotonNetwork.ConnectToRegion(selectedRegion, gameVersion);
            }

            return PhotonNetwork.CreateRoom(roomName, new RoomOptions { MaxPlayers = maxPlayersPerRoom }, null);
        }

        public void NextRegion()
        {
            int targetIndex = availableRegions.IndexOf(selectedRegion) + 1;
            if (targetIndex > availableRegions.Count - 1)
                targetIndex = 0;

            selectedRegion = availableRegions[targetIndex];

            if (PhotonNetwork.connected)
                PhotonNetwork.Disconnect();
        }

        public void PrevRegion()
        {
            int targetIndex = availableRegions.IndexOf(selectedRegion) - 1;
            if (targetIndex < 0)
                targetIndex = availableRegions.Count - 1;

            selectedRegion = availableRegions[targetIndex];

            if (PhotonNetwork.connected)
                PhotonNetwork.Disconnect();
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
            PhotonTargets photonTargets;

            switch (targets)
            {
                case PacketTargets.All:
                    photonTargets = PhotonTargets.All;
                    break;
                case PacketTargets.Others:
                    photonTargets = PhotonTargets.Others;
                    break;
                case PacketTargets.Server:
                    photonTargets = PhotonTargets.MasterClient;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(targets), targets, $"Invalid packet target type: {targets}");
            }

            photonView.RPC("OnWorldPacketReceived", photonTargets, new PhotonPacketContainer(packet));
        }

        private void OnMultiplayerManagerSendToTarget(WorldPacket packet, WorldSession target)
        {
            if (photonSessions.ContainsKey(target.Id))
                photonView.RPC("OnWorldPacketReceived", photonSessions[target.Id].PhotonPlayer, new PhotonPacketContainer(packet));
            else
                Debug.LogError($"Attempted to send {packet} to player: {target.Id}, but session with this Id not found!");
        }

        [UsedImplicitly, PunRPC]
        private void OnWorldPacketReceived(PhotonPacketContainer packetContainer, PhotonMessageInfo messageInfo)
        {
            if (packetContainer.Packet is ServerPacket)
            {
                Debug.Log($"Received {packetContainer.Packet} from server: {messageInfo.sender.ID}!");

                if (photonSessions.ContainsKey(messageInfo.sender.ID))
                    MultiplayerManager.Instance.ReceivedFromServer((ServerPacket)packetContainer.Packet);
                else
                    Debug.LogError($"Received {packetContainer.Packet} from player: {messageInfo.sender.ID}, but session with this Id not found!");
            }
            else
            {
                Debug.Log($"Received {packetContainer.Packet} from player: {messageInfo.sender.ID}!");

                if (photonSessions.ContainsKey(messageInfo.sender.ID))
                    MultiplayerManager.Instance.ReceivedFromClient((ClientPacket)packetContainer.Packet, photonSessions[messageInfo.sender.ID]);
                else
                    Debug.LogError($"Received {packetContainer.Packet} from player: {messageInfo.sender.ID}, but session with this Id not found!");
            }
        }

        #endregion

        private static short SerializeWorldPacket(StreamBuffer outStream, object customObject)
        {
            long initialLength = outStream.Length;
            var packetContainer = (PhotonPacketContainer)customObject;
            PhotonProtocol.Value.Serialize(outStream, packetContainer.Packet is ServerPacket, false);
            PhotonProtocol.Value.Serialize(outStream, packetContainer.Packet.OpCodeIndex, false);
            packetContainer.Packet.Serialize(outStream, PhotonProtocol.Value);
            return (short)(outStream.Position - initialLength);
        }

        private static object DeserializeWorldPacket(StreamBuffer inStream, short length)
        {
            bool isServerPacket = (byte)PhotonProtocol.Value.Deserialize(inStream, (byte)Protocol16.GpType.Byte) == 1;
            int opCode = (int)PhotonProtocol.Value.Deserialize(inStream, (byte)Protocol16.GpType.Integer);

            if (MultiplayerManager.PacketTypes[isServerPacket].ContainsKey(opCode))
            {
                var packet = new PhotonPacketContainer((WorldPacket)Activator.CreateInstance(MultiplayerManager.PacketTypes[isServerPacket][opCode]));
                packet.Packet.Deserialize(inStream, PhotonProtocol.Value);
                return packet;
            }

            Debug.LogError($"Invalid opcode in packet deserialization: {opCode} IsServerPacket: {isServerPacket}");
            return null;
        }
    }
}
