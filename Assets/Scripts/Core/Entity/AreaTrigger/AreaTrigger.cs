namespace Core
{
    public class AreaTrigger : WorldEntity
    {
        internal override bool AutoScoped => true;

        public override void Accept(IVisitor visitor) => visitor.Visit(this);
    }
}