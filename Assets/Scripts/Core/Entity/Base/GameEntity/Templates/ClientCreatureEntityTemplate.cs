namespace Core
{
    public class ClientCreatureEntityTemplate : GameEntityTemplate
    {
        public override GameEntityTypes Type => GameEntityTypes.ClientCreature;

        public uint CreatureDisplayInfo { get; set; }                     // 0 Creature Display Info, References: CreatureDisplayInfo, NoValue = 0
        public uint AnimKit { get; set; }                                 // 1 Anim Kit, References: AnimKit, NoValue = 0
        public uint CreatureID { get; set; }                              // 2 creatureID, References: Creature, NoValue = 0
    }
}