using System.Collections.Generic;

namespace Core
{
    internal sealed class Vehicle
    {
        private readonly List<VehicleSeat> seats = new List<VehicleSeat>();
        private readonly Dictionary<Unit, VehicleSeat> takenSeatsByUnits = new Dictionary<Unit, VehicleSeat>();

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
            takenSeatsByUnits.Clear();
        }

        public bool AddPassenger(Unit passenger, int requestedSeatIndex)
        {
            if (!TryFindSuitableSeat(requestedSeatIndex, out VehicleSeat suitableSeat))
                return false;

            suitableSeat.Passenger = passenger;
            takenSeatsByUnits.Add(passenger, suitableSeat);

            return true;
        }

        public void RemovePassenger(Unit passenger)
        {
            if (!takenSeatsByUnits.TryGetValue(passenger, out VehicleSeat takenSeat))
                return;

            takenSeatsByUnits.Remove(passenger);
            takenSeat.Passenger = null;
        }

        private bool TryFindSuitableSeat(int requestedSeatIndex, out VehicleSeat suitableSeat)
        {
            if (requestedSeatIndex < 0 || requestedSeatIndex >= seats.Count)
                requestedSeatIndex = 0;

            suitableSeat = seats[requestedSeatIndex].Passenger == null
                ? seats[requestedSeatIndex]
                : seats.Find(seat => seat.Passenger == null);

            return suitableSeat != null;
        }
    }
}
