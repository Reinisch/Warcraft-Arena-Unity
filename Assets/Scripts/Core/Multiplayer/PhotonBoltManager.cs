using System;
using Bolt;
using UdpKit;

namespace Core
{
    public abstract class PhotonBoltManager : GlobalEventListener
    {
        public Map<Guid, UdpSession> Sessions => BoltNetwork.SessionList;

        public event Action EventSessionListUpdated;

        public override void SessionListUpdated(Map<Guid, UdpSession> sessionList)
        {
            base.SessionListUpdated(sessionList);

            EventSessionListUpdated?.Invoke();
        }

        public override bool PersistBetweenStartupAndShutdown()
        {
            return true;
        }

        public override void BoltStartBegin()
        {
            base.BoltStartBegin();

            BoltNetwork.RegisterTokenClass<Unit.CreateToken>();
            BoltNetwork.RegisterTokenClass<Player.CreateToken>();
        }

        public abstract void StartServer(ServerRoomToken serverToken, Action onStartSuccess, Action onStartFail);

        public abstract void StartConnection(UdpSession session, ClientConnectionToken token, Action onConnectSuccess, Action onConnectFail);

        public abstract void StartClient(Action onStartSuccess, Action onStartFail);
    }
}
