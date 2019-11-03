using System.Collections.Generic;
using Common;
using UnityEngine;

namespace Core
{
    internal class SpellImplicitTargets
    {
        private readonly Spell spell;
        private readonly Dictionary<Unit, SpellTargetEntry> targetEntryByTarget = new Dictionary<Unit, SpellTargetEntry>();

        internal List<SpellTargetEntry> Entries { get; } = new List<SpellTargetEntry>();

        internal SpellImplicitTargets(Spell spell)
        {
            this.spell = spell;
        }

        internal void Dispose()
        {
            Entries.Clear();
            targetEntryByTarget.Clear();
        }

        internal void AddTargetIfNotExists(Unit target, int effectMask)
        {
            if (!targetEntryByTarget.TryGetValue(target, out SpellTargetEntry targetEntry))
            {
                var newEntry = new SpellTargetEntry
                {
                    Target = target,
                    Processed = false,
                    Alive = target.IsAlive,
                    Crit = false,
                    EffectMask = effectMask
                };

                targetEntryByTarget.Add(target, newEntry);
                Entries.Add(newEntry);
            }
            else
                targetEntry.EffectMask |= effectMask;
        }

        internal void RemoveTargetIfExists(Unit target)
        {
            if (targetEntryByTarget.ContainsKey(target))
            {
                targetEntryByTarget.Remove(target);
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

                targetEntry.Crit = spell.Caster.Spells.IsSpellCrit(targetEntry.Target, spell.SpellInfo, spell.SchoolMask, spell);

                isDelayed |= targetEntry.Delay > 0;
            }
        }

        internal bool Contains(Unit target)
        {
            return targetEntryByTarget.ContainsKey(target);
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