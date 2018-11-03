using System;

namespace Core
{
    public class Creature : Unit, IGridEntity<Creature>
    {
        public GridReference<Creature> GridRef { get; private set; }

        public new CreatureAI AI => base.AI as CreatureAI;

        protected ReactStates ReactState { get; set; }
        protected MovementGeneratorType DefaultMovementType { get; set; }

        protected CreatureData CreatureData { get; set; }
        protected CreatureTemplate CreatureInfo { get; set; }

        public bool IsInGrid() { throw new NotImplementedException(); }
        public void AddToGrid(GridReferenceManager<Creature> refManager) { throw new NotImplementedException(); }
        public void RemoveFromGrid() { throw new NotImplementedException(); }

        public override void DoUpdate(uint time) { throw new NotImplementedException(); }
        public override void AddToWorld() { throw new NotImplementedException(); }
        public override void RemoveFromWorld() { throw new NotImplementedException(); }
        public override void SetDisplayId(uint modelId) { throw new NotImplementedException(); }
    }
}