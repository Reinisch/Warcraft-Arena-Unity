using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class FillBar : MonoBehaviour
{
    public AttributePair AttributeRef { get; set; }
    public UnitFrame UnitFrame { get; set; }

    Slider slider;
    int lastValue;

    public delegate void FillBarValueEvent();
    public event FillBarValueEvent ValueChanged;

    public void Initialize()
    {
        slider = transform.GetComponent<Slider>();
        lastValue = int.MinValue;
        UnitFrame = gameObject.transform.GetComponentInParent<UnitFrame>();
        ValueChanged += OnValueChanged;
    }

    public void UpdateBar()
    {
        if (lastValue != AttributeRef.CurrentValue)
        {
            if (ValueChanged != null)
                ValueChanged();
        }
    }
    public void SetAttribute(AttributePair attributeRef)
    {
        AttributeRef = attributeRef;
        lastValue = int.MinValue;
        OnValueChanged();
    }
    public void OnValueChanged()
    {
        if (AttributeRef == null)
        {
            return;
        }

        lastValue = AttributeRef.CurrentValue;
        slider.value = (float)AttributeRef.CurrentValue / AttributeRef.MaximumValue;
    }
}
