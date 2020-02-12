namespace Core
{
    internal sealed class VehicleSeat
    {
        internal Vehicle Vehicle { get; set; }
        internal VehicleSeatInfo VehicleSeatInfo { get; set; }

        public VehicleSeat(Vehicle vehicle, VehicleSeatInfo vehicleSeatInfo)
        {
            Vehicle = vehicle;
            VehicleSeatInfo = vehicleSeatInfo;
        }

        public void Dispose()
        {
            Vehicle = null;
            VehicleSeatInfo = null;
        }
    }
}