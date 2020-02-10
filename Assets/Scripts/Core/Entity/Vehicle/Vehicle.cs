namespace Core
{
    internal sealed class Vehicle
    {
        internal Unit Unit { get; set; }
        internal VehicleInfo VehicleInfo { get; set; }
        internal CreatureInfo CreatureInfo { get; set; }

        public Vehicle(Unit unit, VehicleInfo vehicleInfo, CreatureInfo creatureInfo)
        {
            Unit = unit;
            VehicleInfo = vehicleInfo;
            CreatureInfo = creatureInfo;
        }

        public void Dispose()
        {
            Unit = null;
            VehicleInfo = null;
            CreatureInfo = null;
        }
    }
}
