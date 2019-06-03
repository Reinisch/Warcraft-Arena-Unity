using JetBrains.Annotations;
using UnityEngine;

namespace Client.UI
{
    public abstract class UIPanel<TPanelType> : MonoBehaviour, IPanel
    {
        [SerializeField, UsedImplicitly] private CanvasGroup panelCanvasGroup;

        public abstract TPanelType PanelType { get; }
        public GameObject GameObject => gameObject;

        internal void Initialize<TPanelInitData>(TPanelInitData initData = default) where TPanelInitData : struct, IPanelInitData
        {
            initData.Process(this);

            PanelInitialized();
        }

        internal void Deinitialize<TPanelDeinitData>(TPanelDeinitData deinitData = default) where TPanelDeinitData : struct, IPanelDeinitData
        {
            PanelDeinitialized();

            deinitData.Process(this);
        }

        internal void Show<TPanelShowData>(TPanelShowData showData = default) where TPanelShowData : struct, IPanelShowData
        {
            showData.Process(this);

            PanelShown();
        }

        internal void Hide<TPanelHideData>(TPanelHideData hideData = default) where TPanelHideData : struct, IPanelHideData
        {
            PanelHidden();

            hideData.Process(this);
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

        internal void DoUpdate(int deltaTime)
        {
            PanelUpdated(deltaTime);
        }
    }
}
