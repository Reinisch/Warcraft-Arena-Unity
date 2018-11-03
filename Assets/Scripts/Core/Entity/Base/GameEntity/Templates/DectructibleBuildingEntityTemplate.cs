namespace Core
{
    public class DestructibleBuildingEntityTemplate : GameEntityTemplate
    {
        public override GameEntityTypes Type => GameEntityTypes.DestructibleBuilding;

        public uint CreditProxyCreature { get; set; }                     // 1 Credit Proxy Creature, References: Creature, NoValue = 0
        public uint HealthRec { get; set; }                               // 2 Health Rec, References: Depublic class GameEntityTemplateibleHitpoint, NoValue = 0
        public uint IntactEvent { get; set; }                             // 3 Intact Event, References: GameEvents, NoValue = 0
        public uint PvpEnabling { get; set; }                             // 4 PVP Enabling, enum { false, true, }; Default: false
        public uint InteriorVisible { get; set; }                         // 5 Interior Visible, enum { false, true, }; Default: false
        public uint InteriorLight { get; set; }                           // 6 Interior Light, enum { false, true, }; Default: false
        public uint DamagedEvent { get; set; }                            // 9 Damaged Event, References: GameEvents, NoValue = 0
        public uint DestroyedEvent { get; set; }                          // 14 Destroyed Event, References: GameEvents, NoValue = 0
        public uint RebuildingTime { get; set; }                          // 16 Rebuilding: Time (secs), int, Min value: 0, Max value: 65535, Default value: 0
        public uint RebuildingEvent { get; set; }                         // 19 Rebuilding: Event, References: GameEvents, NoValue = 0
        public uint DamageEvent { get; set; }                             // 22 Damage Event, References: GameEvents, NoValue = 0
    }
}