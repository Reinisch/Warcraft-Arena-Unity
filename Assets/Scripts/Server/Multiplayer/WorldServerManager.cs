using System.Collections.Generic;
using Common;
using Core;
using UnityEngine;

namespace Server
{
    public class WorldServerManager : WorldManager
    {
        private readonly List<PlayerServerInfo> playerInfos = new List<PlayerServerInfo>();
        private readonly Dictionary<BoltConnection, PlayerServerInfo> playerInfosByConnection = new Dictionary<BoltConnection, PlayerServerInfo>();
        private readonly Dictionary<ulong, PlayerServerInfo> playerInfosByPlayerId = new Dictionary<ulong, PlayerServerInfo>();
        private readonly GameSpellListener spellListener;

        private PlayerServerInfo serverPlayerInfo;
        private ServerRoomToken serverRoomToken;

        private const int DisconnectedPlayerDestroyTime = 10000;

        public WorldServerManager(bool hasClientLogic)
        {
            HasServerLogic = true;
            HasClientLogic = hasClientLogic;

            spellListener = new GameSpellListener(this);
        }

        public override void Dispose()
        {
            serverRoomToken = null;
            spellListener.Dispose();

            playerInfos.Clear();
            playerInfosByConnection.Clear();
            playerInfosByPlayerId.Clear();

            base.Dispose();
        }

        public override void DoUpdate(int deltaTime)
        {
            base.DoUpdate(deltaTime);

            for (int i = playerInfos.Count - 1; i >= 0; i--)
            {
                if (playerInfos[i].NetworkState == PlayerNetworkState.Disconnected)
                {
                    playerInfos[i].DisconnectTimeLeft -= deltaTime;
                    if (playerInfos[i].DisconnectTimeLeft <= 0)
                        UnitManager.Destroy(playerInfos[i].Player);
                }
            }
        }

        internal void SessionUpdated(ServerRoomToken sessionToken)
        {
            serverRoomToken = sessionToken;
        }

        internal void EntityAttached(BoltEntity entity)
        {
            if (entity.PrefabId == BoltPrefabs.MoveState)
            {
                entity.SetScopeAll(false);

                Player player = FindPlayer(entity.Source);
                if (player == null)
                    Object.Destroy(entity.gameObject);
                else
                    player.Controller.AttachClientSideMoveState(entity);
            }
        }

        internal void EntityDetached(BoltEntity entity)
        {
            if (entity.PrefabId == BoltPrefabs.MoveState)
            {
                Player player = FindPlayer(entity.Source);
                if (player == null)
                    Object.Destroy(entity.gameObject);
                else
                    player.Controller.DetachClientSideMoveState(false);
            }

            if (playerInfosByPlayerId.ContainsKey(entity.NetworkId.PackedValue))
            {
                PlayerServerInfo removeInfo = playerInfosByPlayerId[entity.NetworkId.PackedValue];
                playerInfos.Remove(removeInfo);
                playerInfosByPlayerId.Remove(entity.NetworkId.PackedValue);

                if (serverPlayerInfo == removeInfo)
                    serverPlayerInfo = null;

                if (removeInfo.IsClient)
                    playerInfosByConnection.Remove(removeInfo.BoltConnection);
            }
        }

        internal void SetScope(BoltConnection connection, bool inScope)
        {
            UnitManager.SetScope(connection, inScope);
        }

        internal void SetNetworkState(BoltConnection connection, PlayerNetworkState state)
        {
            Assert.IsTrue(playerInfosByConnection.ContainsKey(connection), $"Failed to change connection state for {connection.RemoteEndPoint}");
            if (!playerInfosByConnection.ContainsKey(connection))
                return;

            playerInfosByConnection[connection].NetworkState = state;
            if (state == PlayerNetworkState.Disconnected)
                playerInfosByConnection[connection].DisconnectTimeLeft = DisconnectedPlayerDestroyTime;
        }

        internal void CreatePlayer(BoltConnection boltConnection = null)
        {
            Map mainMap = MapManager.FindMap(1);
            Transform spawnPoint = RandomUtils.GetRandomElement(mainMap.Settings.FindSpawnPoints(Team.Alliance));

            string playerName;
            string unityId;
            if (boltConnection == null)
            {
                playerName = serverRoomToken.LocalPlayerName;
                unityId = SystemInfo.deviceUniqueIdentifier;
            }
            else
            {
                var connectionToken = (ClientConnectionToken) boltConnection.ConnectToken;
                playerName = connectionToken.Name;
                unityId = connectionToken.UnityId;
            }

            var playerCreateToken = new Player.CreateToken
            {
                Position = spawnPoint.position,
                Rotation = spawnPoint.rotation,
                DeathState = DeathState.Alive,
                PlayerName = playerName
            };

            Player newPlayer = UnitManager.Create<Player>(BoltPrefabs.PlayerMage, playerCreateToken);
            newPlayer.ProcessCreation();

            if (boltConnection == null)
                newPlayer.BoltEntity.TakeControl();
            else
                newPlayer.BoltEntity.AssignControl(boltConnection);

            var newPlayerInfo = new PlayerServerInfo(boltConnection, newPlayer, unityId);
            playerInfos.Add(newPlayerInfo);
            playerInfosByPlayerId[newPlayer.NetworkId] = newPlayerInfo;
            if (boltConnection != null)
                playerInfosByConnection[boltConnection] = newPlayerInfo;
            else
                serverPlayerInfo = newPlayerInfo;
        }

        public Player FindPlayer(BoltConnection boltConnection)
        {
            return boltConnection == null ? serverPlayerInfo?.Player : playerInfosByConnection.LookupEntry(boltConnection)?.Player;
        }
    }
}