using Common;
using UnityEngine;

namespace Core
{
    internal class EntityAttributeFloat : EntityAttribute<float>
    {
        public EntityAttributeFloat(Entity entity, EventBus eventBus, float baseValue, float maxValue, EntityAttributes attributeType) : base(entity, eventBus, baseValue, maxValue, attributeType)
        {
            CurrentValue = BaseValue;
        }

        internal override float Reset()
        {
            return CurrentValue = BaseValue;
        }

        internal override float Set(float value)
        {
            float oldValue = CurrentValue;
            float newValue = Mathf.Clamp(value, MinValue, MaxValue);

            if (!Mathf.Approximately(oldValue, newValue))
            {
                EventBus.ExecuteEvent(Entity, GameEvents.UnitAttributeChanged, AttributeType);
                return 0.0f;
            }

            CurrentValue = newValue;
            return newValue - oldValue;
        }

        internal float Modify(float delta)
        {
            return Set(CurrentValue + delta);
        }

        internal void ModifyPercentage(float value, bool apply)
        {
            CurrentValue = StatUtils.ModifyMultiplierPercent(CurrentValue, value, apply);
        }
    }
}
