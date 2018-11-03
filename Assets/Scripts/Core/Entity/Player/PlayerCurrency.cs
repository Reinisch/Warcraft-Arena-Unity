using System;

namespace Core
{
    [Serializable]
    public class PlayerCurrency
    {
        private PlayerCurrencyState state;
        private uint quantity;
        private uint weeklyQuantity;
        private uint trackedQuantity;
        private byte flags;

        public PlayerCurrencyState State { get { return state; } set { state = value; } }
        public uint Quantity { get { return quantity; } set { quantity = value; } }
        public uint WeeklyQuantity { get { return weeklyQuantity; } set { weeklyQuantity = value; } }
        public uint TrackedQuantity { get { return trackedQuantity; } set { trackedQuantity = value; } }
        public byte Flags { get { return flags; } set { flags = value; } }
    }
}