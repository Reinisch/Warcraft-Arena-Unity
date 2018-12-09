using Core;
using UnityEngine;

namespace Client
{
    public class PhotonBoltClientListener : PhotonBoltBaseListener
    {
        public override void SceneLoadLocalDone(string map)
        {
            base.SceneLoadLocalDone(map);

            if (worldManager.HasServerLogic)
            {
                Map mainMap = MapManager.Instance.FindMap(1);
                Transform spawnPoint = RandomHelper.GetRandomElement(mainMap.Settings.FindSpawnPoints(Team.Alliance));
                WorldEntity.CreateInfo playerCreateInfo = new WorldEntity.CreateInfo { Position = spawnPoint.position, Rotation = spawnPoint.rotation };
                Player localPlayer = worldManager.WorldEntityManager.Create<Player>(BoltPrefabs.PlayerMage, playerCreateInfo);
                localPlayer.SetMap(mainMap);
                localPlayer.BoltEntity.TakeControl();

                FindObjectOfType<WarcraftCamera>().Target = localPlayer.transform;
            }
        }

        public override void ControlOfEntityGained(BoltEntity entity)
        {
            base.ControlOfEntityGained(entity);

            if (!worldManager.HasServerLogic)
            {
                Player localPlayer = (Player)worldManager.WorldEntityManager.Find(entity.networkId.PackedValue);
                FindObjectOfType<WarcraftCamera>().Target = localPlayer.transform;
            }
        }
    }
}
