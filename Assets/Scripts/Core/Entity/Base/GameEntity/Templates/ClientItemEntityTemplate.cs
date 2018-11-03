namespace Core
{
    public class ClientItemEntityTemplate : GameEntityTemplate
    {
        public override GameEntityTypes Type => GameEntityTypes.ClientItem;

        public uint Item { get; set; }                      // 0 Item, References: Item, NoValue = 0
    }
}