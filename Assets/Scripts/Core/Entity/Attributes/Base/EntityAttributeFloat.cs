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
            float newValue = Mathf.Clamp(value, 0.0f, MaxValue);

            if (!Mathf.Approximately(oldValue, newValue))
            {
                EventHandler.ExecuteEvent(Entity, GameEvents.EntityAttributeChanged, AttributeType);
                return 0.0f;
            }

            CurrentValue = newValue;
            return newValue - oldValue;
        }

        internal float Modify(float delta)
        {
            return Set(CurrentValue + delta);
        }
    }
}
