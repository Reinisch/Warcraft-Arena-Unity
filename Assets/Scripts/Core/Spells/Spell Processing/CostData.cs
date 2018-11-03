namespace Core
{
    public struct CostData
    {
        public PowerType Power;
        public int Amount;

        public CostData(PowerType power, int amount)
        {
            Power = power;
            Amount = amount;
        }
    }
}