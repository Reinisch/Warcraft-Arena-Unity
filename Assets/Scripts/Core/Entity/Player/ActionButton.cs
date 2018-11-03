namespace Core
{
    public class ActionButton
    {
        private ulong packedData;
        private ActionButtonUpdateState uState;

        public ActionButtonType Type => (ActionButtonType)PlayerHelper.ActionButtonType(packedData);
        public uint Action => (uint)PlayerHelper.ActionButtonAction(packedData);


        public ActionButton()
        {
            packedData = 0;
            uState = ActionButtonUpdateState.New;
        }
    
        public void SetActionAndType(uint action, ActionButtonType type)
        {
            ulong newData = action | (ulong)type << 56;
            if (newData != packedData || uState == ActionButtonUpdateState.Deleted)
            {
                packedData = newData;
                if (uState != ActionButtonUpdateState.New)
                    uState = ActionButtonUpdateState.Changed;
            }
        }
    }
}