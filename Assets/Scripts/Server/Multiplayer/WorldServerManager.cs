using System;
using Core;
using UnityEngine;

namespace Server
{
    public class WorldServerManager : WorldManager
    {
        private Guid localPlayerId = Guid.Empty;

        public override Guid LocalPlayerId => localPlayerId;

        public override void Initialize()
        {
            base.Initialize();

            Map mainMap = MapManager.Instance.FindMap(1);
            Transform spawnPoint = RandomHelper.GetRandomElement(mainMap.Settings.FindSpawnPoints(Team.Alliance));
            Player localPlayer = EntityManager.Instance.SpawnEntity<Player>(EntityType.Player, new EntitySpawnData(spawnPoint.position, spawnPoint.rotation));
            localPlayer.SetMap(mainMap);

            localPlayerId = localPlayer.Guid;
        }
    }
}