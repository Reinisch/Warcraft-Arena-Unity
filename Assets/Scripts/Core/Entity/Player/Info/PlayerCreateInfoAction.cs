namespace Core
{
    public class PlayerCreateInfoAction
    {
        public byte Button { get; set; }
        public byte Type { get; set; }
        public uint Action { get; set; }


        public PlayerCreateInfoAction()
        {
            Button = 0;
            Action = 0;
            Type = 0;
        }

        public PlayerCreateInfoAction(byte button, uint action, byte type)
        {
            Button = button;
            Action = action;
            Type = type;
        }
    }
}