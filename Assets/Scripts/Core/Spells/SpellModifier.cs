using Core.AuraEffects;

namespace Core
{
    internal class SpellModifier
    {
        public Aura Aura { get; }
        public SpellModifierType ModifierType { get; }
        public SpellModifierApplicationType ApplicationType { get; }
        public (SpellModifierType, SpellModifierApplicationType) Kind { get; }

        public float Value { get; set; }

        public SpellModifier(Aura ownerAura, AuraEffectInfoSpellModifier auraEffectInfo)
        {
            Aura = ownerAura;
            ModifierType = auraEffectInfo.ModifierType;
            ApplicationType = auraEffectInfo.ApplicationType;

            Kind = (ModifierType, ApplicationType);
        }
    }
}