using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Client
{
    public class AttributeBar : UIBehaviour
    {
        [SerializeField, UsedImplicitly] private Slider slider;

        public float Ratio { set => slider.value = value; }
    }
}