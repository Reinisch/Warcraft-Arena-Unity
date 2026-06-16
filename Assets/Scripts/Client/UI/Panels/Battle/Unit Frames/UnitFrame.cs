using Client.Sound;
using JetBrains.Annotations;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Zenject;

namespace Client
{
    public class UnitFrame : MonoBehaviour
    {
        [Inject] private UnitFramePresenter presenter;

        [SerializeField, UsedImplicitly] private CanvasGroup canvasGroup;
        [SerializeField, UsedImplicitly] private Image classIcon;
        [SerializeField, UsedImplicitly] private AttributeBar health;
        [SerializeField, UsedImplicitly] private AttributeBar mainResource;
        [SerializeField, UsedImplicitly] private ComboFrame comboFrame;
        [SerializeField, UsedImplicitly] private TextMeshProUGUI unitName;
        [SerializeField, UsedImplicitly] private SoundEntry setSound;
        [SerializeField, UsedImplicitly] private SoundEntry lostSound;

        internal UnitFramePresenter Presenter => presenter;

        [UsedImplicitly]
        private void Awake()
        {
            presenter.Initialize(this);
            presenter.SetComboFrame(comboFrame?.Presenter);
        }

        public void SetVisible(bool visible)
        {
            canvasGroup.blocksRaycasts = visible;
            canvasGroup.interactable = visible;
            canvasGroup.alpha = visible ? 1.0f : 0.0f;
        }

        public void SetUnitName(string name)
        {
            unitName.text = name;
        }

        public void SetHealthRatio(float ratio)
        {
            health.Ratio = ratio;
        }

        public void SetResourceRatio(float ratio)
        {
            mainResource.Ratio = ratio;
        }

        public void SetResourceColor(Color color)
        {
            mainResource.FillImage.color = color;
        }

        public void SetClassIcon(Sprite sprite)
        {
            classIcon.sprite = sprite;
        }

        public void SetComboFrameEnabled(bool enabled)
        {
            if (comboFrame != null)
                comboFrame.Canvas.enabled = enabled;
        }

        public void PlaySetSound(Vector3 position)
        {
            setSound?.Play(position);
        }

        public void PlayLostSound(Vector3 position)
        {
            lostSound?.Play(position);
        }
    }
}
