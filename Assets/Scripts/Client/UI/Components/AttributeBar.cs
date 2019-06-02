using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;

public class AttributeBar : MonoBehaviour
{
    [SerializeField, UsedImplicitly] private Slider slider;

    public float Ratio { set => slider.value = value; }

    public void Initialize()
    {

    }

    public void Deinitialize()
    {

    }
}
