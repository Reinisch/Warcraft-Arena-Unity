namespace Core
{
    public class DynamicEntity : WorldEntity
    {
        public override EntityType EntityType => EntityType.DynamicEntity;
        public override bool AutoScoped => true;
    }
}