namespace Core.AuraEffects
{
    public class AuraEffectControlVehicle : AuraEffect
    {
        public override AuraEffectHandleGroup HandleGroup => AuraEffectHandleGroup.Unique;

        public AuraEffectControlVehicle(Aura aura, AuraEffectInfo effectInfo, int index, float value) : base(aura, effectInfo, index, value)
        {
        }

        public override void HandleEffect(AuraApplication auraApplication, AuraEffectHandleMode mode, bool apply)
        {
            base.HandleEffect(auraApplication, mode, apply);

            if (mode != AuraEffectHandleMode.Normal)
                return;

            Vehicle vehicle = auraApplication.Target.Vehicle;
            if (vehicle == null)
                return;

            Unit passenger = auraApplication.Aura.Caster;
            if (passenger == null)
                return;

            if (passenger == vehicle.Unit)
                return;

            if (apply)
            {
                if (!passenger.IsAlive || passenger.Vehicle == vehicle || vehicle.Unit.IsOnVehicle(passenger))
                    passenger.HandleVehicleApplicationEnter(vehicle, (int)Value, Aura);
            }
            else
            {
                if (vehicle == passenger.Vehicle)
                    passenger.HandleVehicleApplicationExit();
            }
        }
    }
}