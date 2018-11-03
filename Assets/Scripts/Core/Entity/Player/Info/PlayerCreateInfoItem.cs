namespace Core
{
    public class PlayerCreateInfoItem
    {
        public uint ItemID;
        public uint ItemAmount;


        public PlayerCreateInfoItem(uint id, uint amount)
        {
            ItemID = id;
            ItemAmount = amount;
        }
    }
}