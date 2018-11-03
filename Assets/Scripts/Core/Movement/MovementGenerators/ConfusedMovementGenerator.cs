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

            float x, y, z;
            unit.GetPosition(out x, out y, out z);
            X = x;
            Y = y;
            Z = z;

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
                // waiting for next move
                NextMoveTime.Update(timeDiff);
                if (NextMoveTime.Passed)
                {
                    // start moving
                    unit.AddUnitState(UnitState.ConfusedMove);

                    //float dest = 4.0f * (float)RandomSolver.NextDouble() - 2.0f;
                    //
                    //Position pos;
                    //pos.Relocate(X, Y, Z);
                    //unit.MovePositionToFirstCollision(pos, dest, 0.0f);

                    //PathGenerator path(unit);
                    //path.SetPathLengthLimit(30.0f);
                    //bool result = path.CalculatePath(pos.m_positionX, pos.m_positionY, pos.m_positionZ);
                    //if (!result || (path.GetPathType() & PATHFIND_NOPATH))
                    //{
                    //    NextMoveTime.Reset(100);
                    //    return true;
                    //}

                    //Movement::MoveSplineInit init(unit);
                    //init.MovebyPath(path.GetPath());
                    //init.SetWalk(true);
                    //init.Launch();
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
            if (creature.GetVictim() != null)
                creature.SetTarget(creature.EnsureVictim().Guid);
        }
    }
}