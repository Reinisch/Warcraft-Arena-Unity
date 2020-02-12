using System.Collections.Generic;

namespace Core
{
    internal sealed class Vehicle
    {
        private readonly List<VehicleSeat> seats = new List<VehicleSeat>();

        internal Unit Unit { get; set; }
        internal VehicleInfo VehicleInfo { get; set; }
        internal CreatureInfo CreatureInfo { get; set; }

        public Vehicle(Unit unit, VehicleInfo vehicleInfo, CreatureInfo creatureInfo)
        {
            Unit = unit;
            VehicleInfo = vehicleInfo;
            CreatureInfo = creatureInfo;

            for (int i = 0; i < vehicleInfo.Seats.Count; i++)
            {
                seats.Add(new VehicleSeat(this, vehicleInfo.Seats[i]));
            }
        }

        public void Dispose()
        {
            for (int i = 0; i < VehicleInfo.Seats.Count; i++)
            {
                seats[i].Dispose();
            }

            Unit = null;
            VehicleInfo = null;
            CreatureInfo = null;

            seats.Clear();
        }
    }
}
