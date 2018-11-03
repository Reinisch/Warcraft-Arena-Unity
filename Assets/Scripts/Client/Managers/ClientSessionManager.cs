using Core;

namespace Client
{
    public class ClientSessionManager : SingletonGameObject<ClientSessionManager>
    {
        private readonly ClientSessionConstroller sessionController = new ClientSessionConstroller();

        public override void Initialize()
        {
            base.Initialize();

            sessionController.Initialize(MultiplayerManager.Instance.NetworkingType);
            MultiplayerManager.Instance.EventReceivedFromServer += OnMultiplayerManagerReceivedFromServer;
        }

        public override void Deinitialize()
        {
            MultiplayerManager.Instance.EventReceivedFromServer -= OnMultiplayerManagerReceivedFromServer;
            sessionController.Deinitialize();

            base.Deinitialize();
        }

        private void OnMultiplayerManagerReceivedFromServer(ServerPacket serverPacket)
        {
            sessionController.HandlePacket(null, serverPacket);
        }
    }
}
