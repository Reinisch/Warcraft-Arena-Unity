namespace Core
{
    public class GameEntity : WorldEntity
    {
        public override EntityType EntityType => EntityType.GameEntity;
        public override bool AutoScoped => true;
    }
}