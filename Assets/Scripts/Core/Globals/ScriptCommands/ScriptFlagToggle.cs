namespace Core
{
    public abstract class ScriptFlagToggle : ScriptInfo
    {
        public uint FieldID { get; set; }
        public uint FieldValue { get; set; }
    }

    public class ScriptFlagSet : ScriptFlagToggle
    {
        public override ScriptCommands Command => ScriptCommands.FlagSet;
    }

    public class ScriptFlagRemove : ScriptFlagToggle
    {
        public override ScriptCommands Command => ScriptCommands.FlagRemove;
    }
}