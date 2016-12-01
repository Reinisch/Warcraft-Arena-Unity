using System.Collections.Generic;

using BasicRpgEngine.Spells;

namespace BasicRpgEngine.Spells
{
    public enum SpellCastType { Instant, Casted }
    public enum SpellTargetMode { Self, Target }
    public enum SpellGlobalCooldownMode { Normal, NoCooldown }
    public enum AoeMode { None, Self, Cone, Cleave }
    public enum DamageType { Physical, Frost, Fire, Arcane, Nature, Shadow, Holy }
    public enum BuffType { Buff, Debuff, Fixed }
    public enum MagicType { Magic, Physical }
    public enum AuraType { Root, Silence, Speed, Stun, Freeze, Control, HasteRating, CritChance,
        DamageReduction, CritDamageMultiplier, DamageOverTime, HealOverTime, AbsorbDamage, ModelChange,
        Invulnerability, Pacify, SnareSupression, NoTargetable, Invisibility, PhysicalDamageMultiplier,
        FrostDamageMultiplier, FireDamageMultiplier, ArcaneDamageMultiplier, NatureDamageMultiplier,
        ShadowDamageMultiplier, HolyDamageMultiplier }

    public enum AuraControlEffect { None, Disorient, Fear, Polymorph, Sleep }

    public class SpellData
    {
        public byte ID;
        public string Name;
        public SpellCastType SpellCastType;
        public SpellTargetMode SpellTargetMode;
        public SpellGlobalCooldownMode SpellGlobal;
        public Cooldown SpellCooldown;
        public float BaseCastTime;
        public int Range;
        public int ActivationCost;
        public bool IsHarmful = false;
        public bool CastableInStun = false;
        public bool CastableInSilence = false;
        public bool CastableInControl = false;
        public Buff Buff;
        public List<BaseEffect> Effects;

        public SpellData()
        {
            Effects = new List<BaseEffect>();
        }
    }
}