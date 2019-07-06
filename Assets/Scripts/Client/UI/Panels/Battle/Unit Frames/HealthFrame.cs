using System;
using Common;
using Core;
using JetBrains.Annotations;
using UnityEngine;

using EventHandler = Common.EventHandler;

namespace Client
{
    public class HealthFrame : MonoBehaviour
    {
        [SerializeField, UsedImplicitly] private CanvasGroup frameCanvasGroup;
        [SerializeField, UsedImplicitly] private AttributeBar healthBar;

        private Unit unit;

        public Unit Unit
        {
            set
            {
                if (unit != null)
                    DeinitializeUnit();

                if (value != null)
                    InitializeUnit(value);
            }
        }

        public float TargetFrameAlpha { get; set; }
        public float CurrentFrameAlpha { private get => frameCanvasGroup.alpha; set => frameCanvasGroup.alpha = value; }
        public float AlphaTransitionSpeed { private get; set; } = 2.0f;

        public AttributeBar HealthBar => healthBar;

        private readonly Action<EntityAttributes> onAttributeChangedAction;

        private HealthFrame()
        {
            onAttributeChangedAction = OnAttributeChanged;
        }

        public void DoUpdate(float deltaTime)
        {
            if (!Mathf.Approximately(CurrentFrameAlpha, TargetFrameAlpha))
                CurrentFrameAlpha = Mathf.MoveTowards(CurrentFrameAlpha, TargetFrameAlpha, deltaTime * AlphaTransitionSpeed);
        }

        private void InitializeUnit(Unit unit)
        {
            this.unit = unit;

            TargetFrameAlpha = CurrentFrameAlpha;

            OnAttributeChanged(EntityAttributes.Health);

            EventHandler.RegisterEvent(unit, GameEvents.UnitAttributeChanged, onAttributeChangedAction);
        }

        private void DeinitializeUnit()
        {
            EventHandler.UnregisterEvent(unit, GameEvents.UnitAttributeChanged, onAttributeChangedAction);

            unit = null;
        }

        private void OnAttributeChanged(EntityAttributes attributeType)
        {
            if (attributeType == EntityAttributes.Health || attributeType == EntityAttributes.MaxHealth)
                healthBar.Ratio = unit.HealthRatio;
        }
    }
}