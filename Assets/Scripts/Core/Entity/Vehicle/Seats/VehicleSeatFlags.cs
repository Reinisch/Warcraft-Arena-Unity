namespace Core
{
    public enum VehicleSeatFlags
    {
        CanDrive = 1 << 0,
        CanCast = 1 << 1,
        CanSwitchSeats = 1 << 2,

        PassengerHidden = 1 << 3,
        PassengerNotSelectable = 1 << 4,
    }
}