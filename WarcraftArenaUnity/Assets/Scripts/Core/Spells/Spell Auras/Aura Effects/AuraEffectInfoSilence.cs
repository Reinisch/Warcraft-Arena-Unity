using JetBrains.Annotations;
using UnityEngine;

namespace Core.AuraEffects
{
    [UsedImplicitly, CreateAssetMenu(fileName = "Aura Effect Silence", menuName = "Game Data/Spells/Auras/Effects/Silence", order = 2)]
    public class AuraEffectInfoSilence : AuraEffectInfoPreventCasting
    {
        public override SpellPreventionType PreventionType => SpellPreventionType.Silence;
        public override AuraEffectType AuraEffectType => AuraEffectType.Silence;
    }
}