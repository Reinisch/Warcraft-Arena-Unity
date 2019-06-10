namespace Core
{
    public class DynamicEntity : WorldEntity
    {
        public override EntityType EntityType => EntityType.DynamicEntity;
        internal override bool AutoScoped => true;
    }
}