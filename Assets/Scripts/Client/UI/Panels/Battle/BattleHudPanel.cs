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

        [SerializeField, UsedImplicitly] private UnitFrame playerUnitFrame;
        [SerializeField, UsedImplicitly] private UnitFrame playerTargetUnitFrame;
        [SerializeField, UsedImplicitly] private UnitFrame playerTargetTargetUnitFrame;
        [SerializeField, UsedImplicitly] private BuffDisplayFrame playerBuffDisplayFrame;
        [SerializeField, UsedImplicitly] private CastFrame playerCastFrame;
        [SerializeField, UsedImplicitly] private ActionErrorDisplay actionErrorDisplay;
        [SerializeField, UsedImplicitly] private List<ActionBar> actionBars;

        private Player localPlayer;

        protected override void PanelInitialized()
        {
            base.PanelInitialized();

            actionBars.ForEach(actionBar => actionBar.Initialize());
            actionErrorDisplay.Initialize();

            EventHandler.RegisterEvent<Player>(EventHandler.GlobalDispatcher, GameEvents.PlayerControlGained, OnPlayerControlGained);
            EventHandler.RegisterEvent<Player>(EventHandler.GlobalDispatcher, GameEvents.PlayerControlLost, OnPlayerControlLost);

            playerCastFrame.UpdateCaster(localPlayer);
            playerUnitFrame.UpdateUnit(localPlayer);
            playerUnitFrame.SetTargetUnitFrame(playerTargetUnitFrame);
            playerUnitFrame.SetBuffDIsplayFrame(playerBuffDisplayFrame);
            playerTargetUnitFrame.SetTargetUnitFrame(playerTargetTargetUnitFrame);
        }

        protected override void PanelDeinitialized()
        {
            EventHandler.UnregisterEvent<Player>(EventHandler.GlobalDispatcher, GameEvents.PlayerControlGained, OnPlayerControlGained);
            EventHandler.UnregisterEvent<Player>(EventHandler.GlobalDispatcher, GameEvents.PlayerControlLost, OnPlayerControlLost);

            actionErrorDisplay.Deinitialize();
            actionBars.ForEach(actionBar => actionBar.Denitialize());

            playerUnitFrame.UpdateUnit(null);
            playerTargetUnitFrame.UpdateUnit(null);
            playerBuffDisplayFrame.UpdateUnit(null);
            playerTargetTargetUnitFrame.UpdateUnit(null);
            playerCastFrame.UpdateCaster(null);

            localPlayer = null;

            base.PanelDeinitialized();
        }

        protected override void PanelUpdated(float deltaTime)
        {
            base.PanelUpdated(deltaTime);

            playerCastFrame.DoUpdate();
            actionErrorDisplay.DoUpdate(deltaTime);
            playerBuffDisplayFrame.DoUpdate(deltaTime);

            if (localPlayer != null)
                foreach (var actionBar in actionBars)
                    actionBar.DoUpdate(deltaTime);
        }

        private void OnPlayerControlGained(Player player)
        {
            localPlayer = player;

            playerUnitFrame.UpdateUnit(localPlayer);
            playerCastFrame.UpdateCaster(localPlayer);
        }

        private void OnPlayerControlLost(Player player)
        {
            playerUnitFrame.UpdateUnit(null);
            playerCastFrame.UpdateCaster(null);

            localPlayer = null;
        }
    }
}
