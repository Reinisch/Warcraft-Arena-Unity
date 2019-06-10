namespace Core
{
    public class Corpse : WorldEntity
    {
        public override EntityType EntityType => EntityType.Corpse;
        internal override bool AutoScoped => true;
    }
}