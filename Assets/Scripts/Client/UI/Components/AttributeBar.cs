using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Client
{
    public class AttributeBar : UIBehaviour
    {
        [SerializeField, UsedImplicitly] private Slider slider;
        [SerializeField, UsedImplicitly] private Image fillImage;

        public float Ratio { set => slider.value = value; }
        public Image FillImage => fillImage;
    }
}