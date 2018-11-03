using System;
using UnityEngine;

// Defines base stats for creatures (used to calculate HP/mana/armor/attackpower/rangedattackpower/all damage).
namespace Core
{
    public class CreatureBaseStats
    {
        public uint[] BaseHealth; // GlobalHelper.MaxExpansions
        public uint BaseMana;
        public uint BaseArmor;
        public uint AttackPower;
        public uint RangedAttackPower;
        public float[] BaseDamage; // GlobalHelper.MaxExpansions


        public uint GenerateHealth(CreatureTemplate info)
        {
            return (uint)Mathf.CeilToInt(BaseHealth[info.HealthScalingExpansion] * info.ModHealth * info.ModHealthExtra);
        }

        public uint GenerateMana(CreatureTemplate info)
        {
            return BaseMana == 0 ? 0 : (uint)Mathf.CeilToInt(BaseMana * info.ModMana * info.ModManaExtra);
        }

        public uint GenerateArmor(CreatureTemplate info)
        {
            return (uint)Mathf.CeilToInt(BaseArmor * info.ModArmor);
        }

        public float GenerateBaseDamage(CreatureTemplate info)
        {
            return BaseDamage[info.HealthScalingExpansion];
        }

        public static CreatureBaseStats GetBaseStats(byte level, UnitClass unitClass) { throw new NotImplementedException(); }
    }
}