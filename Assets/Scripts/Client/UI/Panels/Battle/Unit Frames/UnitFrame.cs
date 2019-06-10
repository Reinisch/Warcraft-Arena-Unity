using Core;
using JetBrains.Annotations;
using UnityEngine;
using TMPro;

public class UnitFrame : MonoBehaviour
{
    [SerializeField, UsedImplicitly] private AttributeBar health;
    [SerializeField, UsedImplicitly] private AttributeBar mainResource;
    [SerializeField, UsedImplicitly] private TextMeshProUGUI unitName;

    private Unit unit;

    public void Initialize()
    {
        health.Initialize();
        mainResource.Initialize();

        gameObject.SetActive(false);
    }

    public void Deinitialize()
    {
        health.Deinitialize();
        mainResource.Deinitialize();
    }

    public void DoUpdate(float deltaTime)
    {
        if (unit != null)
        {
            health.Ratio = unit.HealthRatio;
            mainResource.Ratio = Mathf.Clamp01((float) unit.GetPower(SpellResourceType.Mana) / unit.GetMaxPower(SpellResourceType.Mana));
        }
    }

    public void SetUnit(Unit newUnit)
    {
        if (unit != null)
            DeinitializeUnit();

        if (newUnit != null)
            InitializeUnit(newUnit);

        gameObject.SetActive(unit != null);
    }

    private void InitializeUnit(Unit unit)
    {
        this.unit = unit;
        unitName.text = unit.Name;
    }

    private void DeinitializeUnit()
    {
        unit = null;
    }
}
