namespace Core
{
    public class AreaDamageEntityTemplate : GameEntityTemplate
    {
        public override GameEntityTypes Type => GameEntityTypes.AreaDamage;

        public uint Open { get; set; }                          // 0 open, References: Lock_, NoValue = 0
        public uint Radius { get; set; }                        // 1 radius, int, Min value: 0, Max value: 50, Default value: 3
        public uint DamageMin { get; set; }                     // 2 damageMin, int, Min value: 0, Max value: 65535, Default value: 0
        public uint DamageMax { get; set; }                     // 3 damageMax, int, Min value: 0, Max value: 65535, Default value: 0
        public uint DamageSchool { get; set; }                  // 4 damageSchool, int, Min value: 0, Max value: 65535, Default value: 0
        public uint AutoClose { get; set; }                     // 5 autoClose (ms), int, Min value: 0, Max value: 2147483647, Default value: 0
        public uint OpenTextID { get; set; }                    // 6 openTextID, References: BroadcastText, NoValue = 0
        public uint CloseTextID { get; set; }                   // 7 closeTextID, References: BroadcastText, NoValue = 0

        public override uint LockId => Open;
        public override uint AutoCloseTime => AutoClose / TimeHelper.InMilliseconds;
    }
}