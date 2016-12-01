using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;

using BasicRpgEngine.Spells;
using BasicRpgEngine.Graphics;

namespace BasicRpgEngine.Characters
{
    public enum EntityType { Character, Creature }

    public class Entity : IDisposable
    {
        public string EntityName
        {
            get;
            private set;
        }
        public string EntityClass
        {
            get;
            private set;
        }
        public EntityType EntityType
        {
            get;
            private set;
        }

        protected float strength;
        protected float agility;
        protected float intelligence;
        protected float stamina;
        protected float speed;
        protected float hasteRating;
        protected float critChance;
        protected float critDamageMultiplier;
        protected float incomingDamageEfficiency;
        protected float attackPower;
        protected float spellPower;
        protected AttributePair health;
        protected AttributePair mainResourse;

        public AttributePair Health
        {
            get { return health; }
        }
        public AttributePair MainResourse
        {
            get { return mainResourse; }
        }
        public float Speed
        {
            get
            {
                if (IsRooted)
                    return 0;
                else
                    return MathHelper.Clamp(speed * SpeedBonusMod *
                        (IsSnaresSupressed ? 1 : SpeedPenaltyMod), 0f, 180f);
            }
        }
        public float HasteRating
        {
            get { return MathHelper.Clamp(hasteRating + HasteRatingAddMod, 0, 1000); }
        }
        public float CritChance
        {
            get { return MathHelper.Clamp(critChance + CritChanceAddMod, 0, 100); }
        }
        public float CritDamageMultiplier
        {
            get { return MathHelper.Clamp(critDamageMultiplier + CritDamageMultiplierAddMod, 0, 500); }
        }
        public float IncomingDamageEfficiency
        {
            get { return MathHelper.Clamp(incomingDamageEfficiency * IncomingDamageEfficiencyMulMod, 0, 1); }
        }
        public float AttackPower
        {
            get { return (attackPower + AttackPowerAddMod)*AttackPowerMulMod; }
        }
        public float SpellPower
        {
            get { return (spellPower + SpellPowerAddMod)*SpellPowerMulMod; }
        }
        public int Strength
        {
            get { return (int)((strength + StrengthAddMod) * StrengthMulMod); }
        }
        public int Agility
        {
            get { return (int)((agility + AgilityAddMod) * AgilityMulMod); }
        }
        public int Intelligence
        {
            get { return (int)((intelligence + IntelligenceAddMod) * IntelligenceMulMod); }
        }
        public int Stamina
        {
            get { return (int)((stamina + StaminaAddMod) * StaminaMulMod); }
        }

        public bool IsRooted
        {
            get { return RootStateCount > 0; }
        }
        public bool IsNoControlable
        {
            get { return NoControlableStateCount > 0; }
        }
        public bool IsDisarmed
        {
            get { return DisarmStateCount > 0; }
        }
        public bool IsStunned
        {
            get { return StunStateCount > 0; }
        }
        public bool IsSnared
        {
            get { return SnareStateCount > 0; }
        }
        public bool IsFreezed
        {
            get { return FreezeStateCount > 0; }
        }
        public bool IsDisoriented
        {
            get { return DisorientStateCount > 0; }
        }
        public bool IsFeared
        {
            get { return FearStateCount > 0; }
        }
        public bool IsPolymorphed
        {
            get { return PolymorphStateCount > 0; }
        }
        public bool IsSleeping
        {
            get { return SleepStateCount > 0; }
        }
        public bool IsSilenced
        {
            get { return SilenceStateCount > 0; }
        }
        public bool IsKnockedDown
        {
            get { return KnockDownStateCount > 0; }
        }
        public bool IsModelChanged
        {
            get { return ModelChangedStateCount > 0; }
        }
        public bool IsInvulnerable
        {
            get { return InvulnerabilityStateCount > 0; }
        }
        public bool IsPacified
        {
            get { return PacifyStateCount > 0; }
        }
        public bool IsSnaresSupressed
        {
            get { return SnareSupressStateCount > 0; }
        }
        public bool IsNotTargetable
        {
            get { return NotTargetableStateCount > 0; }
        }
        public bool IsInvisible
        {
            get { return InvisibleStateCount > 0; }
        }

