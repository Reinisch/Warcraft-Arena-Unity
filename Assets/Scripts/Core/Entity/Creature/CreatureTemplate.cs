using System;
using System.Collections.Generic;

namespace Core
{
    public class CreatureTemplate
    {
        public uint Entry;
        public uint[] DifficultyEntry; // UnitHelper.MaxCreatureDifficulties
        public uint[] KillCredit; // UnitHelper.MaxKillCredit
        public uint Modelid1;
        public uint Modelid2;
        public uint Modelid3;
        public uint Modelid4;
        public string Name;
        public string FemaleName;
        public string SubName;
        public string IconName;
        public uint GossipMenuId;
        public short Minlevel;
        public short Maxlevel;
        public int HealthScalingExpansion;
        public uint RequiredExpansion;
        public uint VignetteID;             /// @todo Read Vignette.db2
        public uint Faction;
        public long Npcflag;
        public float SpeedWalk;
        public float SpeedRun;
        public float Scale;
        public uint Rank;
        public uint Dmgschool;
        public uint BaseAttackTime;
        public uint RangeAttackTime;
        public float BaseVariance;
        public float RangeVariance;
        public uint UnitClass;                                     // enum Classes. Note only 4 classes are known for creatures.
        public UnitFlags UnitFlags;                                     // enum UnitFlags mask values
        public uint Dynamicflags;
        public CreatureFamily Family;                               // enum CreatureFamily values (optional)
        public uint TrainerType;
        public uint TrainerClass;
        public uint TrainerRace;
        public CreatureType Type;                                   // enum CreatureType values
        public CreatureTypeFlags TypeFlags;                        // enum CreatureTypeFlags mask values
        public uint Lootid;
        public uint PickpocketLootId;
        public uint SkinLootId;
        public Dictionary<SpellSchools, int> Resistance;
        public uint[] Spells; // UnitHelper.CreatureMaxSpells
        public uint VehicleId;
        public uint Mingold;
        public uint Maxgold;
        public string AIName;
        public uint MovementType;
        public InhabitTypeValues InhabitType;
        public float HoverHeight;
        public float ModHealth;
        public float ModHealthExtra;
        public float ModMana;
        public float ModManaExtra;  // Added in 4.x, this value is usually 2 for a small group of creatures with double mana
        public float ModArmor;
        public float ModDamage;
        public float ModExperience;
        public bool RacialLeader;
        public uint MovementId;
        public bool RegenHealth;
        public uint MechanicImmuneMask;
        public CreatureFlagsExtra FlagsExtra;
        public uint ScriptID;

        public uint GetRandomValidModelId() { throw new NotImplementedException(); }
        public uint GetFirstValidModelId() { throw new NotImplementedException(); }
        public uint GetFirstInvisibleModel() { throw new NotImplementedException(); }
        public uint GetFirstVisibleModel() { throw new NotImplementedException(); }

        public SkillType GetRequiredLootSkill()
        {
            if ((TypeFlags & CreatureTypeFlags.Herbloot) > 0)
                return SkillType.Herbalism;
            if ((TypeFlags & CreatureTypeFlags.Miningloot) > 0)
                return SkillType.Mining;
            if ((TypeFlags & CreatureTypeFlags.Engineerloot) > 0)
                return SkillType.Engineering;
            return SkillType.Skinning;  // normal case
        }

        public bool IsExotic()
        {
            return (TypeFlags & CreatureTypeFlags.Exotic) != 0;
        }

        public bool IsTameable(bool canTameExotic)
        {
            if (Type != CreatureType.Beast || Family == 0 || (TypeFlags & CreatureTypeFlags.Tameable) == 0)
                return false;

            // if can tame exotic then can tame any tameable
            return canTameExotic || !IsExotic();
        }
    }
}