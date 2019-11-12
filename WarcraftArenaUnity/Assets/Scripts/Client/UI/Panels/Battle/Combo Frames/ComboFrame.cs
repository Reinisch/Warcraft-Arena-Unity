using System;
using System.Collections.Generic;
using Core;
using JetBrains.Annotations;
using UnityEngine;
using Common;

using EventHandler = Common.EventHandler;

namespace Client
{
    public class ComboFrame : MonoBehaviour
    {
        [SerializeField, UsedImplicitly] private Canvas canvas;
        [SerializeField, UsedImplicitly] private CanvasGroup canvasGroup;
        [SerializeField, UsedImplicitly] private List<ComboPointSlot> comboPointSlots;

        private readonly Action<EntityAttributes> onAttributeChangedAction;

        private Unit unit;

        private ComboFrame() => onAttributeChangedAction = OnAttributeChanged;

        public Canvas Canvas => canvas;

        public void UpdateUnit(Unit newUnit)
        {
            if (unit != null)
                UnregisterUnit();

            if (newUnit != null)
                RegisterUnit(newUnit);

            canvasGroup.blocksRaycasts = unit != null;
            canvasGroup.interactable = unit != null;
            canvasGroup.alpha = unit != null ? 1.0f : 0.0f;
        }

        private void RegisterUnit(Unit unit)
        {
            this.unit = unit;

            OnAttributeChanged(EntityAttributes.ComboPoints);

            EventHandler.RegisterEvent(unit, GameEvents.UnitAttributeChanged, onAttributeChangedAction);
        }

        private void UnregisterUnit()
        {
            EventHandler.UnregisterEvent(unit, GameEvents.UnitAttributeChanged, onAttributeChangedAction);

            unit = null;
        }

        private void OnAttributeChanged(EntityAttributes attributeType)
        {
            if (attributeType == EntityAttributes.ComboPoints)
                for (int i = 0; i < comboPointSlots.Count; i++)
                    comboPointSlots[i].ModifyState(i < unit.ComboPoints);
        }
    }
}