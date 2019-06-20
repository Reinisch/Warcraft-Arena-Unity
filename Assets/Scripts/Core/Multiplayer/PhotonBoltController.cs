using System;
using Bolt;
using UdpKit;
using Common;
using EventHandler = Common.EventHandler;

namespace Core
{
    public abstract class PhotonBoltController : GlobalEventListener, IPhotonBoltController
    {
        public Map<Guid, UdpSession> Sessions => BoltNetwork.SessionList;
        public abstract string Version { get; }

        internal void Register()
        {
            OnRegistered();
        }

        internal void Unregister()
        {
            OnUnregistered();
        }

        protected abstract void OnRegistered();

        protected abstract void OnUnregistered();

        public override void SessionListUpdated(Map<Guid, UdpSession> sessionList)
        {
            base.SessionListUpdated(sessionList);

            EventHandler.ExecuteEvent(this, GameEvents.SessionListUpdated);
        }

        public override bool PersistBetweenStartupAndShutdown()
        {
            return true;
        }

        public abstract void StartServer(ServerRoomToken serverToken, Action onStartSuccess, Action onStartFail);

        public abstract void StartConnection(UdpSession session, ClientConnectionToken token, Action onConnectSuccess, Action<ClientConnectFailReason> onConnectFail);

        public abstract void StartClient(Action onStartSuccess, Action onStartFail, bool forceRestart);
    }
}
