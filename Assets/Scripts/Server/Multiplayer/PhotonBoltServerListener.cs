using Core;
using UnityEngine;

namespace Server
{
    public class PhotonBoltServerListener : PhotonBoltBaseListener
    {
        public override void SceneLoadRemoteDone(BoltConnection connection)
        {
            base.SceneLoadRemoteDone(connection);

            Map mainMap = MapManager.Instance.FindMap(1);
            Transform spawnPoint = RandomHelper.GetRandomElement(mainMap.Settings.FindSpawnPoints(Team.Alliance));
            WorldEntity.CreateInfo playerCreateInfo = new WorldEntity.CreateInfo { Position = spawnPoint.position, Rotation = spawnPoint.rotation };
            Player localPlayer = worldManager.WorldEntityManager.Create<Player>(BoltPrefabs.PlayerMage, playerCreateInfo);
            localPlayer.SetMap(mainMap);
            localPlayer.BoltEntity.AssignControl(connection);
        }
    }
}
