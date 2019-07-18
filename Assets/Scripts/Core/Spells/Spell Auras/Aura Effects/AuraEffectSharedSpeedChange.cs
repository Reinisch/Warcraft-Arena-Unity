using Common;

namespace Core.AuraEffects
{
    public class AuraEffectSharedSpeedChange : AuraEffect
    {
        public override AuraEffectHandleGroup HandleGroup => AuraEffectHandleGroup.SpeedChange;

        public AuraEffectSharedSpeedChange(Aura aura, AuraEffectInfo effectInfo, int index, float value) : base(aura, effectInfo, index, value)
        {
        }

        public override void HandleEffect(AuraApplication auraApplication, AuraEffectHandleMode mode, bool apply)
        {
            Logging.LogAura($"Handle aura effect {EffectInfo.name} for target: {auraApplication.Target.Name} in mode {mode}, applying: {apply}");

            auraApplication.Target.AttributeUnitController.UpdateSpeed(UnitMoveType.Run);
        }
    }
}