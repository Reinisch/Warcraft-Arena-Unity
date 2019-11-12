using System;
using Common;
using JetBrains.Annotations;
using UdpKit;
using UnityEngine;

namespace Core
{
    [CreateAssetMenu(fileName = "Photon Reference", menuName = "Game Data/Scriptable/Photon", order = 1)]
    public class PhotonBoltReference : ScriptableReference, IPhotonBoltController
    {
        [SerializeField, UsedImplicitly] private string containerTag;

        private PhotonBoltController underlyingController;

        public IPhotonBoltController UnderlyingController => underlyingController;
        public Map<Guid, UdpSession> Sessions => underlyingController.Sessions;
        public string Version => underlyingController.Version;

        protected override void OnRegistered()
        {
            underlyingController = GameObject.FindGameObjectWithTag(containerTag).GetComponent<PhotonBoltController>();
            underlyingController.Register();
        }

        protected override void OnUnregister()
        {
            underlyingController.Unregister();
            underlyingController = null;
        }

        public void StartServer(ServerRoomToken serverToken, bool withClientLogic, Action onStartSuccess, Action onStartFail)
        {
            underlyingController.StartServer(serverToken, withClientLogic, onStartSuccess, onStartFail);
        }

        public void StartSinglePlayer(ServerRoomToken serverToken, Action onStartSuccess, Action onStartFail)
        {
            underlyingController.StartSinglePlayer(serverToken, onStartSuccess, onStartFail);
        }

        public void StartConnection(UdpSession session, ClientConnectionToken token, Action onConnectSuccess, Action<ClientConnectFailReason> onConnectFail)
        {
            underlyingController.StartConnection(session, token, onConnectSuccess, onConnectFail);
        }

        public void StartClient(Action onStartSuccess, Action onStartFail, bool forceRestart)
        {
            underlyingController.StartClient(onStartSuccess, onStartFail, forceRestart);
        }
    }
}
