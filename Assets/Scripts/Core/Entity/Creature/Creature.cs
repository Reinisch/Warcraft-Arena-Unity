namespace Core
{
    public class Creature : Unit, IGridEntity<Creature>
    {
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
    }
}