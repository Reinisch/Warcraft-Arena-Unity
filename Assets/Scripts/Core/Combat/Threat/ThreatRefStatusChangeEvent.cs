namespace Core
{
    public class ThreatRefStatusChangeEvent : UnitBaseEvent
    {
        public int IntValue { get; set; }
        public bool BoolValue { get; set; }
        public float FloatValue { get; set; }

        public ThreatManager ThreatManager { get; set; }
        public HostileReference HostileReference { get; private set; }


        public ThreatRefStatusChangeEvent(UnitEventType type) : base(type)
        {
            ThreatManager = null;
            HostileReference = null;
        }

        public ThreatRefStatusChangeEvent(UnitEventType type, HostileReference hostileReference) : base(type)
        {
            ThreatManager = null;
            HostileReference = hostileReference;
        }

        public ThreatRefStatusChangeEvent(UnitEventType type, HostileReference hostileReference, float value) : base(type)
        {
            ThreatManager = null;
            HostileReference = hostileReference;
            FloatValue = value;
        }

        public ThreatRefStatusChangeEvent(UnitEventType type, HostileReference hostileReference, bool value) : base(type)
        {
            ThreatManager = null;
            HostileReference = hostileReference;
            BoolValue = value;
        }

        public ThreatRefStatusChangeEvent(UnitEventType type, HostileReference hostileReference, int value) : base(type)
        {
            ThreatManager = null;
            HostileReference = hostileReference;
            IntValue = value;
        }
    }
}