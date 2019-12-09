using UnityEngine;
using Common;

using EventHandler = Common.EventHandler;

namespace Core
{
    internal class EntityAttributeFloat : EntityAttribute<float>
    {
        public EntityAttributeFloat(Entity entity, float baseValue, float maxValue, EntityAttributes attributeType) : base(entity, baseValue, maxValue, attributeType)
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
                EventHandler.ExecuteEvent(Entity, GameEvents.UnitAttributeChanged, AttributeType);
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
