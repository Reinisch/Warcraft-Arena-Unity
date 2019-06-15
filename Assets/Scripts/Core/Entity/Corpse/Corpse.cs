namespace Core
{
    public class Corpse : WorldEntity
    {
        internal override bool AutoScoped => true;

        public override void Accept(IVisitor visitor) => visitor.Visit(this);
    }
}