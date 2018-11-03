namespace Core
{
    public class Flag128
    {
        private readonly uint[] part = new uint[4];


        public Flag128(uint p1 = 0, uint p2 = 0, uint p3 = 0, uint p4 = 0)
        {
            part[0] = p1;
            part[1] = p2;
            part[2] = p3;
            part[3] = p4;
        }

        public bool IsEqual(uint p1 = 0, uint p2 = 0, uint p3 = 0, uint p4 = 0)
        {
            return (part[0] == p1 && part[1] == p2 && part[2] == p3 && part[3] == p4);
        }

        public bool HasAnyFlag(uint p1 = 0, uint p2 = 0, uint p3 = 0, uint p4 = 0)
        {
            return (part[0] & p1) != 0 || (part[1] & p2) != 0 || (part[2] & p3) != 0 || (part[3] & p4) != 0;
        }

        public bool HasFlagAtIndex(int index, uint flag)
        {
            return (part[index] & flag) == flag;
        }

        public bool HasAnyFlagAtIndex(int index, uint flag)
        {
            return (part[index] & flag) != 0;
        }

        public bool HasFlag(uint p1 = 0, uint p2 = 0, uint p3 = 0, uint p4 = 0)
        {
            return (part[0] & p1) == p1 || (part[1] & p2) == p2 || (part[2] & p3) == p3 || (part[3] & p4) == p4;
        }

        public bool HasAnyFlag(Flag128 flag)
        {
            return (part[0] & flag[0]) != 0 || (part[1] & flag[1]) != 0 || (part[2] & flag[2]) != 0 || (part[3] & flag[3]) != 0;
        }

        public bool HasFlag(Flag128 flag)
        {
            return (part[0] & flag[0]) == flag[0] && (part[1] & flag[1]) == flag[1] && (part[2] & flag[2]) == flag[2] && (part[3] & flag[3]) == flag[3];
        }


        public void Set(uint p1 = 0, uint p2 = 0, uint p3 = 0, uint p4 = 0)
        {
            part[0] = p1;
            part[1] = p2;
            part[2] = p3;
            part[3] = p4;
        }

        public Flag128 Copy(Flag128 from)
        {
            part[0] = from.part[0];
            part[1] = from.part[1];
            part[2] = from.part[2];
            part[3] = from.part[3];
            return this;
        }


        public static bool operator ==(Flag128 left, Flag128 right)
        {
            if (ReferenceEquals(left, null))
                return ReferenceEquals(right, null);
            if (ReferenceEquals(right, null))
                return false;

            return left.Equals(right);
        }

        public static bool operator !=(Flag128 left, Flag128 right)
        {
            return !(left == right);
        }

        protected bool Equals(Flag128 other)
        {
            return part[0] == other.part[0] && part[1] == other.part[1] && part[2] == other.part[2] && part[3] == other.part[3];
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
                return false;
            if (ReferenceEquals(this, obj))
                return true;
            if (obj.GetType() != GetType())
                return false;

            return Equals((Flag128)obj);
        }

        public override int GetHashCode()
        {
            return part.GetHashCode();
        }

        public uint this [int index] => part[index];
    }
}