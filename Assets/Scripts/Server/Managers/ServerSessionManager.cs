using Core;

namespace Server
{
    public class ServerSessionManager : SingletonGameObject<ServerSessionManager>
    {
        private readonly ServerSessionConstroller sessionController = new ServerSessionConstroller();

        public override void Initialize()
        {
            base.Initialize();

            sessionController.Initialize(MultiplayerManager.Instance.NetworkingType);
            MultiplayerManager.Instance.EventReceivedFromClient += OnMultiplayerManagerReceivedFromClient;
        }

        public override void Deinitialize()
        {
            MultiplayerManager.Instance.EventReceivedFromClient -= OnMultiplayerManagerReceivedFromClient;
            sessionController.Deinitialize();

            base.Deinitialize();
        }

        private void OnMultiplayerManagerReceivedFromClient(ClientPacket packet, WorldSession session)
        {
            sessionController.HandlePacket(session, packet);
        }
    }
}
