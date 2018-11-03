namespace Core
{
    public class CapturePointEntityTemplate : GameEntityTemplate
    {
        public override GameEntityTypes Type => GameEntityTypes.CapturePoint;

        public uint CaptureTime { get; set; }                           // 0 Capture Time (ms), int, Min value: 0, Max value: 2147483647, Default value: 60000
        public uint GiganticAoi { get; set; }                           // 1 Gigantic AOI, enum { false, true, }; Default: false
        public uint Highlight { get; set; }                             // 2 highlight, enum { false, true, }; Default: true
        public uint Open { get; set; }                                  // 3 open, References: Lock_, NoValue = 0
        public uint AssaultBroadcastHorde { get; set; }                 // 4 Assault Broadcast (Horde), References: BroadcastText, NoValue = 0
        public uint CaptureBroadcastHorde { get; set; }                 // 5 Capture Broadcast (Horde), References: BroadcastText, NoValue = 0
        public uint DefendedBroadcastHorde { get; set; }                // 6 Defended Broadcast (Horde), References: BroadcastText, NoValue = 0
        public uint AssaultBroadcastAlliance { get; set; }              // 7 Assault Broadcast (Alliance), References: BroadcastText, NoValue = 0
        public uint CaptureBroadcastAlliance { get; set; }              // 8 Capture Broadcast (Alliance), References: BroadcastText, NoValue = 0
        public uint DefendedBroadcastAlliance { get; set; }             // 9 Defended Broadcast (Alliance), References: BroadcastText, NoValue = 0
        public uint WorldState1 { get; set; }                           // 10 worldState1, References: WorldState, NoValue = 0
        public uint ContestedEventHorde { get; set; }                   // 11 Contested Event (Horde), References: GameEvents, NoValue = 0
        public uint CaptureEventHorde { get; set; }                     // 12 Capture Event (Horde), References: GameEvents, NoValue = 0
        public uint DefendedEventHorde { get; set; }                    // 13 Defended Event (Horde), References: GameEvents, NoValue = 0
        public uint ContestedEventAlliance { get; set; }                // 14 Contested Event (Alliance), References: GameEvents, NoValue = 0
        public uint CaptureEventAlliance { get; set; }                  // 15 Capture Event (Alliance), References: GameEvents, NoValue = 0
        public uint DefendedEventAlliance { get; set; }                 // 16 Defended Event (Alliance), References: GameEvents, NoValue = 0
        public uint SpellVisual1 { get; set; }                          // 17 Spell Visual 1, References: SpellVisual, NoValue = 0
        public uint SpellVisual2 { get; set; }                          // 18 Spell Visual 2, References: SpellVisual, NoValue = 0
        public uint SpellVisual3 { get; set; }                          // 19 Spell Visual 3, References: SpellVisual, NoValue = 0
        public uint SpellVisual4 { get; set; }                          // 20 Spell Visual 4, References: SpellVisual, NoValue = 0
        public uint SpellVisual5 { get; set; }                          // 21 Spell Visual 5, References: SpellVisual, NoValue = 0

        public override uint LockId => Open;
    }
}