namespace Core
{
    public interface ITargetedMovementGenerator
    {
        Unit Target { get; }
        bool IsReachable { get; }

        void StopFollowing();
    }

    public abstract class TargetedMovementGeneratorMedium<TUnit, TMove> : MovementGeneratorMedium<TUnit, TMove>,
        ITargetedMovementGenerator where TMove : MovementGeneratorMedium<TUnit, TMove> where TUnit : Unit
    {
        public Unit Target => TargetReference.Target;
        public bool IsReachable => Path == null || (Path.Type & PathType.Normal) > 0;

        protected FollowerReference TargetReference { get; set; }
        protected PathGenerator Path { get; set; }
        protected TimeTracker RecheckDistance { get; set; }
        protected float Offset { get; set; }
        protected float Angle { get; set; }
        protected bool RecalculateTravel { get; set; }
        protected bool TargetReached { get; set; }


        protected TargetedMovementGeneratorMedium(Unit target, float offset, float angle)
        {
            TargetReference = new FollowerReference();
            TargetReference.Link(target, this);

            Path = null;
            RecheckDistance = new TimeTracker(0);
            Offset = offset;
            Angle = angle;
            RecalculateTravel = false;
            TargetReached = false;
        }

        public void StopFollowing() { }

        public override bool DoUpdate(TUnit unit, uint timeDiff) {  return false; }
        public override void UnitSpeedChanged() { RecalculateTravel = true; }

        protected void SetTargetLocation(TUnit owner, bool updateDestination) { }
    }

    public abstract class ChaseMovementGenerator<TUnit> : TargetedMovementGeneratorMedium<TUnit, ChaseMovementGenerator<TUnit>> where TUnit : Unit
    {
        public override MovementGeneratorType GeneratorType => MovementGeneratorType.ChaseMotionType;


        protected ChaseMovementGenerator(Unit target, float offset, float angle) : base(target, offset, angle)
        {
        
        }

        public override void DoInitialize(TUnit unit) { }
        public override void DoDeinitialize(TUnit unit) { }
        public override void DoReset(TUnit unit) { }
        public void MovementInform(TUnit unit) { }

        public bool EnableWalking() { return false;}
        public bool LostTarget(TUnit unit) { return unit.GetVictim() != Target; }
        public void ReachTarget(TUnit unit) { }

        protected static void ClearUnitStateMove(TUnit unit) { unit.ClearUnitState(UnitState.ChaseMove); }
        protected static void AddUnitStateMove(TUnit unit) { unit.AddUnitState(UnitState.ChaseMove); }
    }

    public abstract class FollowMovementGenerator<TUnit> : TargetedMovementGeneratorMedium<TUnit, FollowMovementGenerator<TUnit>> where TUnit : Unit
    {
        public override MovementGeneratorType GeneratorType => MovementGeneratorType.FollowMotionType;

        protected FollowMovementGenerator(Unit target, float offset, float angle) : base(target, offset, angle)
        {
        
        }

        public override void DoInitialize(TUnit unit) { }
        public override void DoDeinitialize(TUnit unit) { }
        public override void DoReset(TUnit unit) { }
        public void MovementInform(TUnit unit) { }

        public bool EnableWalking() { return false; }
        public bool LostTarget(TUnit unit) { return false; }
        public void ReachTarget(TUnit unit) { }

        protected void UpdateSpeed(TUnit owner) { }

        protected static void ClearUnitStateMove(TUnit unit) { unit.ClearUnitState(UnitState.FollowMove); }
        protected static void AddUnitStateMove(TUnit unit) { unit.AddUnitState(UnitState.FollowMove); }
    }
}