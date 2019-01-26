using Core;
using JetBrains.Annotations;
using UnityEngine;

namespace Client
{
    public class BattleScreen : MonoBehaviour
    {
        [SerializeField, UsedImplicitly] private UnitFrame playerUnitFrame;
        [SerializeField, UsedImplicitly] private UnitFrame playerTargetUnitFrame;

        private WorldManager worldManager;
        private PhotonBoltClientListener clientListener;

        public void Initialize(PhotonBoltManager photonManager, PhotonBoltClientListener clientListener)
        {
            this.clientListener = clientListener;

            gameObject.SetActive(false);

            playerUnitFrame.Initialize();
            playerTargetUnitFrame.Initialize();

            clientListener.EventPlayerControlGained += OnPlayerControlGained;
            clientListener.EventPlayerControlLost += OnPlayerControlLost;
        }       

        public void Deinitialize()
        {
            clientListener.EventPlayerControlGained -= OnPlayerControlGained;
            clientListener.EventPlayerControlLost -= OnPlayerControlLost;

            playerUnitFrame.Deinitialize();
            playerTargetUnitFrame.Deinitialize();

            gameObject.SetActive(false);

            clientListener = null;
        }

        public void InitializeWorld(WorldManager worldManager)
        {
            this.worldManager = worldManager;
        }

        public void DeinitializeWorld()
        {
            worldManager = null;
        }

        public void DoUpdate(int deltaTime)
        {
            if (clientListener.LocalPlayer != null)
            {
                playerUnitFrame.DoUpdate(deltaTime);
                playerTargetUnitFrame.DoUpdate(deltaTime);
            }
        }

        public void Show()
        {
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        private void OnPlayerControlGained()
        {
            playerUnitFrame.SetUnit(clientListener.LocalPlayer);
        }

        private void OnPlayerControlLost()
        {
            playerUnitFrame.SetUnit(null);
        }
    }
}
