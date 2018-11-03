namespace Core
{
    public class ScriptEquip : ScriptInfo
    {
        public override ScriptCommands Command => ScriptCommands.Equip;

        public uint EquipmentID { get; set; }
    }
}