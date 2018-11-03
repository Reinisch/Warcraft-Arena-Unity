namespace Core
{
    public class ScriptQuestExplored : ScriptInfo
    {
        public override ScriptCommands Command => ScriptCommands.QuestExplored;

        public uint QuestID { get; set; }
        public uint Distance { get; set; }
    }
}