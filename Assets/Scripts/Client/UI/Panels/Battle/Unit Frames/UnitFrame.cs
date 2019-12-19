using System;
using Core;
using JetBrains.Annotations;
using UnityEngine;
using TMPro;
using Common;
using UnityEngine.UI;

using EventHandler = Common.EventHandler;

namespace Client
{
    public class UnitFrame : MonoBehaviour
    {
        [SerializeField, UsedImplicitly] private BalanceReference balance;
        [SerializeField, UsedImplicitly] private RenderingReference rendering;
        [SerializeField, UsedImplicitly] private CanvasGroup canvasGroup;
        [SerializeField, UsedImplicitly] private Image classIcon;
        [SerializeField, UsedImplicitly] private AttributeBar health;
        [SerializeField, UsedImplicitly] private AttributeBar mainResource;
        [SerializeField, UsedImplicitly] private ComboFrame comboFrame;
        [SerializeField, UsedImplicitly] private TextMeshProUGUI unitName;
        [SerializeField, UsedImplicitly] private SoundEntry setSound;
        [SerializeField, UsedImplicitly] private SoundEntry lostSound;

        private readonly Action<EntityAttributes> onAttributeChangedAction;
        private readonly Action onUnitTargetChanged;
        private readonly Action onUnitDisplayPowerChanged;
        private readonly Action onUnitClassChanged;

        private UnitFrame targetUnitFrame;
        private BuffDisplayFrame unitBuffDisplayFrame;
        private Unit unit;

        private UnitFrame()
        {
            onAttributeChangedAction = OnAttributeChanged;
            onUnitTargetChanged = OnUnitTargetChanged;
            onUnitDisplayPowerChanged = OnUnitDisplayPowerChanged;
            onUnitClassChanged = OnUnitClassChanged;
        }

        public void SetTargetUnitFrame(UnitFrame unitFrame)
        {
            targetUnitFrame = unitFrame;

            targetUnitFrame.UpdateUnit(unit?.Target);
        }

        public void SetBuffDisplayFrame(BuffDisplayFrame buffDisplayFrame)
        {
            unitBuffDisplayFrame = buffDisplayFrame;

            unitBuffDisplayFrame.UpdateUnit(unit);
        }

        public void UpdateUnit(Unit newUnit)
        {
            bool wasSet = unit != null;

            if (unit != null)
                DeinitializeUnit();

            if (newUnit != null)
                InitializeUnit(newUnit);

            if (unit != null)
                setSound?.Play();
            else if (wasSet)
                lostSound?.Play();

            canvasGroup.blocksRaycasts = unit != null;
            canvasGroup.interactable = unit != null;
            canvasGroup.alpha = unit != null ? 1.0f : 0.0f;
        }

        private void InitializeUnit(Unit unit)
        {
            this.unit = unit;
            unitName.text = unit.Name;

            comboFrame?.UpdateUnit(unit);
            targetUnitFrame?.UpdateUnit(unit.Target);
            unitBuffDisplayFrame?.UpdateUnit(unit);

            OnAttributeChanged(EntityAttributes.Health);
            OnAttributeChanged(EntityAttributes.Power);
            OnUnitClassChanged();
            OnUnitDisplayPowerChanged();

            EventHandler.RegisterEvent(unit, GameEvents.UnitAttributeChanged, onAttributeChangedAction);
            EventHandler.RegisterEvent(unit, GameEvents.UnitTargetChanged, onUnitTargetChanged);
            EventHandler.RegisterEvent(unit, GameEvents.UnitClassChanged, onUnitClassChanged);
            EventHandler.RegisterEvent(unit, GameEvents.UnitDisplayPowerChanged, onUnitDisplayPowerChanged);
        }

        private void DeinitializeUnit()
        {
            EventHandler.UnregisterEvent(unit, GameEvents.UnitAttributeChanged, onAttributeChangedAction);
            EventHandler.UnregisterEvent(unit, GameEvents.UnitTargetChanged, onUnitTargetChanged);
            EventHandler.UnregisterEvent(unit, GameEvents.UnitClassChanged, onUnitClassChanged);
            EventHandler.UnregisterEvent(unit, GameEvents.UnitDisplayPowerChanged, onUnitDisplayPowerChanged);

            comboFrame?.UpdateUnit(null);
            targetUnitFrame?.UpdateUnit(null);
            unitBuffDisplayFrame?.UpdateUnit(null);

            unit = null;
        }

        private void OnAttributeChanged(EntityAttributes attributeType)
        {
            if (attributeType == EntityAttributes.Health || attributeType == EntityAttributes.MaxHealth)
                health.Ratio = unit.HealthRatio;
            else if (attributeType == EntityAttributes.Power || attributeType == EntityAttributes.MaxPower)
                mainResource.Ratio = Mathf.Clamp01((float)unit.Power / unit.MaxPower);
        }

        private void OnUnitTargetChanged()
        {
            targetUnitFrame?.UpdateUnit(unit.Target);
        }

        private void OnUnitDisplayPowerChanged()
        {
            mainResource.FillImage.color = rendering.SpellPowerColors.Value(unit.DisplayPowerType);
        }

        private void OnUnitClassChanged()
        {
            classIcon.sprite = rendering.ClassIconSprites.Value(unit.ClassType);
            if (comboFrame != null && balance.ClassesByType.TryGetValue(unit.ClassType, out ClassInfo classInfo))
                comboFrame.Canvas.enabled = classInfo.HasPower(SpellPowerType.ComboPoints);
        }
    }
}