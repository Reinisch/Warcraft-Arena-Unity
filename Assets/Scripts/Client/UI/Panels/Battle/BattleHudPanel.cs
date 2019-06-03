using System.Collections.Generic;
using Client.UI;
using Common;
using Core;
using JetBrains.Annotations;
using UnityEngine;

namespace Client
{
    public class BattleHudPanel : UIWindow<BattlePanelType>
    {
        public struct InitData : IPanelInitData
        {
            private readonly PhotonBoltManager photonManager;
            private readonly PhotonBoltClientListener clientListener;

            public InitData(PhotonBoltManager photonManager, PhotonBoltClientListener clientListener)
            {
                this.photonManager = photonManager;
                this.clientListener = clientListener;
            }

            public void Process<TPanelType>(UIPanel<TPanelType> panel)
            {
                Assert.IsTrue(panel is BattleHudPanel);

                if (panel is BattleHudPanel hudPanel)
                {
                    hudPanel.photonManager = photonManager;
                    hudPanel.clientListener = clientListener;
                }
            }
        }

        [SerializeField, UsedImplicitly] private UnitFrame playerUnitFrame;
        [SerializeField, UsedImplicitly] private UnitFrame playerTargetUnitFrame;
        [SerializeField, UsedImplicitly] private List<ActionBar> actionBars;

        private PhotonBoltManager photonManager;
        private PhotonBoltClientListener clientListener;

        public override BattlePanelType PanelType => BattlePanelType.HudPanel;

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
