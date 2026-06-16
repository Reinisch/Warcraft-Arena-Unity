using Core;
using JetBrains.Annotations;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Client
{
    public class CastFrame : MonoBehaviour
    {
        [Inject] private CastFramePresenter presenter;

        [SerializeField, UsedImplicitly] private CanvasGroup canvasGroup;
        [SerializeField, UsedImplicitly] private TextMeshProUGUI spellLabel;
        [SerializeField, UsedImplicitly] private Image spellIcon;
        [SerializeField, UsedImplicitly] private Slider castSlider;

        internal CastFramePresenter Presenter => presenter;

        [UsedImplicitly]
        private void Awake()
        {
            presenter.Initialize(this);
        }

        public void UpdateCaster(Unit newCaster)
        {
            presenter.SetCaster(newCaster);
        }

        public void DoUpdate()
        {
            presenter.Tick();
        }

        public void SetVisible(bool visible)
        {
            canvasGroup.blocksRaycasts = visible;
            canvasGroup.interactable = visible;
            canvasGroup.alpha = visible ? 1.0f : 0.0f;
        }

        public void SetSpellLabel(string text)
        {
            spellLabel.text = text;
        }

        public void SetSpellIcon(Sprite sprite)
        {
            spellIcon.sprite = sprite;
        }

        public void SetCastProgress(float progress)
        {
            castSlider.value = progress;
        }
    }
}
