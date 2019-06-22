using System.Collections.Generic;
using Client.UI;
using Common;
using Core;
using JetBrains.Annotations;
using UnityEngine;

namespace Client
{
    public class BattleHudPanel : UIWindow<BattleScreen>
    {
        public struct RegisterToken : IPanelRegisterToken<BattleHudPanel>
        {
            public void Initialize(BattleHudPanel panel)
            {
                panel.gameObject.SetActive(false);
            }
        }

        public struct UnregisterToken : IPanelUnregisterToken<BattleHudPanel>
        {
            public void Deinitialize(BattleHudPanel panel)
            {
                panel.gameObject.SetActive(false);
            }
        }

        [SerializeField, UsedImplicitly] private PhotonBoltReference photon;
        [SerializeField, UsedImplicitly] private UnitFrame playerUnitFrame;
        [SerializeField, UsedImplicitly] private UnitFrame playerTargetUnitFrame;
        [SerializeField, UsedImplicitly] private UnitFrame playerTargetTargetUnitFrame;
        [SerializeField, UsedImplicitly] private CastFrame playerCastFrame;
        [SerializeField, UsedImplicitly] private List<ActionBar> actionBars;

        private Player localPlayer;

        protected override void PanelInitialized()
        {
            base.PanelInitialized();

            actionBars.ForEach(actionBar => actionBar.Initialize());

            EventHandler.RegisterEvent<Player>(photon, GameEvents.PlayerControlGained, OnPlayerControlGained);
            EventHandler.RegisterEvent<Player>(photon, GameEvents.PlayerControlLost, OnPlayerControlLost);

            playerUnitFrame.UpdateUnit(localPlayer);
            playerUnitFrame.SetTargetUnitFrame(playerTargetUnitFrame);
            playerTargetUnitFrame.SetTargetUnitFrame(playerTargetTargetUnitFrame);
        }

        protected override void PanelDeinitialized()
        {
            EventHandler.UnregisterEvent<Player>(photon, GameEvents.PlayerControlGained, OnPlayerControlGained);
            EventHandler.UnregisterEvent<Player>(photon, GameEvents.PlayerControlLost, OnPlayerControlLost);

            actionBars.ForEach(actionBar => actionBar.Denitialize());

            playerUnitFrame.UpdateUnit(null);
            playerTargetUnitFrame.UpdateUnit(null);
            playerTargetTargetUnitFrame.UpdateUnit(null);

            localPlayer = null;

            base.PanelDeinitialized();
        }

        protected override void PanelUpdated(float deltaTime)
        {
            base.PanelUpdated(deltaTime);

            playerCastFrame.DoUpdate(deltaTime);

            if (localPlayer != null)
                foreach (var actionBar in actionBars)
                    actionBar.DoUpdate(deltaTime);
        }

        private void OnPlayerControlGained(Player player)
        {
            localPlayer = player;

            playerUnitFrame.UpdateUnit(localPlayer);
            playerCastFrame.UpdateUnit(localPlayer);
        }

        private void OnPlayerControlLost(Player player)
        {
            playerUnitFrame.UpdateUnit(null);
            playerCastFrame.UpdateUnit(null);

            localPlayer = null;
        }
    }
}
