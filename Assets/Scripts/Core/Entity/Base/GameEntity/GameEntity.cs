namespace Core
{
    public class GameEntity : WorldEntity
    {
        public override EntityType EntityType => EntityType.GameEntity;
        internal override bool AutoScoped => true;
    }
}