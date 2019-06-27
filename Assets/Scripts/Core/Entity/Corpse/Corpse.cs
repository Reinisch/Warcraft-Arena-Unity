namespace Core
{
    public class Corpse : WorldEntity
    {
        internal override bool AutoScoped => true;

        public override string Name { get; internal set; }
    }
}