namespace Core
{
    internal abstract class EntityAttribute<TValueType>
    {
        protected readonly EntityAttributes AttributeType;
        protected readonly Entity Entity;

        protected TValueType BaseValue;
        protected TValueType CurrentValue;
        protected TValueType MaxValue;

        public TValueType Base => BaseValue;
        public TValueType Value => CurrentValue;
        public TValueType Max => MaxValue;
        
        protected EntityAttribute(Entity entity, TValueType baseValue, TValueType maxValue, EntityAttributes attributeType)
        {
            Entity = entity;
            BaseValue = baseValue;
            MaxValue = maxValue;
            AttributeType = attributeType;
        }

        internal abstract TValueType Reset();

        internal abstract TValueType Set(TValueType value);
    }
}
