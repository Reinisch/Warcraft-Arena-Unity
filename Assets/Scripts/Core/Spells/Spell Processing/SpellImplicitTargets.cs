using System.Collections.Generic;
using UnityEngine;

namespace Core
{
    internal class SpellImplicitTargets
    {
        private readonly Spell spell;
        private readonly HashSet<Unit> targetSet = new HashSet<Unit>();

        internal List<SpellTargetEntry> Entries { get; } = new List<SpellTargetEntry>();

        internal SpellImplicitTargets(Spell spell)
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
            if (!targetSet.Contains(target))
            {
                targetSet.Add(target);

                Entries.Add(new SpellTargetEntry
                {
                    Target = target,
                    Processed = false,
                    Alive = target.IsAlive,
                    Damage = 0,
                    Crit = false,
                });
            }
        }

        internal void RemoveTargetIfExists(Unit target)
        {
            if (targetSet.Contains(target))
            {
                targetSet.Remove(target);
                for (int i = Entries.Count - 1; i >= 0; i--)
                    if (Entries[i].Target == target)
                        Entries.RemoveAt(i);
            }
        }

        internal void HandleLaunch(out bool isDelayed, out SpellProcessingToken processingToken)
        {
            isDelayed = false;
            processingToken = new SpellProcessingToken
            {
                ServerFrame = BoltNetwork.ServerFrame,
                Source = spell.ExplicitTargets.Source,
                Destination = spell.ExplicitTargets.Destination ?? Vector3.zero,
            };

            foreach (SpellTargetEntry targetEntry in Entries)
            {
                // calculate hit result
                if (spell.OriginalCaster != null)
                {
                    targetEntry.MissCondition = spell.OriginalCaster.Spells.SpellHitResult(targetEntry.Target, spell.SpellInfo, spell.CanReflect);
                    if (targetEntry.MissCondition != SpellMissType.Immune)
                        targetEntry.MissCondition = SpellMissType.None;
                }
                else
                    targetEntry.MissCondition = SpellMissType.Evade;

                // calculate hit delay for spells with speed
                if (spell.SpellInfo.Speed > 0.0f && spell.Caster != targetEntry.Target)
                {
                    float distance = Mathf.Clamp(Vector3.Distance(spell.Caster.Position, targetEntry.Target.Position), StatUtils.DefaultCombatReach, float.MaxValue);
                    targetEntry.Delay = Mathf.FloorToInt(distance / spell.SpellInfo.Speed * 1000.0f);
                    processingToken.ProcessingEntries.Add((targetEntry.Target.Id, targetEntry.Delay));
                }
                else
                    targetEntry.Delay = 0;

                targetEntry.Crit = spell.Caster.Spells.IsSpellCrit(targetEntry.Target, spell.SpellInfo, spell.SchoolMask);

                isDelayed |= targetEntry.Delay > 0;
            }
        }

        internal bool Contains(Unit target)
        {
            return targetSet.Contains(target);
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