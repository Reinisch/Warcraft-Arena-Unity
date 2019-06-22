using System;
using Core;
using JetBrains.Annotations;
using UnityEngine;
using TMPro;
using Common;

using EventHandler = Common.EventHandler;

public class UnitFrame : MonoBehaviour
{
    [SerializeField, UsedImplicitly] private AttributeBar health;
    [SerializeField, UsedImplicitly] private AttributeBar mainResource;
    [SerializeField, UsedImplicitly] private TextMeshProUGUI unitName;

    private readonly Action<EntityAttributes> onAttributeChangedAction;
    private readonly Action onUnitTargetChanged;
    private UnitFrame targetUnitFrame;
    private Unit unit;

    private UnitFrame()
    {
        onAttributeChangedAction = OnAttributeChanged;
        onUnitTargetChanged = OnUnitTargetChanged;
    }

    public void SetTargetUnitFrame(UnitFrame unitFrame)
    {
        targetUnitFrame = unitFrame;

        targetUnitFrame.UpdateUnit(unit?.Target);
    }

    public void UpdateUnit(Unit newUnit)
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
        targetUnitFrame?.UpdateUnit(unit.Target);

        EventHandler.RegisterEvent(unit, GameEvents.EntityAttributeChanged, onAttributeChangedAction);
        EventHandler.RegisterEvent(unit, GameEvents.UnitTargetChanged, onUnitTargetChanged);
    }

    private void DeinitializeUnit()
    {
        EventHandler.UnregisterEvent(unit, GameEvents.EntityAttributeChanged, onAttributeChangedAction);
        EventHandler.UnregisterEvent(unit, GameEvents.UnitTargetChanged, onUnitTargetChanged);

        targetUnitFrame?.UpdateUnit(null);
        unit = null;
    }

    private void OnAttributeChanged(EntityAttributes attributeType)
    {
        if (attributeType == EntityAttributes.Health || attributeType == EntityAttributes.MaxHealth)
            health.Ratio = unit.HealthRatio;
        else if (attributeType == EntityAttributes.Power || attributeType == EntityAttributes.MaxPower)
            mainResource.Ratio = Mathf.Clamp01((float)unit.Mana / unit.MaxMana);
    }

    private void OnUnitTargetChanged()
    {
        targetUnitFrame?.UpdateUnit(unit.Target);
    }
}
