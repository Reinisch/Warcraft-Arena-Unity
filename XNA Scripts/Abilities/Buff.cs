using System;
using System.Collections.Generic;

using BasicRpgEngine.Spells;
using BasicRpgEngine.Characters;

namespace BasicRpgEngine.Spells
{
    public class Buff : ICloneable, IDisposable
    {
        public int CurrentStucks { get; private set; }
        public int MaxStucks { get; private set; }
        public int Id { get; private set; }
        public MagicType MagicType { get; private set; }
        public BuffType BuffType { get; private set; }
        public AoeMode AoeMode { get; private set; }
        public string Name { get; private set; }
        public bool HasAbsorbAura { get; private set; }
        public bool HasStucks { get; private set; }
        public bool NeedsRemoval { get; set; }
        public int Radius { get; private set; }

        public ITargetable CasterRef { get; set; }
        public bool IsStuckableSameCaster { get; private set; }
        public bool IsStuckableWithOhters { get; private set; }

        public TimeSpan Duration { get; private set; }
        public TimeSpan TimeLeft { get; private set; }
        public float Seconds { get; private set; }

        public List<AuraBase> Auras { get; private set; }
        public List<BasicScript> Scripts { get; private set; }
        public List<SpellInfoModifierFromBuff> SpellModifiers { get; private set; }
        public List<SpellTrigger> SpellTriggers { get; private set; }

        public Buff(int id, string name, BuffType buffType, MagicType magicType, AoeMode aoeMode,int radius,
            float duration, int currentStucks, int maxStucks, bool hasStucks,
            bool isStuckableSameCaster,bool isStuckableWithOthers, bool hasAbsorb = false)
        {
            Id = id;
            CurrentStucks = currentStucks;
            MaxStucks = maxStucks;
            HasStucks = hasStucks;
            Name = name;
            BuffType = buffType;
            NeedsRemoval = false;
            HasAbsorbAura = hasAbsorb;
            this.Seconds = duration;

            if (duration == -1)
                Duration = TimeSpan.FromSeconds(1);
            else
                Duration = TimeSpan.FromSeconds(duration);
            TimeLeft = Duration;
            MagicType = magicType;
            AoeMode = aoeMode;
            if (AoeMode == AoeMode.None)
                Radius = 0;
            else
                Radius = radius;

            IsStuckableWithOhters = isStuckableWithOthers;
            IsStuckableSameCaster = isStuckableSameCaster;

            Auras = new List<AuraBase>();
            Scripts = new List<BasicScript>();
            SpellModifiers = new List<SpellInfoModifierFromBuff>();
            SpellTriggers = new List<SpellTrigger>();
        }

        public Buff Apply(ITargetable target)
        {
            if (!IsStuckableWithOhters)
                target.Character.Entity.Buffs.RemoveAll(item => item.Name == Name);
            else if (!IsStuckableSameCaster)
                target.Character.Entity.Buffs.RemoveAll(item => item.CasterRef == CasterRef && item.Name == Name);

            Buff newBuff = (Buff)this.Clone();

            foreach (SpellInfoModifierFromBuff modifier in SpellModifiers)
            {
                SpellInfoModifierFromBuff newModifier = (SpellInfoModifierFromBuff)modifier.Clone();
                Spell spell;
                newModifier.BuffRef = newBuff;
                if (target.Character.Entity.Spells.TryGetValue(newModifier.SpellName, out spell))
                    spell.Modifiers.Add(newModifier);
            }

            foreach (SpellTrigger trigger in SpellTriggers)
            {
                SpellTrigger newTrigger = (SpellTrigger)trigger.Clone();
                Spell spell;
                newTrigger.BuffRef = newBuff;
                if (target.Character.Entity.Spells.TryGetValue(newTrigger.SpellName, out spell))
                    spell.Triggers.Add(newTrigger);
            }

            target.Character.Entity.Buffs.Add(newBuff);
            return newBuff;
        }
        public void UpdateScriptTriggers(TimeSpan elapsedTime, Buff buff, SpellData spellData, ITargetable scriptOwner)
        {
            foreach (BasicScript script in Scripts)
                script.Update(elapsedTime, buff, spellData, scriptOwner);
        }
        public bool Update(TimeSpan elapsedTime)
        {
            bool hasTriggered = false;
            if (Seconds != -1)
            {
                TimeLeft -= elapsedTime;
                if (TimeLeft.TotalMilliseconds < 0)
                {
                    TimeLeft = TimeSpan.Zero;
                    foreach (BasicScript script in Scripts)
                        if (script.Update(elapsedTime, this, null, null))
                            hasTriggered = true;
                    return hasTriggered;
                }
                return false;
            }
            foreach (BasicScript script in Scripts)
                if (script.Update(elapsedTime, this, null, null))
                    hasTriggered = true;
            return hasTriggered;
        }
        public void StuckRemoved()
        {
            if (--CurrentStucks < 1)
                NeedsRemoval = true;
        }

        public object Clone()
        {
            Buff clone = new Buff(Id, Name, BuffType, MagicType, AoeMode, Radius, Seconds, CurrentStucks, MaxStucks, HasStucks, IsStuckableSameCaster, IsStuckableWithOhters, HasAbsorbAura);
            foreach (AuraBase modifier in Auras)
                clone.Auras.Add((AuraBase)modifier.Clone());
            foreach (BasicScript script in Scripts)
                clone.Scripts.Add((BasicScript)script.Clone(CasterRef));
            foreach (SpellInfoModifierFromBuff spellModifier in SpellModifiers)
                clone.SpellModifiers.Add((SpellInfoModifierFromBuff)spellModifier.Clone());
            foreach (SpellTrigger trigger in SpellTriggers)
                clone.SpellTriggers.Add((SpellTrigger)trigger.Clone());
            clone.CasterRef = CasterRef;
            return clone;
        }
        public void Dispose()
        {
            NeedsRemoval = true;
            foreach (SpellInfoModifierFromBuff modifier in SpellModifiers)
                modifier.Dispose();
            foreach (BasicScript script in Scripts)
                script.Dispose();
        }
    }
}