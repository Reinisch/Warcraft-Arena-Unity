using System.Collections.Generic;
using Client.UI;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Client
{
    public class BattleHudPanel : UIPanel<BattleScreen>
    {
        [Inject] private BattleHudPresenter presenter;

        [SerializeField, UsedImplicitly] private CanvasGroup canvasGroup;
        [SerializeField, UsedImplicitly] private UnitFrame playerUnitFrame;
        [SerializeField, UsedImplicitly] private Crosshair crosshairFrame;
        [SerializeField, UsedImplicitly] private UnitFrame playerTargetUnitFrame;
        [SerializeField, UsedImplicitly] private UnitFrame playerTargetTargetUnitFrame;
        [SerializeField, UsedImplicitly] private BuffDisplayFrame playerBuffDisplayFrame;
        [SerializeField, UsedImplicitly] private BuffDisplayFrame targetBuffDisplayFrame;
        [SerializeField, UsedImplicitly] private CastFrame playerCastFrame;
        [SerializeField, UsedImplicitly] private ActionErrorDisplay actionErrorDisplay;
        [SerializeField, UsedImplicitly] private List<ActionBar> actionBars;
        [SerializeField, UsedImplicitly] private Button openLobbyButton;

        public Button OpenLobbyButton => openLobbyButton;

        public void SetAlpha(float alpha)
        {
            canvasGroup.alpha = alpha;
        }

        public void SetCrosshairActive(bool active)
        {
            crosshairFrame.SetActive(active);
        }

        protected override void PanelInitialized()
        {
            base.PanelInitialized();

            presenter.Initialize(
                this,
                playerUnitFrame.Presenter,
                playerTargetUnitFrame.Presenter,
                playerTargetTargetUnitFrame.Presenter,
                playerBuffDisplayFrame.Presenter,
                targetBuffDisplayFrame.Presenter,
                playerCastFrame.Presenter,
                actionErrorDisplay.Presenter,
                actionBars);
        }

        protected override void PanelDeinitialized()
        {
            presenter.Deinitialize();

            base.PanelDeinitialized();
        }

        protected override void PanelUpdated(float deltaTime)
        {
            base.PanelUpdated(deltaTime);

            presenter.Tick(deltaTime);
        }
    }
}