        public float HasteRatingAddMod { get; set; }
        public float CritChanceAddMod { get; set; }
        public float CritDamageMultiplierAddMod { get; set; }
        public float CritChanceMulMod { get; set; }
        public float IncomingDamageEfficiencyMulMod { get; set; }
        public float StrengthMulMod { get; set; }
        public float AgilityMulMod { get; set; }
        public float IntelligenceMulMod { get; set; }
        public float StaminaMulMod { get; set; }
        public float StrengthAddMod { get; set; }
        public float AgilityAddMod { get; set; }
        public float IntelligenceAddMod { get; set; }
        public float StaminaAddMod { get; set; }
        public float SpeedBonusMod { get; set; }
        public float SpeedPenaltyMod { get; set; }
        public float SpellPowerMulMod { get; set; }
        public float SpellPowerAddMod { get; set; }
        public float AttackPowerMulMod { get; set; }
        public float AttackPowerAddMod { get; set; }
        public float PhysicalDamageMulMod { get; set; }
        public float FrostDamageMulMod { get; set; }
        public float FireDamageMulMod { get; set; }
        public float ArcaneDamageMulMod { get; set; }
        public float NatureDamageMulMod { get; set; }
        public float ShadowDamageMulMod { get; set; }
        public float HolyDamageMulMod { get; set; }

        public int RootStateCount { get; set; }
        public int NoControlableStateCount { get; set; }
        public int DisarmStateCount { get; set; }
        public int StunStateCount { get; set; }
        public int SnareStateCount { get; set; }
        public int FreezeStateCount { get; set; }
        public int DisorientStateCount { get; set; }
        public int FearStateCount { get; set; }
        public int PolymorphStateCount { get; set; }
        public int SleepStateCount { get; set; }
        public int SilenceStateCount { get; set; }
        public int KnockDownStateCount { get; set; }
        public int InvulnerabilityStateCount { get; set; }
        public int PacifyStateCount { get; set; }
        public int SnareSupressStateCount { get; set; }
        public int NotTargetableStateCount { get; set; }
        public int InvisibleStateCount { get; set; }
        public int ModelChangedStateCount { get; set; }
        public AnimatedSprite CurrentReplacedModel { get; set; }

        public Dictionary<string, Spell> Spells
        {
            get;
            private set;
        }
        public Dictionary<string, AnimatedSprite> ReplacingModels
        {
            get;
            private set;
        }
        public BuffList Buffs
        {
            get;
            private set;
        }

        public Entity(string name, EntityType entityType, EntityData entityData)
        {
            EntityName = name;
            EntityType = entityType;
            EntityClass = entityData.EntityClass;

            stamina = entityData.Stamina;
            agility = entityData.Agility;
            strength = entityData.Strength;
            intelligence = entityData.Intelligence;
            speed = 8.0f;
            hasteRating = 0;
            critChance = 33;
            critDamageMultiplier = 2f;
            incomingDamageEfficiency = 1;
            attackPower = 10;
            spellPower = 10;
            health = new AttributePair(entityData.BaseHealth);
            mainResourse = new AttributePair(entityData.Intelligence);

            HasteRatingAddMod = 0;
            CritChanceAddMod = 0;
            CritDamageMultiplierAddMod = 0;
            CritChanceMulMod = 1;
            IncomingDamageEfficiencyMulMod = 1;
            StrengthMulMod = 1;
            AgilityMulMod = 1;
            IntelligenceMulMod = 1;
            StaminaMulMod = 1;
            StrengthAddMod = 0;
            AgilityAddMod = 0;
            IntelligenceAddMod = 0;
            StaminaAddMod = 0;
            SpeedPenaltyMod = 1;
            SpeedBonusMod = 1;
            SpellPowerMulMod = 1;
            SpellPowerAddMod = 0;
            AttackPowerMulMod = 1;
            AttackPowerAddMod = 0;
            PhysicalDamageMulMod = 1;
            FrostDamageMulMod = 1;
            FireDamageMulMod = 1;
            ArcaneDamageMulMod = 1;
            NatureDamageMulMod = 1;
            ShadowDamageMulMod = 1;
            HolyDamageMulMod = 1;

            RootStateCount = 0;
            NoControlableStateCount = 0;
            DisarmStateCount = 0;
            StunStateCount = 0;
            SnareStateCount = 0;
            FreezeStateCount = 0;
            DisorientStateCount = 0;
            FearStateCount = 0;
            PolymorphStateCount = 0;
            SleepStateCount = 0;
            SilenceStateCount = 0;
            KnockDownStateCount = 0;
            InvulnerabilityStateCount = 0;
            PacifyStateCount = 0;
            SnareSupressStateCount = 0;
            NotTargetableStateCount = 0;
            InvisibleStateCount = 0;
            ModelChangedStateCount = 0;
            CurrentReplacedModel = null;

            Buffs = new BuffList(this);
            ReplacingModels = new Dictionary<string, AnimatedSprite>();
            Spells = new Dictionary<string, Spell>();
        }

