namespace Core
{
    public class ScriptCreateItem : ScriptInfo
    {
        public override ScriptCommands Command => ScriptCommands.CreateItem;

        public uint ItemEntry { get; set; }
        public uint Amount { get; set; }
    }
}