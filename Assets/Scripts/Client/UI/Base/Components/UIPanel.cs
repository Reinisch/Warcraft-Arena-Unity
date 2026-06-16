using JetBrains.Annotations;
using UnityEngine;

namespace Client.UI
{
    public abstract class UIPanel: MonoBehaviour
    {
        [SerializeField, UsedImplicitly]
        private CanvasGroup panelCanvasGroup;

        public bool IsShown => gameObject.activeSelf;

        public void Show()
        {
            gameObject.SetActive(true);

            PanelShown();
        }

        public void Hide()
        {
            gameObject.SetActive(false);

            PanelHidden();
        }

        internal void DoUpdate(float deltaTime)
        {
            PanelUpdated(deltaTime);
        }

        protected void ModifyInteractable(bool interactable)
        {
            panelCanvasGroup.interactable = interactable;
        }

        protected virtual void PanelInitialized()
        {
            gameObject.SetActive(false);
        }

        protected virtual void PanelDeinitialized()
        {
            gameObject.SetActive(false);
        }

        protected virtual void PanelShown()
        {
        }

        protected virtual void PanelHidden()
        {
        }

        protected virtual void PanelUpdated(float deltaTime)
        {
        }
    }

    public abstract class UIPanel<T> : UIPanel where T : UIScreen<T>
    {
        protected T Screen { get; private set; }

        internal void Register(T screen)
        {
            Screen = screen;

            PanelInitialized();
        }

        internal void Unregister()
        {
            PanelDeinitialized();
        }
    }
}
