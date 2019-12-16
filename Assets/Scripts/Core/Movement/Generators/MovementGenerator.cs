namespace Core
{
    internal abstract class MovementGenerator
    {
        public abstract MovementType Type { get; }

        public abstract void Begin(Unit unit);
        public abstract void Finish(Unit unit);
        public abstract void Reset(Unit unit);
        public abstract bool Update(Unit unit, int deltaTime);
    }
}