using Core;

namespace Server
{
    public class PlayerServerInfo
    {
        public bool IsClient => BoltConnection != null;
        public bool IsServer => BoltConnection == null;

        public PlayerNetworkState NetworkState { get; set; }
        public int DisconnectTimeLeft { get; set; }
        public string UnityId { get; set; }

        public BoltConnection BoltConnection { get; private set; }
        public Player Player { get; private set; }

        public PlayerServerInfo(BoltConnection boltConneciton, Player player, string unityId)
        {
            NetworkState = PlayerNetworkState.Connected;
            BoltConnection = boltConneciton;
            UnityId = unityId;
            Player = player;
        }
    }
}