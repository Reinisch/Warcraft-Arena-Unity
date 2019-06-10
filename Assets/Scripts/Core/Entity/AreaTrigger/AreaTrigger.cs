namespace Core
{
    public class AreaTrigger : WorldEntity
    {
        public override EntityType EntityType => EntityType.AreaTrigger;
        internal override bool AutoScoped => true;
    }
}