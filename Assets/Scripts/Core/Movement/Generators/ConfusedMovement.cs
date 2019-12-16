using Common;

namespace Core
{
    internal sealed class ConfusedMovement : MovementGenerator
    {
        private TimeTracker nextMoveTime = new TimeTracker();

        public override MovementType Type => MovementType.Confused;

        public override void Begin(Unit unit)
        {
            unit.AddState(UnitControlState.Confused);
            unit.SetFlag(UnitFlags.Confused);

            if (!unit.IsAlive || unit.IsStopped)
                return;

            unit.AddState(UnitControlState.Move);
        }

        public override void Finish(Unit unit)
        {
            nextMoveTime.Reset(0);

            if (!unit.IsAlive || unit.IsStopped)
                return;

            unit.AddState(UnitControlState.Confused | UnitControlState.ConfusedMove);
        }

        public override void Reset(Unit unit)
        {
            nextMoveTime.Reset(0);

            unit.StopMoving();
            unit.AddState(UnitControlState.Confused | UnitControlState.ConfusedMove);
        }

        public override bool Update(Unit unit, int deltaTime)
        {
            if (unit.HasState(UnitControlState.Root | UnitControlState.Stunned | UnitControlState.Distracted))
                return true;

            if (nextMoveTime.Passed)
            {
                // currently moving, update location
                unit.AddState(UnitControlState.ConfusedMove);
                nextMoveTime.Reset(RandomUtils.Next(800, 1500));
            }
            else
            {
                nextMoveTime.Update(deltaTime);
                if (nextMoveTime.Passed)
                    unit.AddState(UnitControlState.ConfusedMove);
            }

            return true;
        }
    }
}