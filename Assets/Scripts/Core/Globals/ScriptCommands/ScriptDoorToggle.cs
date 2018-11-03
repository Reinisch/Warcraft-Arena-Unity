namespace Core
{
    public abstract class ScriptDoorToggle : ScriptInfo
    {
        public uint GOGuid { get; set; }
        public uint ResetDelay { get; set; }
    }

    public class ScriptDoorOpen : ScriptDoorToggle
    {
        public override ScriptCommands Command => ScriptCommands.OpenDoor;
    }

    public class ScriptDoorClose : ScriptDoorToggle
    {
        public override ScriptCommands Command => ScriptCommands.CloseDoor;
    }
}