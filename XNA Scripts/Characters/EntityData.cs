using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BasicRpgEngine.Characters
{
    [Serializable]
    public class EntityData
    {
        public string EntityName;
        public string EntityType;
        public string EntityClass;

        public int AttackPower;
        public int SpellPower;

        public float SpellPowerMulMod;
        public float AttacPowerMulMod;
        public float SpellPowerAddMod;
        public float AttackPowerAddMod;

        public float StrengthMulMod;
        public float AgilityMulMod;
        public float IntelligenceMulMode;
        public float StaminaMulMod;

        public float StrengthAddMod;
        public float AgilityAddMod;
        public float IntelligenceAddMod;
        public float StaminaAddMod;

        public int Stamina;
        public int Agility;
        public int Strength;
        public int Intelligence;

        public float HasteRating;
        public float CritChance;
        public float CritDamageMultiplier;
        public float DamageReduction;

        public float HasteRatingAddMod;
        public float CritChanceAddMod;
        public float CritDamageMultiplierAddMod;
        public float DamageReductionAddMod;

        public float Speed;
        public float SpeedAddMod;
        public float SpeedMulMod;

        public bool IsRooted;
        public bool IsNoControlable;
        public bool IsDisarmed;
        public bool IsStunned;
        public bool IsSnared;
        public bool IsFreezed;
        public bool IsDisoriented;
        public bool IsFeared;
        public bool IsPolymorphed;
        public bool IsSilenced;
        public bool IsSleeping;
        public bool IsModelChanged;

        public int CurrentHealth;
        public int BaseHealth;

        public int CurrentMainResourse;
        public int MainResourse;

        public List<string> Buffs;
        public List<string> ReplacingModels;
        public List<string> Spells;

        private EntityData()
        {}
        public EntityData(string entityClass, int stamina, int agility, int strength, int intelligence, int baseHealth)
        {
            EntityClass = entityClass;
            Stamina = stamina;
            Agility = agility;
            Strength = strength;
            Intelligence = intelligence;
            BaseHealth = baseHealth;
        }

        public object Clone()
        {
            EntityData data = new EntityData();

            data.EntityClass = this.EntityClass;
            data.Stamina = this.Stamina;
            data.Agility = this.Agility;
            data.Strength = this.Strength;
            data.Intelligence = this.Intelligence;
            data.BaseHealth = this.BaseHealth;

            return data;
        }
    }
}