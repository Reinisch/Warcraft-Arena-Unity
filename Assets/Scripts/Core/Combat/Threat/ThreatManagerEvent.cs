namespace Core
{
    public class ThreatManagerEvent : ThreatRefStatusChangeEvent
    {
        public ThreatContainer ThreatContainer { get; set; }

        public ThreatManagerEvent(UnitEventType type) : base(type)
        {
            ThreatContainer = null;
        }

        public ThreatManagerEvent(UnitEventType type, HostileReference hostileReference) : base(type, hostileReference)
        {
            ThreatContainer = null;
        }
    }
}