using System;
using Client.UI;
using Common;
using Core;
using Zenject;

namespace Client
{
    public class ComboFramePresenter : Presenter<ComboFrame>
    {
        [Inject] private EventBus eventBus;

        private readonly Action<EntityAttributes> onAttributeChangedAction;

        private Unit unit;

        public ComboFramePresenter()
        {
            onAttributeChangedAction = OnAttributeChanged;
        }

        public void SetUnit(Unit newUnit)
        {
            if (unit != null)
                UnregisterUnit();

            if (newUnit != null)
                RegisterUnit(newUnit);

            View.SetVisible(unit != null);
        }

        private void RegisterUnit(Unit newUnit)
        {
            unit = newUnit;

            OnAttributeChanged(EntityAttributes.ComboPoints);

            eventBus.RegisterEvent(unit, GameEvents.UnitAttributeChanged, onAttributeChangedAction);
        }

        private void UnregisterUnit()
        {
            eventBus.UnregisterEvent(unit, GameEvents.UnitAttributeChanged, onAttributeChangedAction);

            unit = null;
        }

        private void OnAttributeChanged(EntityAttributes attributeType)
        {
            if (attributeType != EntityAttributes.ComboPoints)
                return;

            for (int i = 0; i < View.ComboPointSlotCount; i++)
                View.SetComboPointActive(i, i < unit.ComboPoints);
        }
    }
}
