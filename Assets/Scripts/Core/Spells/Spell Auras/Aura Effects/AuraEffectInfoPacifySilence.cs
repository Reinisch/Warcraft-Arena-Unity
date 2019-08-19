using JetBrains.Annotations;
using UnityEngine;

namespace Core.AuraEffects
{
    [UsedImplicitly, CreateAssetMenu(fileName = "Aura Effect Silence Pacify", menuName = "Game Data/Spells/Auras/Effects/Silence and Pacify", order = 2)]
    public class AuraEffectInfoSilencePacify : AuraEffectInfoPreventCasting
    {
        public override SpellPreventionType PreventionType => SpellPreventionType.Silence | SpellPreventionType.Pacify;
        public override AuraEffectType AuraEffectType => AuraEffectType.SilencePacify;
    }
}