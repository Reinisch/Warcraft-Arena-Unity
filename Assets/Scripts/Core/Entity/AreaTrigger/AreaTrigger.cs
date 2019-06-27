namespace Core
{
    public class AreaTrigger : WorldEntity
    {
        internal override bool AutoScoped => true;

        public override string Name { get; internal set; }
    }
}