using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public enum MageSpells
{
    FrostBolt,
    FireBall,
    FrostNova,
    Blink,
}

public class WarcraftDatabase : MonoBehaviour
{ 
    public static WarcraftDatabase Instanse { get; private set; }

    #region Spell Info

    public static Dictionary<int, TrinitySpellInfo> SpellInfos { get; private set; }
    public static Dictionary<int, TrinitySpellEffectInfoEntry> SpellEffectEntries { get; private set; }

    public static Dictionary<int, SpellChargeCategory> SpellChargeCategories { get; private set; }
    public static Dictionary<int, SpellCastTimes> SpellCastTimes { get; private set; }
    public static Dictionary<int, SpellDuration> SpellDurations { get; private set; }
    public static Dictionary<int, SpellRange> SpellRanges { get; private set; }
    public static Dictionary<int, SpellPowerCost> SpellPowerCosts { get; private set; }
    public static Dictionary<int, SpellRadius> SpellRadiuses { get; private set; }

    public static Dictionary<int, GameObject> SpellVisuals { get; private set; }
    public static Dictionary<int, AudioClip> SpellCastSounds { get; private set; }
    public static Dictionary<int, AudioClip> SpellHitSounds { get; private set; }

    #endregion

    void Awake()
    {
        if (Instanse == null)
            Instanse = this;
        else
        {
            Destroy(this);
            return;
        }

        Load();

        System.GC.Collect();
    }


    void Load()
    {
        LoadSpells();
    }

    // #TODO : Read from json
    void LoadSpells()
    {
        LoadSpellVisuals();

        LoadSpellSounds();


        LoadSpellRadiuses();

        LoadSpellPowerCosts();

        LoadSpellRanges();

        LoadSpellDurations();

        LoadCastTimes();

        LoadSpellChargeCategories();

        LoadSpellEffectInfo();

        LoadSpellInfo();
    }


    #region Spell Info Loading

    // #TODO : Add references in manager or create <name, id> dictionary or add file
    void LoadSpellVisuals()
    {
        SpellVisuals = new Dictionary<int, GameObject>();

        SpellVisuals.Add(1, Resources.Load<GameObject>("Prefabs/Spells/Frost Nova"));
    }

    void LoadSpellSounds()
    {
        SpellCastSounds = new Dictionary<int, AudioClip>();
        SpellHitSounds = new Dictionary<int, AudioClip>();

        SpellCastSounds.Add(1, Resources.Load<AudioClip>("Sound/Spells/Mage/FrostNova"));
        SpellHitSounds.Add(1, Resources.Load<AudioClip>("Sound/Spells/Mage/IceImpact"));
    }


    void LoadSpellRadiuses()
    {
        SpellRadiuses = new Dictionary<int, SpellRadius>();

        #region Standard Radiuses Id = Meters

        var spellRadius = new SpellRadius()
        {
            Id = 12,
            Radius = 12,
            RadiusMin = 0,
            RadiusMax = 12,
            RadiusPerLevel = 0,
        };
        SpellRadiuses.Add(spellRadius.Id, spellRadius);

        #endregion
    }

    void LoadSpellPowerCosts()
    {
        SpellPowerCosts = new Dictionary<int, SpellPowerCost>();

        // Standard mana minor cost : Referenced in spell helper
        var spellPowerCost = new SpellPowerCost()
        {
            Id = 1,
            ManaCost = 0,
            ManaCostPercentage = 2.0f,
            HealthCostPercentage = 0.0f,
            ManaCostPercentagePerSecond = 0.0f,

            RequiredAura = -1,
        };
        SpellPowerCosts.Add(spellPowerCost.Id, spellPowerCost);
    }

    void LoadSpellRanges()
    {
        SpellRanges = new Dictionary<int, SpellRange>();

        // Self cast : Referenced in spell helper
        var spellRange = new SpellRange()
        {
            Id = 1,
            Flags = SpellRangeFlag.DEFAULT,
        };
        SpellRanges.Add(spellRange.Id, spellRange);
    }

    void LoadSpellDurations()
    {
        SpellDurations = new Dictionary<int, SpellDuration>();

        for(int i = 0; i <= 120; i++)
        {
            SpellDurations.Add(i, new SpellDuration()
            {
                Id = i,
                Duration = i,
                DurationPerLevel = 0,
                MaxDuration = i,
            });
        }
    }

