namespace Core
{
    public class Creature : Unit, IGridEntity<Creature>
    {
        public override EntityType EntityType => EntityType.Creature;
        public override bool AutoScoped => true;

        public GridReference<Creature> GridRef { get; private set; }
        public CreatureAI AI => ai as CreatureAI;

        public bool IsInGrid()
        {
            return false;
        }

        public void AddToGrid(GridReferenceManager<Creature> refManager)
        {

        }

        public void RemoveFromGrid()
        {

        }

        public override void DoUpdate(int timeDelta)
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