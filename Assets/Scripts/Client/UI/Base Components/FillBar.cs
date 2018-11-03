using UnityEngine;

public class FillBar : MonoBehaviour
{
    public UnitFrame UnitFrame { get; set; }

    public delegate void FillBarValueEvent();
    public event FillBarValueEvent ValueChanged;

    public void Initialize()
    {
        UnitFrame = gameObject.transform.GetComponentInParent<UnitFrame>();
        ValueChanged += OnValueChanged;
    }

    public void UpdateBar()
    {
        
    }
    public void SetAttribute()
    {
        OnValueChanged();
    }
    public void OnValueChanged()
    {
    }
}
