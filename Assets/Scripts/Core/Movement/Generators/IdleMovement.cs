namespace Core
{
    internal sealed class IdleMovement : MovementGenerator
    {
        public override MovementType Type => MovementType.Idle;

        public override void Begin(Unit unit)
        {
            unit.StopMoving();
        }

        public override void Finish(Unit unit)
        {
            unit.StopMoving();
        }

        public override void Reset(Unit unit)
        {
            unit.StopMoving();
        }

        public override bool Update(Unit unit, int deltaTime)
        {
            return true;
        }
    }
}