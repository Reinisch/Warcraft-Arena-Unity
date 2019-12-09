using System.Collections.Generic;

namespace Core.AuraEffects
{
    public class AuraEffectChangeDisplayModel : AuraEffect
    {
        public new AuraEffectInfoChangeDisplayModel EffectInfo { get; }
        public override AuraEffectHandleGroup HandleGroup => AuraEffectHandleGroup.Unique;

        public AuraEffectChangeDisplayModel(Aura aura, AuraEffectInfoChangeDisplayModel effectInfo, int index, float value) : base(aura, effectInfo, index, value)
        {
            EffectInfo = effectInfo;
        }

        public override void HandleEffect(AuraApplication auraApplication, AuraEffectHandleMode mode, bool apply)
        {
            base.HandleEffect(auraApplication, mode, apply);

            if (mode != AuraEffectHandleMode.Normal)
                return;

            if (apply)
            {
                if (IsReplacementValid(auraApplication.Target.TransformSpellInfo, Aura.SpellInfo))
                    auraApplication.Target.UpdateTransformSpell(this);
            }
            else
            {
                auraApplication.Target.ResetTransformSpell();
                IReadOnlyList<AuraEffect> transformEffects = auraApplication.Target.Auras.GetAuraEffects(AuraEffectType.ChangeDisplayModel);
                if (transformEffects != null)
                    for (int i = 0; i < transformEffects.Count; i++)
                        if (IsReplacementValid(auraApplication.Target.TransformSpellInfo, transformEffects[i].Aura.SpellInfo))
                            auraApplication.Target.UpdateTransformSpell((AuraEffectChangeDisplayModel)transformEffects[i]);
            }
        }

        private bool IsReplacementValid(SpellInfo source, SpellInfo target) => source == null || !target.IsPositive || source.IsPositive;
    }
}
