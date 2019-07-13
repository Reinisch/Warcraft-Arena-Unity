using Common;

namespace Core
{
    public abstract class ConfusedMovementGenerator<TUnit> : MovementGeneratorMedium<TUnit, ConfusedMovementGenerator<TUnit>> where TUnit : Unit
    {
        protected TimeTracker NextMoveTime { get; set; }
        protected float X { get; set; }
        protected float Y { get; set; }
        protected float Z { get; set; }

        public override MovementGeneratorType GeneratorType => MovementGeneratorType.ConfusedMotionType;

        protected ConfusedMovementGenerator()
        {
            NextMoveTime = new TimeTracker(0);
        }

        public override void DoInitialize(TUnit unit)
        {
            unit.AddState(UnitState.Confused);
            unit.SetFlag(UnitFlags.Confused);

            if (!unit.IsAlive || unit.IsStopped)
                return;

            unit.AddState(UnitState.Move);
        }

        public override void DoReset(TUnit unit)
        {
            NextMoveTime.Reset(0);

            if (!unit.IsAlive || unit.IsStopped)
                return;

            unit.AddState(UnitState.Confused | UnitState.ConfusedMove);
        }

        public override bool DoUpdate(TUnit unit, uint timeDiff)
        {
            if (unit.HasState(UnitState.Root | UnitState.Stunned | UnitState.Distracted))
                return true;

            if (NextMoveTime.Passed)
            {
                // currently moving, update location
                unit.AddState(UnitState.ConfusedMove);
                NextMoveTime.Reset(RandomUtils.Next(800, 1500));
            }
            else
            {
                NextMoveTime.Update(timeDiff);
                if (NextMoveTime.Passed)
                    unit.AddState(UnitState.ConfusedMove);
            }

            return true;
        }
    }

    public class ConfusedPlayerMovementGenerator : ConfusedMovementGenerator<Player>
    {
        public override void DoDeinitialize(Player player)
        {
            player.RemoveFlag(UnitFlags.Confused);
            player.RemoveState(UnitState.Confused | UnitState.ConfusedMove);
        }
    }

    public class ConfusedCreatureMovementGenerator : ConfusedMovementGenerator<Creature>
    {
        public override void DoDeinitialize(Creature creature)
        {
            creature.RemoveFlag(UnitFlags.Confused);
            creature.RemoveState(UnitState.Confused | UnitState.ConfusedMove);
        }
    }
}