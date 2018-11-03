namespace Core
{
    public class GuildBankEntityTemplate : GameEntityTemplate
    {
        public override GameEntityTypes Type => GameEntityTypes.GuildBank;

        public uint ConditionID1 { get; set; }          // 0 conditionID1, References: PlayerCondition, NoValue = 0
    }
}