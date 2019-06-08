using System.Collections.Generic;
using Client.UI;
using JetBrains.Annotations;
using UnityEngine;

namespace Client
{
    public class BattleHudPanel : UIWindow
    {
        public struct RegisterToken : IPanelRegisterToken<BattleHudPanel>
        {
            private readonly PhotonBoltClientListener clientListener;

            public RegisterToken(PhotonBoltClientListener clientListener)
            {
                this.clientListener = clientListener;
            }

            public void Initialize(BattleHudPanel panel)
            {
                panel.clientListener = clientListener;
            }
        }

        public struct UnregisterToken : IPanelUnregisterToken<BattleHudPanel>
        {
            public void Deinitialize(BattleHudPanel panel)
            {
                panel.gameObject.SetActive(false);
            }
        }

        [SerializeField, UsedImplicitly] private UnitFrame playerUnitFrame;
        [SerializeField, UsedImplicitly] private UnitFrame playerTargetUnitFrame;
        [SerializeField, UsedImplicitly] private List<ActionBar> actionBars;

        private PhotonBoltClientListener clientListener;

        protected override void PanelInitialized()
        {
            base.PanelInitialized();

            playerUnitFrame.Initialize();
            playerTargetUnitFrame.Initialize();

            actionBars.ForEach(actionBar => actionBar.Initialize());

            clientListener.EventPlayerControlGained += OnPlayerControlGained;
            clientListener.EventPlayerControlLost += OnPlayerControlLost;
        }

        protected override void PanelDeinitialized()
        {
            clientListener.EventPlayerControlGained -= OnPlayerControlGained;
            clientListener.EventPlayerControlLost -= OnPlayerControlLost;

            actionBars.ForEach(actionBar => actionBar.Deinitialize());

            playerUnitFrame.Deinitialize();
            playerTargetUnitFrame.Deinitialize();

            base.PanelDeinitialized();
        }

        protected override void PanelUpdated(int deltaTime)
        {
            base.PanelUpdated(deltaTime);

            if (clientListener.LocalPlayer != null)
            {
                playerUnitFrame.DoUpdate(deltaTime);
                playerTargetUnitFrame.DoUpdate(deltaTime);

                foreach (var actionBar in actionBars)
                    actionBar.DoUpdate(deltaTime);
            }
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
