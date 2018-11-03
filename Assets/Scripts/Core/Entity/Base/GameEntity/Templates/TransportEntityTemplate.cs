namespace Core
{
    public class TransportEntityTemplate : GameEntityTemplate
    {
        public override GameEntityTypes Type => GameEntityTypes.Transport;

        public uint Timeto2NdFloor { get; set; }                        // 0 Time to 2nd floor (ms), int, Min value: 0, Max value: 2147483647, Default value: 0
        public uint StartOpen { get; set; }                             // 1 startOpen, enum { false, true, }; Default: false
        public uint AutoClose { get; set; }                             // 2 autoClose (ms), int, Min value: 0, Max value: 2147483647, Default value: 0
        public uint Reached1StFloor { get; set; }                       // 3 Reached 1st floor, References: GameEvents, NoValue = 0
        public uint Reached2NdFloor { get; set; }                       // 4 Reached 2nd floor, References: GameEvents, NoValue = 0
        public int SpawnMap { get; set; }                               // 5 Spawn Map, References: Map, NoValue = -1
        public uint Timeto3RdFloor { get; set; }                        // 6 Time to 3rd floor (ms), int, Min value: 0, Max value: 2147483647, Default value: 0
        public uint Reached3RdFloor { get; set; }                       // 7 Reached 3rd floor, References: GameEvents, NoValue = 0
        public uint Timeto4ThFloor { get; set; }                        // 8 Time to 4th floor (ms), int, Min value: 0, Max value: 2147483647, Default value: 0
        public uint Reached4ThFloor { get; set; }                       // 9 Reached 4th floor, References: GameEvents, NoValue = 0
        public uint Timeto5ThFloor { get; set; }                        // 10 Time to 5th floor (ms), int, Min value: 0, Max value: 2147483647, Default value: 0
        public uint Reached5ThFloor { get; set; }                       // 11 Reached 5th floor, References: GameEvents, NoValue = 0
        public uint Timeto6ThFloor { get; set; }                        // 12 Time to 6th floor (ms), int, Min value: 0, Max value: 2147483647, Default value: 0
        public uint Reached6ThFloor { get; set; }                       // 13 Reached 6th floor, References: GameEvents, NoValue = 0
        public uint Timeto7ThFloor { get; set; }                        // 14 Time to 7th floor (ms), int, Min value: 0, Max value: 2147483647, Default value: 0
        public uint Reached7ThFloor { get; set; }                       // 15 Reached 7th floor, References: GameEvents, NoValue = 0
        public uint Timeto8TthFloor { get; set; }                       // 16 Time to 8th floor (ms), int, Min value: 0, Max value: 2147483647, Default value: 0
        public uint Reached8ThFloor { get; set; }                       // 17 Reached 8th floor, References: GameEvents, NoValue = 0
        public uint Timeto9TthFloor { get; set; }                       // 18 Time to 9th floor (ms), int, Min value: 0, Max value: 2147483647, Default value: 0
        public uint Reached9TthFloor { get; set; }                      // 19 Reached 9th floor, References: GameEvents, NoValue = 0
        public uint Timeto10ThFloor { get; set; }                       // 20 Time to 10th floor (ms), int, Min value: 0, Max value: 2147483647, Default value: 0
        public uint Reached10TthFloor { get; set; }                     // 21 Reached 10th floor, References: GameEvents, NoValue = 0
        public uint OnlychargeheightCheck { get; set; }                 // 22 only charge height check. (yards), int, Min value: 0, Max value: 65535, Default value: 0
        public uint OnlychargetimeCheck { get; set; }                   // 23 only charge time check, int, Min value: 0, Max value: 65535, Default value: 0

        public override uint AutoCloseTime => AutoClose / TimeHelper.InMilliseconds;
    }
}