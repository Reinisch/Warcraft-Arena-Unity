namespace Core
{
    public class NewFlagDropEntityTemplate : GameEntityTemplate
    {
        public override GameEntityTypes Type => GameEntityTypes.NewFlagDrop;

        public uint Open { get; set; }                          // 0 open, References: Lock_, NoValue = 0

        public override uint LockId => Open;
    }
}