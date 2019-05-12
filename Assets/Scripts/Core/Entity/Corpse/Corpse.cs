namespace Core
{
    public class Corpse : WorldEntity
    {
        public override EntityType EntityType => EntityType.Corpse;
        public override bool AutoScoped => true;
    }
}