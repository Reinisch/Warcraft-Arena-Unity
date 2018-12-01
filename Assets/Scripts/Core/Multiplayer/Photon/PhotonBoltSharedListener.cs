using Bolt;
using UnityEngine;

namespace Core.Photon.Bolt
{
    public class PhotonBoltSharedListener : GlobalEventListener
    {
        public override void Connected(BoltConnection boltConnection)
        {
            Debug.Log($"Player with ConnectionId: {boltConnection.ConnectionId} joined the game!");

            if (GameManager.Instance.HasServerLogic)
                MultiplayerManager.Instance.PlayerConnected(new PhotonWorldSession(boltConnection));
        }

        public override void Disconnected(BoltConnection boltConnection)
        {
            Debug.Log($"Player with ConnectionId: {boltConnection.ConnectionId} left the game!");

            if (GameManager.Instance.HasServerLogic)
                MultiplayerManager.Instance.PlayerDisconnected(new PhotonWorldSession(boltConnection));
        }
    }
}
