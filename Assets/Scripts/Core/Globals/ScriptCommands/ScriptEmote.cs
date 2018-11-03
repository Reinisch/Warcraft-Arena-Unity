namespace Core
{
    public class ScriptEmote : ScriptInfo
    {
        public override ScriptCommands Command => ScriptCommands.Emote;

        public uint EmoteID { get; set; }
        public uint Flags { get; set; }
    }
}