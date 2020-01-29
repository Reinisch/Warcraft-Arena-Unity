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

        [SerializeField, UsedImplicitly] private CanvasGroup canvasGroup;
        [SerializeField, UsedImplicitly] private UnitFrame playerUnitFrame;
        [SerializeField, UsedImplicitly] private UnitFrame playerTargetUnitFrame;
        [SerializeField, UsedImplicitly] private UnitFrame playerTargetTargetUnitFrame;
        [SerializeField, UsedImplicitly] private BuffDisplayFrame playerBuffDisplayFrame;
        [SerializeField, UsedImplicitly] private BuffDisplayFrame targetBuffDisplayFrame;
        [SerializeField, UsedImplicitly] private CastFrame playerCastFrame;
        [SerializeField, UsedImplicitly] private ActionErrorDisplay actionErrorDisplay;
        [SerializeField, UsedImplicitly] private List<ActionBar> actionBars;

        private Player localPlayer;

        protected override void PanelInitialized()
        {
            base.PanelInitialized();

            canvasGroup.alpha = 0.0f;
            actionBars.ForEach(actionBar => actionBar.Initialize());
            actionErrorDisplay.Initialize();

            EventHandler.RegisterEvent<Player, bool>(GameEvents.ClientControlStateChanged, OnControlStateChanged);

            playerCastFrame.UpdateCaster(localPlayer);
            playerUnitFrame.UpdateUnit(localPlayer);
            playerUnitFrame.SetTargetUnitFrame(playerTargetUnitFrame);
            playerUnitFrame.SetBuffDisplayFrame(playerBuffDisplayFrame);
            playerTargetUnitFrame.SetTargetUnitFrame(playerTargetTargetUnitFrame);
            playerTargetUnitFrame.SetBuffDisplayFrame(targetBuffDisplayFrame);
        }

        protected override void PanelDeinitialized()
        {
            EventHandler.UnregisterEvent<Player, bool>(GameEvents.ClientControlStateChanged, OnControlStateChanged);

            actionErrorDisplay.Deinitialize();
            actionBars.ForEach(actionBar => actionBar.Denitialize());

            playerUnitFrame.UpdateUnit(null);
            playerTargetUnitFrame.UpdateUnit(null);
            playerBuffDisplayFrame.UpdateUnit(null);
            playerTargetTargetUnitFrame.UpdateUnit(null);
            targetBuffDisplayFrame.UpdateUnit(null);
            playerCastFrame.UpdateCaster(null);

            localPlayer = null;

            base.PanelDeinitialized();
        }

        protected override void PanelUpdated(float deltaTime)
        {
            base.PanelUpdated(deltaTime);

            playerCastFrame.DoUpdate();
            actionErrorDisplay.DoUpdate(deltaTime);
            playerBuffDisplayFrame.DoUpdate();
            targetBuffDisplayFrame.DoUpdate();

            if (localPlayer != null)
                foreach (var actionBar in actionBars)
                    actionBar.DoUpdate(deltaTime);
        }

        private void OnControlStateChanged(Player player, bool underControl)
        {
            if (underControl)
            {
                localPlayer = player;
                canvasGroup.alpha = 1.0f;

                OnPlayerClassChanged();

                playerUnitFrame.UpdateUnit(localPlayer);
                playerCastFrame.UpdateCaster(localPlayer);

                EventHandler.RegisterEvent(localPlayer, GameEvents.UnitClassChanged, OnPlayerClassChanged);
            }
            else
            {
                EventHandler.UnregisterEvent(localPlayer, GameEvents.UnitClassChanged, OnPlayerClassChanged);

                playerUnitFrame.UpdateUnit(null);
                playerCastFrame.UpdateCaster(null);

                canvasGroup.alpha = 0.0f;
                localPlayer = null;
            }
        }

        private void OnPlayerClassChanged()
        {
            foreach (var actionBar in actionBars)
                actionBar.ModifyContent(localPlayer.ClassType);
        }
    }
}
