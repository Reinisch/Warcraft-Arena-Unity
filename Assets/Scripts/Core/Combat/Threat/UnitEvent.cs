namespace Core
{
    public class UnitBaseEvent
    {
        public UnitEventType Type { get; private set; }

        public UnitBaseEvent(UnitEventType type)
        {
            Type = type;
        }

        public bool MatchesTypeMask(UnitEventType mask)
        {
            return (Type & mask) != 0;
        }
    }
}