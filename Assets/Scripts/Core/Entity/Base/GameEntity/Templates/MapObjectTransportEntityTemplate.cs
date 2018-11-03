namespace Core
{
    public class MapObjectTransportEntityTemplate : GameEntityTemplate
    {
        public override GameEntityTypes Type => GameEntityTypes.MapObjTransport;

        public uint TaxiPathID { get; set; }                            // 0 taxiPathID, References: TaxiPath, NoValue = 0
        public uint MoveSpeed { get; set; }                             // 1 moveSpeed, int, Min value: 1, Max value: 60, Default value: 1
        public uint AccelRate { get; set; }                             // 2 accelRate, int, Min value: 1, Max value: 20, Default value: 1
        public uint StartEventID { get; set; }                          // 3 startEventID, References: GameEvents, NoValue = 0
        public uint StopEventID { get; set; }                           // 4 stopEventID, References: GameEvents, NoValue = 0
        public uint TransportPhysics { get; set; }                      // 5 transportPhysics, References: TransportPhysics, NoValue = 0
        public int SpawnMap { get; set; }                               // 6 Spawn Map, References: Map, NoValue = -1
        public uint WorldState1 { get; set; }                           // 7 worldState1, References: WorldState, NoValue = 0
        public uint Allowstopping { get; set; }                         // 8 allow stopping, enum { false, true, }; Default: false
        public uint InitStopped { get; set; }                           // 9 Init Stopped, enum { false, true, }; Default: false
        public uint TrueInfiniteAoi { get; set; }                       // 10 True Infinite AOI (programmer only!), enum { false, true, }; Default: false
    }
}