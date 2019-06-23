using System.Collections.Generic;
using UnityEngine;

namespace Core
{
    internal class SpellUniqueTargets
    {
        private readonly Spell spell;
        private readonly HashSet<Unit> targetSet = new HashSet<Unit>();

        internal List<SpellTargetEntry> Entries { get; } = new List<SpellTargetEntry>();

        internal SpellUniqueTargets(Spell spell)
        {
            this.spell = spell;
        }

        internal void Dispose()
        {
            Entries.Clear();
            targetSet.Clear();
        }

        internal void AddTargetIfNotExists(Unit target)
        {
            if (targetSet.Contains(target))
                return;

            SpellTargetEntry spellTargetEntry = new SpellTargetEntry();
            spellTargetEntry.Target = target;
            spellTargetEntry.Processed = false;
            spellTargetEntry.Alive = target.IsAlive;
            spellTargetEntry.Damage = 0;
            spellTargetEntry.Crit = false;
            spellTargetEntry.ScaleAura = false;

            // calculate hit result
            if (spell.OriginalCaster != null)
            {
                spellTargetEntry.MissCondition = spell.OriginalCaster.SpellHitResult(target, spell.SpellInfo, spell.CanReflect);
                if (spellTargetEntry.MissCondition != SpellMissType.Immune)
                    spellTargetEntry.MissCondition = SpellMissType.None;
            }
            else
                spellTargetEntry.MissCondition = SpellMissType.Evade;

            // calculate hit delay for spells with speed
            if (spell.SpellInfo.Speed > 0.0f && spell.Caster != target)
            {
                float distance = Mathf.Clamp(Vector3.Distance(spell.Caster.Position, target.Position), StatUtils.DefaultCombatReach, float.MaxValue);
                spellTargetEntry.Delay = distance / spell.SpellInfo.Speed;
            }
            else
                spellTargetEntry.Delay = 0.0f;

            Entries.Add(spellTargetEntry);
            targetSet.Add(target);
        }

        internal int TargetCountForEffect(int effectIndex)
        {
            int count = 0;
            for (int i = 0; i < Entries.Count; i++)
                if (Entries[i].EffectMask.HasBit(effectIndex))
                    count++;

            return count;
        }
    }
}