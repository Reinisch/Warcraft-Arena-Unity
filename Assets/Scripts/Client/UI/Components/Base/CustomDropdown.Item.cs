using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Client
{
    public partial class CustomDropdown
    {
        protected class Item : MonoBehaviour, IPointerEnterHandler, ICancelHandler
        {
            [SerializeField] private TMP_Text text;
            [SerializeField] private Image image;
            [SerializeField] private RectTransform rectTransform;
            [SerializeField] private Toggle toggle;

            public TMP_Text Text
            {
                get => text;
                set => text = value;
            }

            public Image Image
            {
                get => image;
                set => image = value;
            }

            public RectTransform RectTransform
            {
                get => rectTransform;
                set => rectTransform = value;
            }

            public Toggle Toggle
            {
                get => toggle;
                set => toggle = value;
            }

            public virtual void OnPointerEnter(PointerEventData eventData)
            {
                EventSystem.current.SetSelectedGameObject(gameObject);
            }

            public virtual void OnCancel(BaseEventData eventData)
            {
                TMP_Dropdown dropdown = GetComponentInParent<TMP_Dropdown>();
                if (dropdown) dropdown.Hide();
            }
        }
    }
}
