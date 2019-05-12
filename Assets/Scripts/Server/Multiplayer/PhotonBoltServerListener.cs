using System.Collections.Generic;
using Core;
using JetBrains.Annotations;
using UnityEngine;

namespace Server
{
    public class PhotonBoltServerListener : PhotonBoltBaseListener
    {
        [SerializeField, UsedImplicitly] private int disconnectedPlayerDestroyTime = 10000;

        private readonly List<PlayerServerInfo> playerInfos = new List<PlayerServerInfo>();
        private readonly Dictionary<BoltConnection, PlayerServerInfo> playerInfosByConnection = new Dictionary<BoltConnection, PlayerServerInfo>();
        private readonly Dictionary<ulong, PlayerServerInfo> playerInfosByPlayerId = new Dictionary<ulong, PlayerServerInfo>();

        private new WorldServerManager worldManager;

        public void Initialize(WorldServerManager worldManager)
        {
            base.Initialize(worldManager);

            this.worldManager = worldManager;
        }

        public new void Deinitialize()
        {
            base.Deinitialize();

            playerInfos.Clear();
            playerInfosByConnection.Clear();
            playerInfosByPlayerId.Clear();

            worldManager = null;
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
                        worldManager.UnitManager.Destroy(playerInfos[i].Player);
                }
            }
        }

        public override void SceneLoadLocalDone(string map)
        {
            base.SceneLoadLocalDone(map);

            if (worldManager.HasClientLogic)
                CreatePlayer();
        }

        public override void SceneLoadRemoteDone(BoltConnection connection)
        {
            base.SceneLoadRemoteDone(connection);

            CreatePlayer(connection);
        }

        public override void Connected(BoltConnection boltConnection)
        {
            base.Connected(boltConnection);

            worldManager.SetScope(boltConnection, true);
        }

        public override void Disconnected(BoltConnection boltConnection)
        {
            base.Disconnected(boltConnection);

            if (playerInfosByConnection.ContainsKey(boltConnection))
            {
                playerInfosByConnection[boltConnection].NetworkState = PlayerNetworkState.Disconnected;
                playerInfosByConnection[boltConnection].DisconnectTimeLeft = disconnectedPlayerDestroyTime;
            }
        }

        public override void EntityAttached(BoltEntity entity)
        {
            base.EntityAttached(entity);

            if (entity.prefabId == BoltPrefabs.MoveState)
            {
                Player player = playerInfosByConnection.LookupEntry(entity.source)?.Player;
                entity.SetScopeAll(false);

                if (player == null)
                    Destroy(entity.gameObject);
                else
                    player.Controller.AttachClientSideMoveState(entity);
            }
        }

        public override void EntityDetached(BoltEntity entity)
        {
            base.EntityDetached(entity);

            if (entity.prefabId == BoltPrefabs.MoveState)
            {
                Player player = playerInfosByConnection.LookupEntry(entity.source)?.Player;
                if (player == null)
                    Destroy(entity.gameObject);
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

        private void CreatePlayer(BoltConnection boltConnection = null)
        {
            Map mainMap = worldManager.MapManager.FindMap(1);
            Transform spawnPoint = RandomHelper.GetRandomElement(mainMap.Settings.FindSpawnPoints(Team.Alliance));
            WorldEntity.CreateInfo playerCreateInfo = new WorldEntity.CreateInfo {Position = spawnPoint.position, Rotation = spawnPoint.rotation};
            Player newPlayer = worldManager.UnitManager.Create<Player>(BoltPrefabs.PlayerMage, playerCreateInfo);

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
    }
}
