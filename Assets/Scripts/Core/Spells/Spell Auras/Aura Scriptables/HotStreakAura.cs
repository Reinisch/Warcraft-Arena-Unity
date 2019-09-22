using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

namespace Core
{
    [UsedImplicitly, CreateAssetMenu(fileName = "Hot Streak Aura Script", menuName = "Game Data/Spells/Auras/Aura Scripts/Hot Streak", order = 1)]
    internal class HotStreakAura : AuraScriptable, IAuraScriptSpellDamageHandler
    {
        [SerializeField, UsedImplicitly] List<SpellInfo> critCheckedSpells;
        [SerializeField, UsedImplicitly] SpellInfo heatingUpSpell;
        [SerializeField, UsedImplicitly] SpellInfo hotStreakSpell;

        public void OnSpellDamageDone(SpellDamageInfo damageInfo)
        {
            if (!critCheckedSpells.Contains(damageInfo.SpellInfo) || damageInfo.SpellDamageType != SpellDamageType.Direct)
                return;

            bool isCrit = damageInfo.HitType.HasTargetFlag(HitType.CriticalHit);
            if (isCrit)
            {
                if (damageInfo.Caster.Auras.HasAuraWithSpell(heatingUpSpell.Id))
                {
                    damageInfo.Caster.Auras.RemoveAuraWithSpellInfo(heatingUpSpell, AuraRemoveMode.Spell);
                    damageInfo.Caster.Spells.TriggerSpell(hotStreakSpell, damageInfo.Caster);
                }
                else
                    damageInfo.Caster.Spells.TriggerSpell(heatingUpSpell, damageInfo.Caster);
            }
            else
                damageInfo.Caster.Auras.RemoveAuraWithSpellInfo(heatingUpSpell, AuraRemoveMode.Spell);
        }
    }
}