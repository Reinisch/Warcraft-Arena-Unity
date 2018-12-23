namespace Core
{
    public class MovementInfo
    {
        public MovementFlags Flags { get; private set; }
        public bool Jumping { get; set; }

        public void AddMovementFlag(MovementFlags flag)
        {
            Flags |= flag;
        }

        public void RemoveMovementFlag(MovementFlags flag)
        {
            Flags &= ~flag;
        }

        public bool HasMovementFlag(MovementFlags flag)
        {
            return (Flags & flag) != 0;
        }
    }
}