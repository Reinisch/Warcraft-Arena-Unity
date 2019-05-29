using System.Collections.Generic;
using Common;
using Core;
using UnityEngine;
using UnityEngine.Assertions;

namespace Server
{
    public class WorldServerManager : WorldManager
    {
        private readonly List<PlayerServerInfo> playerInfos = new List<PlayerServerInfo>();
        private readonly Dictionary<BoltConnection, PlayerServerInfo> playerInfosByConnection = new Dictionary<BoltConnection, PlayerServerInfo>();
        private readonly Dictionary<ulong, PlayerServerInfo> playerInfosByPlayerId = new Dictionary<ulong, PlayerServerInfo>();
        private const int DisconnectedPlayerDestroyTime = 10000;

        public WorldServerManager(bool hasClientLogic)
        {
            HasServerLogic = true;
            HasClientLogic = hasClientLogic;
        }

        public override void Dispose()
        {
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

        internal void EntityAttached(BoltEntity entity)
        {
            if (entity.prefabId == BoltPrefabs.MoveState)
            {
                entity.SetScopeAll(false);

                Player player = FindPlayer(entity.source);
                if (player == null)
                    Object.Destroy(entity.gameObject);
                else
                    player.Controller.AttachClientSideMoveState(entity);
            }
        }

        internal void EntityDetached(BoltEntity entity)
        {
            if (entity.prefabId == BoltPrefabs.MoveState)
            {
                Player player = FindPlayer(entity.source);
                if (player == null)
                    Object.Destroy(entity.gameObject);
                else
                    player.Controller.DetachClientSideMoveState(false);
            }

            if (playerInfosByPlayerId.ContainsKey(entity.networkId.PackedValue))
            {
                PlayerServerInfo removeInfo = playerInfosByPlayerId[entity.networkId.PackedValue];
                playerInfos.Remove(removeInfo);
                playerInfosByPlayerId.Remove(entity.networkId.PackedValue);

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
            WorldEntity.CreateInfo playerCreateInfo = new WorldEntity.CreateInfo { Position = spawnPoint.position, Rotation = spawnPoint.rotation };
            Player newPlayer = UnitManager.Create<Player>(BoltPrefabs.PlayerMage, playerCreateInfo);

            if (boltConnection == null)
                newPlayer.BoltEntity.TakeControl();
            else
                newPlayer.BoltEntity.AssignControl(boltConnection);

            var newPlayerInfo = new PlayerServerInfo(boltConnection, newPlayer);
            playerInfos.Add(newPlayerInfo);
            playerInfosByPlayerId[newPlayer.NetworkId] = newPlayerInfo;
            if (boltConnection != null)
                playerInfosByConnection[boltConnection] = newPlayerInfo;
        }

        private Player FindPlayer(BoltConnection boltConnection)
        {
            return playerInfosByConnection.LookupEntry(boltConnection)?.Player;
        }
    }
}