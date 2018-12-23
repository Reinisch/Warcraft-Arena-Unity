using UnityEngine.Assertions;

namespace Core
{
    public abstract class MovementGenerator
    {
        public abstract MovementGeneratorType GeneratorType { get; }

        public abstract void Initialize(Unit unit);
        public abstract void Deinitialize(Unit unit);
        public abstract void Reset(Unit unit);
        public abstract bool Update(Unit unit, uint timeDiff);
    }

    public abstract class MovementGeneratorMedium<TUnit,TMove> : MovementGenerator where TMove : MovementGeneratorMedium<TUnit, TMove> where TUnit : Unit
    {
        public override void Initialize(Unit u) 
        {
            Assert.IsNotNull(u as TUnit);

            DoInitialize(u as TUnit);
        }
        public abstract void DoInitialize(TUnit u);

        public override void Deinitialize(Unit u)
        {
            Assert.IsNotNull(u as TUnit);

            DoDeinitialize(u as TUnit);
        }
        public abstract void DoDeinitialize(TUnit u);

        public override void Reset(Unit u)
        {
            Assert.IsNotNull(u as TUnit);

            DoReset(u as TUnit);
        }
        public abstract void DoReset(TUnit u);

        public override bool Update(Unit u, uint timeDiff)
        {
            Assert.IsNotNull(u as TUnit);

            return DoUpdate(u as TUnit, timeDiff);
        }
        public abstract bool DoUpdate(TUnit u, uint timeDiff);
    }
}