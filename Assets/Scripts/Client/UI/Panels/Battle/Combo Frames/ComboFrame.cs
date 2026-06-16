using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using Zenject;

namespace Client
{
    public class ComboFrame : MonoBehaviour
    {
        [Inject] private ComboFramePresenter presenter;

        [SerializeField, UsedImplicitly] private Canvas canvas;
        [SerializeField, UsedImplicitly] private CanvasGroup canvasGroup;
        [SerializeField, UsedImplicitly] private List<ComboPointSlot> comboPointSlots;

        public Canvas Canvas => canvas;

        public int ComboPointSlotCount => comboPointSlots.Count;

        internal ComboFramePresenter Presenter => presenter;

        [UsedImplicitly]
        private void Awake()
        {
            presenter.Initialize(this);
        }

        public void SetVisible(bool visible)
        {
            canvasGroup.blocksRaycasts = visible;
            canvasGroup.interactable = visible;
            canvasGroup.alpha = visible ? 1.0f : 0.0f;
        }

        public void SetComboPointActive(int index, bool active)
        {
            comboPointSlots[index].ModifyState(active);
        }
    }
}
