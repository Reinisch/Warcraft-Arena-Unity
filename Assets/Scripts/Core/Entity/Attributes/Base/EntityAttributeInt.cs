using UnityEngine;
using Common;

using EventHandler = Common.EventHandler;

namespace Core
{
    internal class EntityAttributeInt : EntityAttribute<int>
    {
        public EntityAttributeInt(Entity entity, int baseValue, int maxValue, EntityAttributes attributeType) : base(entity, baseValue, maxValue, attributeType)
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
                EventHandler.ExecuteEvent(Entity, GameEvents.UnitAttributeChanged, AttributeType);

            return newValue - oldValue;
        }

        internal int Modify(int delta)
        {
            return Set(CurrentValue + delta);
        }
    }
}
