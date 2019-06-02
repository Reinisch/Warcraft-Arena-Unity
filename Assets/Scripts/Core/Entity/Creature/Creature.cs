namespace Core
{
    public class Creature : Unit
    {
        public override EntityType EntityType => EntityType.Creature;
        public override bool AutoScoped => true;

        public new CreatureAI AI => base.AI as CreatureAI;
        public GridReference<Creature> GridRef { get; } = new GridReference<Creature>();

        internal override void DoUpdate(int timeDelta)
        {
        }

        public override void Accept(IUnitVisitor unitVisitor)
        {
            unitVisitor.Visit(this);
        }

        public override void Accept(IVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}