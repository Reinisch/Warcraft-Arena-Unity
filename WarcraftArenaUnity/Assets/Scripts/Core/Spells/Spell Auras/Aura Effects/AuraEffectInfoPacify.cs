using JetBrains.Annotations;
using UnityEngine;

namespace Core.AuraEffects
{
    [UsedImplicitly, CreateAssetMenu(fileName = "Aura Effect Pacify", menuName = "Game Data/Spells/Auras/Effects/Pacify", order = 2)]
    public class AuraEffectInfoPacify : AuraEffectInfoPreventCasting
    {
        public override SpellPreventionType PreventionType => SpellPreventionType.Silence;
        public override AuraEffectType AuraEffectType => AuraEffectType.Pacify;
    }
}