namespace Core
{
    public class ControlZoneEntityTemplate : GameEntityTemplate
    {
        public override GameEntityTypes Type => GameEntityTypes.ControlZone;

        public uint Radius { get; set; }                                  // 0 radius, int, Min value: 0, Max value: 100, Default value: 10
        public uint Spell { get; set; }                                   // 1 spell, References: Spell, NoValue = 0
        public uint WorldState1 { get; set; }                             // 2 worldState1, References: WorldState, NoValue = 0
        public uint Worldstate2 { get; set; }                             // 3 worldstate2, References: WorldState, NoValue = 0
        public uint CaptureEventHorde { get; set; }                       // 4 Capture Event (Horde), References: GameEvents, NoValue = 0
        public uint CaptureEventAlliance { get; set; }                    // 5 Capture Event (Alliance), References: GameEvents, NoValue = 0
        public uint ContestedEventHorde { get; set; }                    // 6 Contested Event (Horde), References: GameEvents, NoValue = 0
        public uint ContestedEventAlliance { get; set; }                  // 7 Contested Event (Alliance), References: GameEvents, NoValue = 0
        public uint ProgressEventHorde { get; set; }                      // 8 Progress Event (Horde), References: GameEvents, NoValue = 0
        public uint ProgressEventAlliance { get; set; }                   // 9 Progress Event (Alliance), References: GameEvents, NoValue = 0
        public uint NeutralEventHorde { get; set; }                       // 10 Neutral Event (Horde), References: GameEvents, NoValue = 0
        public uint NeutralEventAlliance { get; set; }                    // 11 Neutral Event (Alliance), References: GameEvents, NoValue = 0
        public uint NeutralPercent { get; set; }                          // 12 neutralPercent, int, Min value: 0, Max value: 100, Default value: 0
        public uint Worldstate3 { get; set; }                             // 13 worldstate3, References: WorldState, NoValue = 0
        public uint MinSuperiority { get; set; }                          // 14 minSuperiority, int, Min value: 1, Max value: 65535, Default value: 1
        public uint MaxSuperiority { get; set; }                          // 15 maxSuperiority, int, Min value: 1, Max value: 65535, Default value: 1
        public uint MinTime { get; set; }                                 // 16 minTime, int, Min value: 1, Max value: 65535, Default value: 1
        public uint MaxTime { get; set; }                                 // 17 maxTime, int, Min value: 1, Max value: 65535, Default value: 1
        public uint GiganticAoi { get; set; }                            // 18 Gigantic AOI, enum { false, true, }; Default: false
        public uint Highlight { get; set; }                               // 19 highlight, enum { false, true, }; Default: true
        public uint StartingValue { get; set; }                           // 20 startingValue, int, Min value: 0, Max value: 100, Default value: 50
        public uint Unidirectional { get; set; }                          // 21 unidirectional, enum { false, true, }; Default: false
        public uint Killbonustime { get; set; }                           // 22 kill bonus time %, int, Min value: 0, Max value: 100, Default value: 0
        public uint SpeedWorldState1 { get; set; }                        // 23 speedWorldState1, References: WorldState, NoValue = 0
        public uint SpeedWorldState2 { get; set; }                        // 24 speedWorldState2, References: WorldState, NoValue = 0
        public uint UncontestedTime { get; set; }                         // 25 Uncontested Time, int, Min value: 0, Max value: 65535, Default value: 0
        public uint FrequentHeartbeat { get; set; }                       // 26 Frequent Heartbeat, enum { false, true, }; Default: false
    }
}