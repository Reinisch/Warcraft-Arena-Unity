using Common;

namespace Core
{
    internal abstract class EntityAttribute<TValueType>
    {
        protected readonly Entity Entity;
        protected readonly EventBus EventBus;

        protected EntityAttributes AttributeType;
        protected TValueType BaseValue;
        protected TValueType CurrentValue;
        protected TValueType MaxValue;
        protected TValueType MinValue;

        public TValueType Base => BaseValue;
        public TValueType Value => CurrentValue;
        public TValueType Max => MaxValue;
        public TValueType Min => MinValue;

        protected EntityAttribute(Entity entity, EventBus eventBus, TValueType baseValue, TValueType maxValue, EntityAttributes attributeType)
        {
            Entity = entity;
            EventBus = eventBus;
            BaseValue = baseValue;
            MaxValue = maxValue;
            AttributeType = attributeType;
        }

        internal void ModifyAttribute(TValueType baseValue, TValueType currentValue, TValueType minValue, TValueType maxValue, EntityAttributes attributeType)
        {
            AttributeType = attributeType;
            BaseValue = baseValue;
            MaxValue = maxValue;
            MinValue = minValue;
            CurrentValue = currentValue;
        }

        internal abstract TValueType Reset();

        internal abstract TValueType Set(TValueType value);
    }
}
