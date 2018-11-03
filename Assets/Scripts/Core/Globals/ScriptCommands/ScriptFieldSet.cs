namespace Core
{
    public class ScriptFieldSet : ScriptInfo
    {
        public override ScriptCommands Command => ScriptCommands.FieldSet;

        public uint FieldID { get; set; }
        public uint FieldValue { get; set; }
    }
}