namespace Core
{
    public class MailBoxEntityTemplate : GameEntityTemplate
    {
        public override GameEntityTypes Type => GameEntityTypes.Mailbox;

        public uint ConditionID1 { get; set; }              // 0 conditionID1, References: PlayerCondition, NoValue = 0
    }
}