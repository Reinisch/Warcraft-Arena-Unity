using System.Collections.Generic;
using Client.UI;
using Common;
using Core;
using Zenject;

namespace Client
{
    public class BattleHudPresenter : Presenter<BattleHudPanel>
    {
        [Inject] private EventBus eventBus;
        [Inject] private PlayerManager playerManager;
        [Inject] private InterfaceReference interfaceModule;

        private UnitFramePresenter playerUnitFrame;
        private UnitFramePresenter playerTargetUnitFrame;
        private UnitFramePresenter playerTargetTargetUnitFrame;
        private BuffDisplayPresenter playerBuffDisplayFrame;
        private BuffDisplayPresenter targetBuffDisplayFrame;
        private CastFramePresenter playerCastFrame;
        private ActionErrorDisplayPresenter actionErrorDisplay;
        private List<ActionBar> actionBars;

        private MovementMode? activeMovementMode;

        public void Initialize(
            BattleHudPanel view,
            UnitFramePresenter playerUnitFrame,
            UnitFramePresenter playerTargetUnitFrame,
            UnitFramePresenter playerTargetTargetUnitFrame,
            BuffDisplayPresenter playerBuffDisplayFrame,
            BuffDisplayPresenter targetBuffDisplayFrame,
            CastFramePresenter playerCastFrame,
            ActionErrorDisplayPresenter actionErrorDisplay,
            List<ActionBar> actionBars)
        {
            base.Initialize(view);

            this.playerUnitFrame = playerUnitFrame;
            this.playerTargetUnitFrame = playerTargetUnitFrame;
            this.playerTargetTargetUnitFrame = playerTargetTargetUnitFrame;
            this.playerBuffDisplayFrame = playerBuffDisplayFrame;
            this.targetBuffDisplayFrame = targetBuffDisplayFrame;
            this.playerCastFrame = playerCastFrame;
            this.actionErrorDisplay = actionErrorDisplay;
            this.actionBars = actionBars;

            View.SetAlpha(0.0f);
            actionBars.ForEach(actionBar => actionBar.Initialize());
            actionErrorDisplay.Activate();

            playerManager.EventPlayerChanged += OnControlStateChanged;

            View.OpenLobbyButton.onClick.AddListener(OnOpenLobbyButtonClicked);

            playerUnitFrame.SetTargetFrame(playerTargetUnitFrame);
            playerUnitFrame.SetBuffDisplayFrame(playerBuffDisplayFrame);
            playerTargetUnitFrame.SetTargetFrame(playerTargetTargetUnitFrame);
            playerTargetUnitFrame.SetBuffDisplayFrame(targetBuffDisplayFrame);
        }

        public override void Deinitialize()
        {
            playerManager.EventPlayerChanged -= OnControlStateChanged;

            View.OpenLobbyButton.onClick.RemoveListener(OnOpenLobbyButtonClicked);

            actionErrorDisplay.Deactivate();
            actionBars.ForEach(actionBar => actionBar.Denitialize());

            playerUnitFrame.SetUnit(null);
            playerTargetUnitFrame.SetUnit(null);
            playerBuffDisplayFrame.SetUnit(null);
            playerTargetTargetUnitFrame.SetUnit(null);
            targetBuffDisplayFrame.SetUnit(null);
            playerCastFrame.SetCaster(null);

            base.Deinitialize();
        }

        public override void Tick(float deltaTime)
        {
            playerCastFrame.Tick();
            actionErrorDisplay.Tick(deltaTime);
            playerBuffDisplayFrame.Tick(deltaTime);
            targetBuffDisplayFrame.Tick(deltaTime);
            View.SetCrosshairActive(playerManager.Player is { MovementMode: MovementMode.Shooter });
            View.OpenLobbyButton.interactable = !interfaceModule.IsPanelShown<LobbyScreen, LobbyPanel>();

            if (playerManager.Player != null)
            {
                RefreshActionBarGroups();

                foreach (var actionBar in actionBars)
                    if (actionBar.IsActive)
                        actionBar.DoUpdate(deltaTime);
            }
        }

        private void RefreshActionBarGroups()
        {
            MovementMode movementMode = playerManager.Player.MovementMode;

            if (activeMovementMode == movementMode)
                return;

            activeMovementMode = movementMode;

            foreach (var actionBar in actionBars)
                actionBar.SetActive(actionBar.MovementMode == movementMode);
        }

        private void OnControlStateChanged(bool underControl)
        {
            if (underControl)
            {
                View.SetAlpha(1.0f);

                OnPlayerClassChanged();

                playerUnitFrame.SetUnit(playerManager.Player);
                playerCastFrame.SetCaster(playerManager.Player);

                eventBus.RegisterEvent(playerManager.Player, GameEvents.UnitClassChanged, OnPlayerClassChanged);
            }
            else
            {
                eventBus.UnregisterEvent(playerManager.Player, GameEvents.UnitClassChanged, OnPlayerClassChanged);

                playerUnitFrame.SetUnit(null);
                playerCastFrame.SetCaster(null);

                activeMovementMode = null;

                View.SetAlpha(0.0f);
            }
        }

        private void OnPlayerClassChanged()
        {
            foreach (var actionBar in actionBars)
                actionBar.ModifyContent(playerManager.Player.ClassType);
        }

        private void OnOpenLobbyButtonClicked()
        {
            interfaceModule.ShowScreen<LobbyScreen, LobbyPanel>();
        }
    }
}
