namespace Core
{
    public class UnitActionBarEntry
    {
        public uint PackedData { get; private set; }

        public uint Action => UnitHelper.UnitActionButtonAction(PackedData);
        public ActiveStates Type => (ActiveStates)UnitHelper.UnitActionButtonType(PackedData);
        public bool IsActionBarForSpell => Type == ActiveStates.Disabled || Type == ActiveStates.Enabled || Type == ActiveStates.Passive;


        public UnitActionBarEntry()
        {
            PackedData = (uint) ActiveStates.Disabled << 24;
        }

        public void SetActionAndType(uint action, ActiveStates type)
        {
            PackedData = UnitHelper.MakeUnitActionButton(action, (uint)type);
        }

        public void SetType(ActiveStates type)
        {
            PackedData = UnitHelper.MakeUnitActionButton(UnitHelper.UnitActionButtonAction(PackedData), (uint)type);
        }

        public void SetAction(uint action)
        {
            PackedData = (PackedData & 0xFF000000) | UnitHelper.UnitActionButtonAction(action);
        }
    }
}