using System;
using Client.UI;
using Common;
using Core;
using UnityEngine;
using Zenject;

namespace Client
{
    public class UnitFramePresenter : Presenter<UnitFrame>
    {
        [Inject] private EventBus eventBus;
        [Inject] private BalanceReference balance;
        [Inject] private RenderingReference rendering;

        private readonly Action<EntityAttributes> onAttributeChangedAction;
        private readonly Action onUnitTargetChanged;
        private readonly Action onUnitDisplayPowerChanged;
        private readonly Action onUnitClassChanged;

        private UnitFramePresenter targetFrame;
        private BuffDisplayPresenter buffDisplayFrame;
        private ComboFramePresenter comboFrame;
        private Unit unit;

        public UnitFramePresenter()
        {
            onAttributeChangedAction = OnAttributeChanged;
            onUnitTargetChanged = OnUnitTargetChanged;
            onUnitDisplayPowerChanged = OnUnitDisplayPowerChanged;
            onUnitClassChanged = OnUnitClassChanged;
        }

        public void SetTargetFrame(UnitFramePresenter frame)
        {
            targetFrame = frame;

            targetFrame.SetUnit(unit?.Target);
        }

        public void SetBuffDisplayFrame(BuffDisplayPresenter presenter)
        {
            buffDisplayFrame = presenter;

            buffDisplayFrame.SetUnit(unit);
        }

        public void SetComboFrame(ComboFramePresenter presenter)
        {
            comboFrame = presenter;
        }

        public void SetUnit(Unit newUnit)
        {
            bool wasSet = unit != null;
            Unit oldUnit = unit;

            if (unit != null)
                DeinitializeUnit();

            if (newUnit != null)
                InitializeUnit(newUnit);

            if (unit != null)
                View.PlaySetSound(unit.Position);
            else if (wasSet)
                View.PlayLostSound(oldUnit.Position);

            View.SetVisible(unit != null);
        }

        private void InitializeUnit(Unit newUnit)
        {
            unit = newUnit;
            View.SetUnitName(unit.Name);

            comboFrame?.SetUnit(unit);
            targetFrame?.SetUnit(unit.Target);
            buffDisplayFrame?.SetUnit(unit);

            OnAttributeChanged(EntityAttributes.Health);
            OnAttributeChanged(EntityAttributes.Power);
            OnUnitClassChanged();
            OnUnitDisplayPowerChanged();

            eventBus.RegisterEvent(unit, GameEvents.UnitAttributeChanged, onAttributeChangedAction);
            eventBus.RegisterEvent(unit, GameEvents.UnitTargetChanged, onUnitTargetChanged);
            eventBus.RegisterEvent(unit, GameEvents.UnitClassChanged, onUnitClassChanged);
            eventBus.RegisterEvent(unit, GameEvents.UnitDisplayPowerChanged, onUnitDisplayPowerChanged);
        }

        private void DeinitializeUnit()
        {
            eventBus.UnregisterEvent(unit, GameEvents.UnitAttributeChanged, onAttributeChangedAction);
            eventBus.UnregisterEvent(unit, GameEvents.UnitTargetChanged, onUnitTargetChanged);
            eventBus.UnregisterEvent(unit, GameEvents.UnitClassChanged, onUnitClassChanged);
            eventBus.UnregisterEvent(unit, GameEvents.UnitDisplayPowerChanged, onUnitDisplayPowerChanged);

            comboFrame?.SetUnit(null);
            targetFrame?.SetUnit(null);
            buffDisplayFrame?.SetUnit(null);

            unit = null;
        }

        private void OnAttributeChanged(EntityAttributes attributeType)
        {
            if (attributeType == EntityAttributes.Health || attributeType == EntityAttributes.MaxHealth)
                View.SetHealthRatio(unit.HealthRatio);
            else if (attributeType == EntityAttributes.Power || attributeType == EntityAttributes.MaxPower)
                View.SetResourceRatio(Mathf.Clamp01((float)unit.Power / unit.MaxPower));
        }

        private void OnUnitTargetChanged()
        {
            targetFrame?.SetUnit(unit.Target);
        }

        private void OnUnitDisplayPowerChanged()
        {
            View.SetResourceColor(rendering.SpellPowerColors.Value(unit.DisplayPowerType));
        }

        private void OnUnitClassChanged()
        {
            View.SetClassIcon(rendering.ClassIconSprites.Value(unit.ClassType));

            if (balance.ClassesByType.TryGetValue(unit.ClassType, out ClassInfo classInfo))
                View.SetComboFrameEnabled(classInfo.HasPower(SpellPowerType.ComboPoints));
        }
    }
}
