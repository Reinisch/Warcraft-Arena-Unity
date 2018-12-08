namespace Core
{
    public class EntityGridStopper : IGridVisitor
    {
        public void Visit(GridReferenceManager<Creature> creatureContainer)
        {
            // stop any fights at grid de-activation and remove dynobjects created at cast by creatures
            var creatureRef = creatureContainer.First;
            while (creatureRef != null)
            {
                creatureRef.Value.Source.RemoveAllDynObjects();
                if (creatureRef.Value.Source.IsInCombat())
                {
                    creatureRef.Value.Source.AI.EnterEvadeMode();
                }
                creatureRef = creatureRef.Next;
            }
        }
        public void Visit<TEntity>(GridReferenceManager<TEntity> container) where TEntity : Entity { }
    }
}