    void LoadCastTimes()
    {
        SpellCastTimes = new Dictionary<int, SpellCastTimes>();

        // Instant cast : Referenced in spell helper
        var spellCastTime = new SpellCastTimes()
        {
            Id = 1,
            CastTime = 0.0f,
            CastTimePerLevel = 0.0f,
            MinCastTime = 0.0f,
        };
        SpellCastTimes.Add(spellCastTime.Id, spellCastTime);
    }

    void LoadSpellChargeCategories()
    {
        SpellChargeCategories = new Dictionary<int, SpellChargeCategory>();

        // No charges : Referenced in spell helper
        var spellCategory = new SpellChargeCategory()
        {
            Id = 1,
            ChargeRecoveryTime = 0,
            MaxCharges = 0,
            ChargeCategoryType = 0,
        };
        SpellChargeCategories.Add(spellCategory.Id, spellCategory);
    }


    void LoadSpellEffectInfo()
    {
        SpellEffectEntries = new Dictionary<int, TrinitySpellEffectInfoEntry>();

        #region Frost Nova Effects Id: 1, 2
        var spellEffectEntry = new TrinitySpellEffectInfoEntry()
        {
            Id = 1,
            Effect = SpellEffectType.SCHOOL_DAMAGE,
            AuraType = AuraType.SPELL_AURA_NONE,
            BonusCoefficient = 0.17f,
            RadiusEntryId = 12,
            TargetA = TargetTypes.TARGET_UNIT_TARGET_ENEMY,
        };
        SpellEffectEntries.Add(spellEffectEntry.Id, spellEffectEntry);
        spellEffectEntry = new TrinitySpellEffectInfoEntry()
        {
            Id = 2,
            Effect = SpellEffectType.APPLY_AURA,
            AuraType = AuraType.SPELL_AURA_MOD_ROOT,
            Mechanic = Mechanics.FREEZE,
            RadiusEntryId = 12,
            TargetA = TargetTypes.TARGET_UNIT_TARGET_ENEMY,
        };
        SpellEffectEntries.Add(spellEffectEntry.Id, spellEffectEntry);
        #endregion
    }

    void LoadSpellInfo()
    {
        SpellInfos = new Dictionary<int, TrinitySpellInfo>();

        #region Frost Nova Id : 1, Effects : 1, 2

        var spellInfo = new TrinitySpellInfo()
        {
            Id = 1,
            Dispel = DispelType.MAGIC,
            Mechanic = Mechanics.FREEZE,
            Attributes = SpellAttributes.DAMAGE_DOESNT_BREAK_AURAS | SpellAttributes.CANT_BE_REFLECTED,
            Targets = TargetTypes.TARGET_UNIT_SRC_AREA_ENEMY,
            SchoolMask = SpellSchoolMask.FROST,
            DamageClass = SpellDamageClass.MAGIC,
            PreventionType = SpellPreventionType.SILENCE | SpellPreventionType.PACIFY,
            ExplicitTargetMask = SpellCastTargetFlags.UNIT_ENEMY,
            InterruptFlags = SpellInterruptFlags.NONE,
            FamilyName = SpellFamilyNames.MAGE,

            ChargeCategory = SpellHelper.ZeroChargeCategory,
            CastTime = SpellHelper.InstantCastTime,
            Range = SpellHelper.SelfCastRange,
            Duration = SpellDurations[8],
            PowerCosts = new List<SpellPowerCost>() { SpellHelper.BasicManaCost },

            RecoveryTime = 0.5f,
            StartRecoveryTime = 0.0f,
            StartRecoveryCategory = 0,

            Speed = 0.0f,

            StackAmount = 0,
            MaxAffectedTargets = 0,

            SpellIconId = 1,
            ActiveIconId = 2,
            VisualId = 1,
        };

        spellInfo.SpellEffectInfos.Add(new TrinitySpellEffectInfo(spellInfo, 0, SpellEffectEntries[1]));
        spellInfo.SpellEffectInfos.Add(new TrinitySpellEffectInfo(spellInfo, 1, SpellEffectEntries[2]));

        SpellInfos.Add(spellInfo.Id, spellInfo);

        #endregion
    }

    #endregion
}
