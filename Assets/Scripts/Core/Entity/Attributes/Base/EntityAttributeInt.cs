using Common;
using UnityEngine;

namespace Core
{
    internal class EntityAttributeInt : EntityAttribute<int>
    {
        public EntityAttributeInt(Entity entity, EventBus eventBus, int baseValue, int maxValue, EntityAttributes attributeType) : base(entity, eventBus, baseValue, maxValue, attributeType)
        {
            CurrentValue = BaseValue;
        }

        internal override int Reset()
        {
            return CurrentValue = BaseValue;
        }

        internal override int Set(int value)
        {
            int oldValue = CurrentValue;
            int newValue = Mathf.Clamp(value, MinValue, MaxValue);
            CurrentValue = newValue;

            if (oldValue != newValue)
                EventBus.ExecuteEvent(Entity, GameEvents.UnitAttributeChanged, AttributeType);

            return newValue - oldValue;
        }

        internal int Modify(int delta)
        {
            return Set(CurrentValue + delta);
        }
    }
}
