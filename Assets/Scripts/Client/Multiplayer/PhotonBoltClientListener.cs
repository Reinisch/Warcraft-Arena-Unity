using Core;
using UnityEngine;

namespace Client
{
    public class PhotonBoltClientListener : PhotonBoltBaseListener
    {
        public override void SceneLoadLocalDone(string map)
        {
            base.SceneLoadLocalDone(map);

            Map mainMap = MapManager.Instance.FindMap(1);
            Transform spawnPoint = RandomHelper.GetRandomElement(mainMap.Settings.FindSpawnPoints(Team.Alliance));
            WorldEntity.CreateInfo playerCreateInfo = new WorldEntity.CreateInfo {Position = spawnPoint.position, Rotation = spawnPoint.rotation};
            Player localPlayer = worldManager.WorldEntityManager.Create<Player>(BoltPrefabs.PlayerMage, playerCreateInfo);
            localPlayer.SetMap(mainMap);

            localPlayer.GetComponent<WarcraftController>().Initialize(localPlayer);
            FindObjectOfType<WarcraftCamera>().Target = localPlayer.transform;
        }
    }
}
