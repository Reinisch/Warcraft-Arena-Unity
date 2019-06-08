using JetBrains.Annotations;
using UnityEngine;

namespace Client.UI
{
    public abstract class UIPanel : MonoBehaviour
    {
        [SerializeField, UsedImplicitly] private CanvasGroup panelCanvasGroup;

        internal void Initialize()
        {
            PanelInitialized();
        }

        internal void Deinitialize()
        {
            PanelDeinitialized();
        }

        internal void Show()
        {
            gameObject.SetActive(true);

            PanelShown();
        }

        internal void Hide()
        {
            gameObject.SetActive(false);

            PanelHidden();
        }

        internal void DoUpdate(int deltaTime)
        {
            PanelUpdated(deltaTime);
        }

        protected void UpdateInputState(bool interactable)
        {
            panelCanvasGroup.interactable = interactable;
        }

        protected virtual void PanelInitialized()
        {
        }

        protected virtual void PanelDeinitialized()
        {
        }

        protected virtual void PanelShown()
        {
        }

        protected virtual void PanelHidden()
        {
        }

        protected virtual void PanelUpdated(int deltaTime)
        {
        }
    }
}