        public void Update(TimeSpan elapsedTime, bool maybeBuffChanged)
        {
            foreach (KeyValuePair<string,Spell> spell in Spells)
                spell.Value.SpellCooldown.Update(elapsedTime);

            bool maybeAurasChanged;
            foreach (Buff buff in Buffs)
            {
                maybeAurasChanged = false;
                foreach (AuraBase aura in buff.Auras)
                    if (aura.Update(elapsedTime, this))
                        maybeAurasChanged = true;

                if (maybeAurasChanged)
                    if (buff.Auras.RemoveAll(AuraFade) != 0)
                        maybeBuffChanged = true;
            }

            if (maybeBuffChanged)
            {
                if (IsModelChanged == false)
                    ReplacingModels.Clear();

                Buffs.RemoveAll(buff => buff.TimeLeft == TimeSpan.Zero || buff.NeedsRemoval || (buff.Scripts.Count == 0
                    && buff.Auras.Count == 0 && buff.SpellModifiers.Count == 0 && buff.BuffType != BuffType.Fixed));
            }
        }
        public bool AuraFade(AuraBase aura)
        {
            if (aura.TimeLeft == TimeSpan.Zero)
            {
                aura.Reverse(this);
                return true;
            }
            return false;
        }

        public void Dispose()
        {
            foreach (Spell spell in Spells.Values)
                spell.Dispose();
            Buffs.Dispose();
        }
    }

    public class BuffList : IDisposable
    {
        private Entity entity;
        private List<Buff> buffList;
        private Predicate<Buff> currentMatch;

        public BuffList(Entity entityRef)
        {
            entity = entityRef;
            buffList = new List<Buff>();
            currentMatch = buff => false;
        }

        public int Count
        {
            get { return buffList.Count; }
        }
        public void Add(Buff buff)
        {
            foreach (AuraBase aura in buff.Auras)
                aura.Apply(entity);
            buffList.Add(buff);
        }
        public Buff Find(Predicate<Buff> match)
        {
            return buffList.Find(match);
        }
        public bool Contains(Buff item)
        {
            return buffList.Contains(item);
        }
        public bool Remove(Buff buff)
        {
            if (buffList.Remove(buff))
            {
                foreach (AuraBase aura in buff.Auras)
                    aura.Reverse(entity);
                return true;
            }
            return false;
        }
        public int RemoveAll(Predicate<Buff> match)
        {
            currentMatch = match;
            return buffList.RemoveAll(BuffFade);
        }
        public bool BuffFade(Buff buff)
        {
            if (currentMatch(buff))
            {
                foreach (AuraBase aura in buff.Auras)
                    aura.Reverse(entity);
                buff.Dispose();
                return true;
            }
            return false;
        }
        public IEnumerator<Buff> GetEnumerator()
        {
            return buffList.GetEnumerator();
        }
        public void Dispose()
        {
            entity = null;
            foreach (Buff buff in buffList)
                buff.Dispose();
        }
    }
}