using Common;

namespace Core
{
    public abstract class ConfusedMovementGenerator<TUnit> : MovementGeneratorMedium<TUnit, ConfusedMovementGenerator<TUnit>> where TUnit : Unit
    {
        private TimeTracker nextMoveTime = new TimeTracker();

        protected float X { get; set; }
        protected float Y { get; set; }
        protected float Z { get; set; }

        public override MovementGeneratorType GeneratorType => MovementGeneratorType.ConfusedMotionType;

        public override void DoInitialize(TUnit unit)
        {
            unit.AddState(UnitControlState.Confused);
            unit.SetFlag(UnitFlags.Confused);

            if (!unit.IsAlive || unit.IsStopped)
                return;

            unit.AddState(UnitControlState.Move);
        }

        public override void DoReset(TUnit unit)
        {
            nextMoveTime.Reset(0);

            if (!unit.IsAlive || unit.IsStopped)
                return;

            unit.AddState(UnitControlState.Confused | UnitControlState.ConfusedMove);
        }

        public override bool DoUpdate(TUnit unit, uint timeDiff)
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
                nextMoveTime.Update(timeDiff);
                if (nextMoveTime.Passed)
                    unit.AddState(UnitControlState.ConfusedMove);
            }

            return true;
        }
    }

    public class ConfusedPlayerMovementGenerator : ConfusedMovementGenerator<Player>
    {
        public override void DoDeinitialize(Player player)
        {
            player.RemoveFlag(UnitFlags.Confused);
            player.RemoveState(UnitControlState.Confused | UnitControlState.ConfusedMove);
        }
    }

    public class ConfusedCreatureMovementGenerator : ConfusedMovementGenerator<Creature>
    {
        public override void DoDeinitialize(Creature creature)
        {
            creature.RemoveFlag(UnitFlags.Confused);
            creature.RemoveState(UnitControlState.Confused | UnitControlState.ConfusedMove);
        }
    }
}