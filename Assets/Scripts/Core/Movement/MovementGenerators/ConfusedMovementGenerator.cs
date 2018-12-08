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
            unit.AddUnitState(UnitState.Confused);
            unit.SetFlag(EntityFields.UnitFlags, (uint)UnitFlags.Confused);

            if (!unit.IsAlive || unit.IsStopped())
                return;

            unit.StopMoving();
            unit.AddUnitState(UnitState.Move);
        }

        public override void DoReset(TUnit unit)
        {
            NextMoveTime.Reset(0);

            if (!unit.IsAlive || unit.IsStopped())
                return;

            unit.StopMoving();
            unit.AddUnitState(UnitState.Confused | UnitState.ConfusedMove);
        }

        public override bool DoUpdate(TUnit unit, uint timeDiff)
        {
            if (unit.HasUnitState(UnitState.Root | UnitState.Stunned | UnitState.Distracted))
                return true;

            if (NextMoveTime.Passed)
            {
                // currently moving, update location
                unit.AddUnitState(UnitState.ConfusedMove);

                if (unit.MoveSpline.Finalized())
                    NextMoveTime.Reset(RandomHelper.Next(800, 1500));
            }
            else
            {
                NextMoveTime.Update(timeDiff);
                if (NextMoveTime.Passed)
                {
                    unit.AddUnitState(UnitState.ConfusedMove);
                }
            }

            return true;
        }
    }

    public class ConfusedPlayerMovementGenerator : ConfusedMovementGenerator<Player>
    {
        public override void DoDeinitialize(Player player)
        {
            player.RemoveFlag(EntityFields.UnitFlags, (uint)UnitFlags.Confused);
            player.ClearUnitState(UnitState.Confused | UnitState.ConfusedMove);
            player.StopMoving();
        }
    }

    public class ConfusedCreatureMovementGenerator : ConfusedMovementGenerator<Creature>
    {
        public override void DoDeinitialize(Creature creature)
        {
            creature.RemoveFlag(EntityFields.UnitFlags, (uint)UnitFlags.Confused);
            creature.ClearUnitState(UnitState.Confused | UnitState.ConfusedMove);
        }
    }
}