using Core.AuraEffects;

namespace Core
{
    internal class SpellModifier
    {
        public Aura Aura { get; }
        public AuraEffectInfoSpellModifier AuraModifier { get; }

        public float Value { get; set; }

        public SpellModifierType ModifierType => AuraModifier.ModifierType;
        public SpellModifierApplicationType ApplicationType => AuraModifier.ApplicationType;
        public (SpellModifierType, SpellModifierApplicationType) Kind => (ModifierType, ApplicationType);

        public SpellModifier(Aura ownerAura, AuraEffectInfoSpellModifier auraEffectInfo)
        {
            Aura = ownerAura;
            AuraModifier = auraEffectInfo;
        }
    }
}