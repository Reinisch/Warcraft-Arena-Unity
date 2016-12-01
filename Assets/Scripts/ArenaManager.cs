using UnityEngine;
using System.Collections.Generic;

public class ArenaManager: MonoBehaviour
{
    public static ArenaManager Instanse { get; set; }

    public static List<Unit> ArenaUnits { get { return Instanse.arenaUnits; } }
    public static PlayerInterface PlayerInterface { get { return Instanse.playerInterface; } }

    public SpellInfo SpellInfo { get; set; }
    public GameObject Trash { get; set; }
    public GameObject Effects { get; set; }

    public Unit PlayerUnit { get; set; }

    [SerializeField]
    PlayerInterface playerInterface;

    List<Unit> arenaUnits = new List<Unit>();

    public Dictionary<int, ModifierApplier> SpellModifiers { get; private set; }
    public Dictionary<int, SpellScriptDelegate> SpellScripts { get; private set; }
    public Dictionary<string, Sprite> SpellIcons { get; private set; }
    public SpellDataStorage SpellLibrary { get; private set; }

    void Awake()
    {
        if(Instanse == null)
            Instanse = this;
        else
        {
            Destroy(this);
            return;
        }

        SpellInfo = new SpellInfo(0, "");
        PlayerUnit = GameObject.FindGameObjectWithTag("Player").GetComponent<Unit>();
        PlayerUnit.id = 1;

        Trash = GameObject.Find("Trash");
        Effects = GameObject.Find("Effects");

        #region Spell Scripts

        #region Buff Expired Spell Trigger: Cast arg[0] spell after buff expired : Script Id = 1
        SpellScripts = new Dictionary<int, SpellScriptDelegate>();
        SpellScripts.Add(1, delegate(SpellScriptInfoArgs scriptInfo, SpellScriptInvokeArgs spellArgs, float[] args)
        {
            Spell spell = spellArgs.caster.character.Spells.GetSpell((int)args[0]);
            SpellData spellData = spellArgs.world.SpellLibrary.GetSpell((int)args[0]);
            spellArgs.world.ApplySpell(spellArgs.caster, spellArgs.target, spell, spellData);
        });
        #endregion

        #endregion

        #region Spell Modifiers

        #region Shatter:  If target is frozen deal arg[0] times more damage, with arg[2] time more plus arg[1] % crit chance : Modifier Id = 1
        SpellModifiers = new Dictionary<int,ModifierApplier>();
        SpellModifiers.Add(1, delegate(Unit caster, Unit target, SpellInfo spellInfo, Spell spell, ArenaManager world, float[] args)
            {
                if (target.Character.states[EntityStateType.Freeze].InEffect)
                {
                    spellInfo.parameters[ParameterType.DamageDealing][ParameterValue.BonusMultiplier] *= args[0];
                    spellInfo.parameters[ParameterType.CritChance][ParameterValue.FinalBonusAddition] += args[1];
                    spellInfo.parameters[ParameterType.CritChance][ParameterValue.BonusMultiplier] *= args[2];
                    return true;
                }
                return false;
            });
        #endregion

        #endregion

        #region Spells
        SpellLibrary = new SpellDataStorage();

        DamageEffect damageEffect;
        TeleportEffect teleportEffect;
        KnockBackEffect knockBackEffect;
        ApplyBuffEffect applyBuffEffect;
        BuffRemovalEffect buffRemovalEffect;
        AoeEffect aoeEffect;
        SpellData newSpellData;
        Buff buff;

        //Mage
        #region Fire Blast : Spell Id = 1 / Buff Id = None
        // Fire Blast Spell Data
        newSpellData = new SpellData();
        newSpellData.id = 1;
        newSpellData.spellName = "Fire Blast";
        newSpellData.iconName = "Icons/Mage/Spell_Mage_InfernoBlast";
        newSpellData.effectName = null;
        newSpellData.castSoundName = null;
        newSpellData.hitSoundName = "Sound/Spells/Mage/FireBlast";
        newSpellData.spellProjectileName = null;
        newSpellData.range = 5;
        newSpellData.activationCost = 10;      
        newSpellData.baseCooldown = 0.5f;
        newSpellData.baseCastTime = 0;
        newSpellData.spellCastType = SpellCastType.Instant;
        newSpellData.spellTargetMode = SpellTargetMode.SingleTarget;
        newSpellData.spellGlobal = SpellGlobalCooldownMode.Normal;
        newSpellData.spellProjectileType = SpellProjectileEffect.Instant;

        //Enemy, Single Target ,40-50 Fire Damage 
        damageEffect = new DamageEffect(DamageType.Fire, 30, 40);
        newSpellData.Effects.Add(damageEffect);
        //Add Fire Blast Spell
        SpellLibrary.AddSpellData(newSpellData);
        #endregion

        #region Frost Nova : Spell Id = 2 / Buff Id = 1
        //New Spell
        newSpellData = new SpellData();
        newSpellData.id = 2;
        newSpellData.spellName = "Frost Nova";
        newSpellData.iconName = "Icons/Mage/Spell_Frost_FrostNova";
        newSpellData.effectName = "Frost Nova";
        newSpellData.castSoundName = "Sound/Spells/Mage/FrostNova";
        newSpellData.hitSoundName = "Sound/Spells/Mage/IceImpact";
        newSpellData.spellProjectileName = null;
        newSpellData.range = 0;
        newSpellData.activationCost = 20;
        newSpellData.baseCooldown = 0;
        newSpellData.baseCastTime = 0;
        newSpellData.spellCastType = SpellCastType.Instant;
        newSpellData.spellTargetMode = SpellTargetMode.NoTarget;
        newSpellData.spellGlobal = SpellGlobalCooldownMode.NoCooldown;
        newSpellData.spellProjectileType = SpellProjectileEffect.Instant;
        //Enemy, Self Area:4 , Frost Damage 15-15
        damageEffect = new DamageEffect(DamageType.Frost, 15, 15);
        aoeEffect = new AoeEffect(damageEffect, AoeMode.Self, 4);
        newSpellData.Effects.Add(aoeEffect);
        //Magic Debuff for 3 sec
        buff = new Buff(1, 2, "Frost Nova", "Icons/Mage/Spell_Frost_FrostNova", BuffType.Debuff, AbilityType.Magic, 3, 0, 0, false, false);
        buff.buffEffectType = BuffEffectType.StaticEffect;
        buff.buffEffectName = "Frost Nova Freeze State";
        //Aura Root
        buff.Auras.Add(new StateModifier(EntityStateType.Root));
        //Area Freeze
        buff.Auras.Add(new StateModifier(EntityStateType.Freeze));
        applyBuffEffect = new ApplyBuffEffect(buff);
        aoeEffect = new AoeEffect(applyBuffEffect, AoeMode.Self, 4);
        newSpellData.Effects.Add(aoeEffect);
        //Add Frost Nova Spell
        SpellLibrary.AddSpellData(newSpellData);
        #endregion

        #region Blazing Speed : Spell Id = 3 / Buff Id = 2
        //New Spell
        newSpellData = new SpellData();
        newSpellData.id = 3;
        newSpellData.spellName = "Blazing Speed";
        newSpellData.iconName = "Icons/Mage/Spell_Fire_BurningSpeed";
        newSpellData.effectName = null;
        newSpellData.castSoundName = "Sound/Spells/Mage/BlazingSpeed";
        newSpellData.hitSoundName = null;
        newSpellData.spellProjectileName = null;
        newSpellData.range = 0;
        newSpellData.activationCost = 0;
        newSpellData.baseCooldown = 5;
        newSpellData.baseCastTime = 0;
        newSpellData.spellCastType = SpellCastType.Instant;
        newSpellData.spellTargetMode = SpellTargetMode.Self;
        newSpellData.spellGlobal = SpellGlobalCooldownMode.NoCooldown;
        newSpellData.spellProjectileType = SpellProjectileEffect.Instant;
        //Magic Buff for 3 sec
        buff = new Buff(2, 3, "Blazing Speed", "Icons/Mage/Spell_Fire_BurningSpeed", BuffType.Buff, AbilityType.Magic, 3, 0, 0, true, true);
        buff.buffEffectType = BuffEffectType.None;
        //Aura 150% Speed
        buff.Auras.Add(new ParameterModifier(ParameterType.Speed, ParameterValue.BonusMultiplier, 2f));
        //Area Freeze
        applyBuffEffect = new ApplyBuffEffect(buff);
        newSpellData.Effects.Add(applyBuffEffect);
        //Add Frost Nova Spell
        SpellLibrary.AddSpellData(newSpellData);
        #endregion

        #region Pyroblast : Spell Id = 4 / Buff Id = 8
        // Fire Blast Spell Data
        newSpellData = new SpellData();
        newSpellData.id = 4;
        newSpellData.spellName = "Pyroblast";
        newSpellData.iconName = "Icons/Mage/Spell_Fire_Fireball02";
        newSpellData.effectName = null;
        newSpellData.castSoundName = null;
        newSpellData.hitSoundName = "Sound/Spells/Mage/FireBlast";
        newSpellData.spellProjectileName = "Pyroblast";
        newSpellData.range = 10;
        newSpellData.activationCost = 10;
        newSpellData.baseCooldown = 0.5f;
        newSpellData.baseCastTime = 1.5f;
        newSpellData.spellCastType = SpellCastType.Casted;
        newSpellData.spellTargetMode = SpellTargetMode.SingleTarget;
        newSpellData.spellGlobal = SpellGlobalCooldownMode.Normal;
        newSpellData.spellProjectileType = SpellProjectileEffect.TargetedProjectile;

        //Enemy, Single Target ,40-50 Fire Damage 
        damageEffect = new DamageEffect(DamageType.Fire, 60, 120);
        newSpellData.Effects.Add(damageEffect);

        buff = new Buff(8, 4, "Pyroblast", "Icons/Mage/Spell_Fire_Fireball02", BuffType.Debuff, AbilityType.Magic, 6, 0, 0, false, false);
        buff.buffEffectType = BuffEffectType.None;
        buff.buffEffectName = null;
        damageEffect = new DamageEffect(DamageType.Fire, 20, 30);
        buff.Auras.Add(new PeriodicEffectAura(1, damageEffect, buff));
        //Area Freeze
        applyBuffEffect = new ApplyBuffEffect(buff);
        newSpellData.Effects.Add(applyBuffEffect);
        //Add Fire Blast Spell
        SpellLibrary.AddSpellData(newSpellData);
        #endregion

        #region Ice Barrier : Spell Id = 5 / Buff Id = 3
        //New Spell
        newSpellData = new SpellData();
        newSpellData.id = 5;
        newSpellData.spellName = "Ice Barrier";
        newSpellData.iconName = "Icons/Mage/spell_ice_lament";
        newSpellData.effectName = null;
        newSpellData.castSoundName = "Sound/Spells/Mage/IceBarrirerImpact";
        newSpellData.hitSoundName = null;
        newSpellData.spellProjectileName = null;
        newSpellData.range = 0;
        newSpellData.activationCost = 0;
        newSpellData.baseCooldown = 0.5f;
        newSpellData.baseCastTime = 0;
        newSpellData.spellCastType = SpellCastType.Instant;
        newSpellData.spellTargetMode = SpellTargetMode.Self;
        newSpellData.spellGlobal = SpellGlobalCooldownMode.Normal;
        newSpellData.spellProjectileType = SpellProjectileEffect.Instant;
        //Magic Buff for 30 sec
        buff = new Buff(3, 5, "Ice Barrier", "Icons/Mage/spell_ice_lament", BuffType.Buff, AbilityType.Magic, 30, 0, 0, true, true);
        buff.buffEffectType = BuffEffectType.StaticEffect;
        buff.buffEffectName = "Ice Barrier";
        //Aura 100 absorb
        buff.Auras.Add(new AbsorbAura(100, buff));
        //Area Freeze
        applyBuffEffect = new ApplyBuffEffect(buff);
        newSpellData.Effects.Add(applyBuffEffect);
        //Add Frost Nova Spell
        SpellLibrary.AddSpellData(newSpellData);
        #endregion

        #region Frost Bolt : Spell Id = 6 / Buff Id = 4
        // Fire Blast Spell Data
        newSpellData = new SpellData();
        newSpellData.id = 6;
        newSpellData.spellName = "Frost Bolt";
        newSpellData.iconName = "Icons/Mage/Spell_Frost_FrostBolt02";
        newSpellData.effectName = null;
        newSpellData.castSoundName = null;
        newSpellData.hitSoundName = "Sound/Spells/Mage/IceImpact";
        newSpellData.spellProjectileName = "Frost Bolt";
        newSpellData.range = 10;
        newSpellData.activationCost = 10;
        newSpellData.baseCooldown = 0;
        newSpellData.baseCastTime = 1.0f;
        newSpellData.spellCastType = SpellCastType.Casted;
        newSpellData.spellTargetMode = SpellTargetMode.SingleTarget;
        newSpellData.spellGlobal = SpellGlobalCooldownMode.Normal;
        newSpellData.spellProjectileType = SpellProjectileEffect.TargetedProjectile;
        //Enemy, Single Target 20-50 Frost Damage 
        damageEffect = new DamageEffect(DamageType.Fire, 30, 50);
        damageEffect.Modifiers.Add(new SpellModifier(1, 2, 50, 2));
        newSpellData.Effects.Add(damageEffect);
        //Magic Buff for 5 sec
        buff = new Buff(4, 6, "Frost Bolt", "Icons/Mage/Spell_Frost_FrostBolt02", BuffType.Debuff, AbilityType.Magic, 5, 0, 0, false, false);
        buff.buffEffectType = BuffEffectType.None;
        buff.buffEffectName = null;
        buff.Auras.Add(new ParameterModifier(ParameterType.Speed, ParameterValue.PenaltyMultiplier, 0.6f));
        //Area Freeze
        applyBuffEffect = new ApplyBuffEffect(buff);
        newSpellData.Effects.Add(applyBuffEffect);
        //Add Fire Blast Spell
        SpellLibrary.AddSpellData(newSpellData);
        #endregion

        #region Invicibility : Spell Id = 7 / Buff Id = 5
        //New Spell
        newSpellData = new SpellData();
        newSpellData.id = 7;
        newSpellData.spellName = "Invicibility";
        newSpellData.iconName = "Icons/Mage/ability_mage_GreaterInvisibility";
        newSpellData.effectName = null;
        newSpellData.castSoundName = "Sound/Spells/Mage/Invisibility_Impact_Chest";
        newSpellData.hitSoundName = null;
        newSpellData.spellProjectileName = null;
        newSpellData.range = 0;
        newSpellData.activationCost = 0;
        newSpellData.baseCooldown = 5f;
        newSpellData.baseCastTime = 0;
        newSpellData.spellCastType = SpellCastType.Instant;
        newSpellData.spellTargetMode = SpellTargetMode.Self;
        newSpellData.spellGlobal = SpellGlobalCooldownMode.NoCooldown;
        newSpellData.spellProjectileType = SpellProjectileEffect.Instant;
        //Magic Buff for 3 sec
        buff = new Buff(5, 7, "Invicibility", "Icons/Mage/ability_mage_GreaterInvisibility", BuffType.Buff, AbilityType.Magic, 3, 0, 0, true, true);
        buff.buffEffectType = BuffEffectType.None;
        //Aura Invicible + Fade
        buff.Auras.Add(new FadeAura(0.2f));
        buff.Auras.Add(new StateModifier(EntityStateType.Invisible));
        //Area Freeze
        applyBuffEffect = new ApplyBuffEffect(buff);
        newSpellData.Effects.Add(applyBuffEffect);
        //Add Frost Nova Spell
        SpellLibrary.AddSpellData(newSpellData);
        #endregion

        #region Deep Freeze : Spell Id = 8 / Buff Id = 6
        //New Spell
        newSpellData = new SpellData();
        newSpellData.id = 8;
        newSpellData.spellName = "Deep Freeze";
        newSpellData.iconName = "Icons/Mage/Ability_Mage_DeepFreeze";
        newSpellData.effectName = null;
        newSpellData.castSoundName = null;
        newSpellData.hitSoundName = "Sound/Spells/Mage/DeepFreeze";
        newSpellData.spellProjectileName = null;
        newSpellData.range = 10;
        newSpellData.activationCost = 20;
        newSpellData.baseCooldown = 10;
        newSpellData.baseCastTime = 0;
        newSpellData.spellCastType = SpellCastType.Instant;
        newSpellData.spellTargetMode = SpellTargetMode.SingleTarget;
        newSpellData.spellGlobal = SpellGlobalCooldownMode.NoCooldown;
        newSpellData.spellProjectileType = SpellProjectileEffect.Instant;
        //Magic Debuff for 3 sec
        buff = new Buff(6, 8, "Deep Freeze", "Icons/Mage/Ability_Mage_DeepFreeze", BuffType.Debuff, AbilityType.Magic, 3, 0, 0, false, false);
        buff.buffEffectType = BuffEffectType.StaticEffect;
        buff.buffEffectName = "Deep Freeze";
        //Aura Root
        buff.Auras.Add(new StateModifier(EntityStateType.Stun));
        //Area Freeze
        buff.Auras.Add(new StateModifier(EntityStateType.Freeze));
        applyBuffEffect = new ApplyBuffEffect(buff);
        newSpellData.Effects.Add(applyBuffEffect);
        //Add Deep Freeze Spell
        SpellLibrary.AddSpellData(newSpellData);
        #endregion

        #region Blink : Spell Id = 9 / Buff Id = None
        //New Spell
        newSpellData = new SpellData();
        newSpellData.id = 9;
        newSpellData.spellName = "Blink";
        newSpellData.iconName = "Icons/Mage/Spell_Arcane_Blink";
        newSpellData.effectName = null;
        newSpellData.castSoundName = "Sound/Spells/Mage/Teleport";
        newSpellData.hitSoundName = null;
        newSpellData.spellProjectileName = null;
        newSpellData.range = 0;
        newSpellData.activationCost = 0;
        newSpellData.baseCooldown = 0.5f;
        newSpellData.baseCastTime = 0;
        newSpellData.spellCastType = SpellCastType.Instant;
        newSpellData.spellTargetMode = SpellTargetMode.Self;
        newSpellData.spellGlobal = SpellGlobalCooldownMode.NoCooldown;
        newSpellData.spellProjectileType = SpellProjectileEffect.Instant;
        newSpellData.flags = SpellFlags.CastableWhileStunned;
        //Teleport forward 5
        buffRemovalEffect = new BuffRemovalEffect(delegate(Buff removalBuff, Unit caster, Unit target, Spell spell, ArenaManager world)
            {
                for (int i = 0; i < removalBuff.Auras.Count; i++)
                {
                    if (removalBuff.Auras[i] is StateModifier)
                    {
                        StateModifier aura = removalBuff.Auras[i] as StateModifier;
                        if (aura.stateId == EntityStateType.Stun || aura.stateId == EntityStateType.Root)
                            return true;
                    }
                }
                return false;
            });
        newSpellData.Effects.Add(buffRemovalEffect);
        teleportEffect = new TeleportEffect(15);
        newSpellData.Effects.Add(teleportEffect);
        //Add Frost Nova Spell
        SpellLibrary.AddSpellData(newSpellData);
        #endregion

        #region Blast Wave : Spell Id = 10 / Buff Id = 7
        //New Spell
        newSpellData = new SpellData();
        newSpellData.id = 10;
        newSpellData.spellName = "Blast Wave";
        newSpellData.iconName = "Icons/Mage/spell_holy_excorcism_02";
        newSpellData.effectName = "Blast Wave";
        newSpellData.castSoundName = "Sound/Spells/Mage/FireNova";
        newSpellData.hitSoundName = null;
        newSpellData.spellProjectileName = null;
        newSpellData.range = 0;
        newSpellData.activationCost = 20;
        newSpellData.baseCooldown = 5;
        newSpellData.baseCastTime = 0;
        newSpellData.spellCastType = SpellCastType.Instant;
        newSpellData.spellTargetMode = SpellTargetMode.NoTarget;
        newSpellData.spellGlobal = SpellGlobalCooldownMode.Normal;
        newSpellData.spellProjectileType = SpellProjectileEffect.Instant;
        //Enemy, Self Area:4 , Frost Damage 15-15
        damageEffect = new DamageEffect(DamageType.Fire, 15, 15);
        aoeEffect = new AoeEffect(damageEffect, AoeMode.Self, 5);
        newSpellData.Effects.Add(aoeEffect);

        knockBackEffect = new KnockBackEffect(4, 3);
        aoeEffect = new AoeEffect(knockBackEffect, AoeMode.Self, 5);
        newSpellData.Effects.Add(aoeEffect);
        //Magic Debuff for 1 sec
        buff = new Buff(7, 10, "Blast Wave", "Icons/Mage/spell_holy_excorcism_02", BuffType.Debuff, AbilityType.Magic, 1f, 0, 0, false, false);
        buff.buffEffectType = BuffEffectType.None;
        buff.buffEffectName = null;
        //Aura Root
        buff.Auras.Add(new StateModifier(EntityStateType.Knockback));
        buff.Auras.Add(new ParameterModifier(ParameterType.Speed, ParameterValue.PenaltyMultiplier, 0.3f));
        applyBuffEffect = new ApplyBuffEffect(buff);
        aoeEffect = new AoeEffect(applyBuffEffect, AoeMode.Self, 5);
        newSpellData.Effects.Add(aoeEffect);
        //Add Frost Nova Spell
        SpellLibrary.AddSpellData(newSpellData);
        #endregion

        #region Living Bomb : Spell Id = 11 / Buff Id = 9
        // Fire Blast Spell Data
        newSpellData = new SpellData();
        newSpellData.id = 11;
        newSpellData.spellName = "Living Bomb";
        newSpellData.iconName = "Icons/Mage/Ability_Mage_LivingBomb";
        newSpellData.effectName = null;
        newSpellData.castSoundName = null;
        newSpellData.hitSoundName = "Sound/Spells/Mage/LivingBombCast";
        newSpellData.spellProjectileName = null;
        newSpellData.range = 10;
        newSpellData.activationCost = 10;
        newSpellData.baseCooldown = 0.5f;
        newSpellData.baseCastTime = 0;
        newSpellData.spellCastType = SpellCastType.Instant;
        newSpellData.spellTargetMode = SpellTargetMode.SingleTarget;
        newSpellData.spellGlobal = SpellGlobalCooldownMode.Normal;
        newSpellData.spellProjectileType = SpellProjectileEffect.Instant;

        //Enemy, Single Target ,40-50 Fire Damage 
        damageEffect = new DamageEffect(DamageType.Fire, 20, 30);
        newSpellData.Effects.Add(damageEffect);

        buff = new Buff(9, 11, "Living Bomb", "Icons/Mage/Ability_Mage_LivingBomb", BuffType.Debuff, AbilityType.Magic, 2, 0, 0, false, false);
        buff.buffEffectType = BuffEffectType.None;
        buff.buffEffectName = null;
        damageEffect = new DamageEffect(DamageType.Fire, 10, 25);
        buff.Auras.Add(new PeriodicEffectAura(0.5f, damageEffect, buff));
        buff.Auras.Add(new SpellScriptAura(new SpellScriptInfoArgs(11, CharacterEventTypes.OnBuffExpired, 1, null, null), buff, 12));
        //Area Freeze
        applyBuffEffect = new ApplyBuffEffect(buff);
        newSpellData.Effects.Add(applyBuffEffect);
        //Add Fire Blast Spell
        SpellLibrary.AddSpellData(newSpellData);
        #endregion

        #region Living Bomb Explosion : Spell Id = 12 / Buff Id = None
        //New Spell
        newSpellData = new SpellData();
        newSpellData.id = 12;
        newSpellData.spellName = "Living Bomb Explosion";
        newSpellData.iconName = "Icons/Mage/Ability_Mage_LivingBomb";
        newSpellData.effectName = null;
        newSpellData.castSoundName = null;
        newSpellData.hitSoundName = "Sound/Spells/Mage/LivingBombArea";
        newSpellData.spellProjectileName = null;
        newSpellData.range = 0;
        newSpellData.activationCost = 0;
        newSpellData.baseCooldown = 0;
        newSpellData.baseCastTime = 0;
        newSpellData.spellCastType = SpellCastType.Instant;
        newSpellData.spellTargetMode = SpellTargetMode.SingleTarget;
        newSpellData.spellGlobal = SpellGlobalCooldownMode.NoCooldown;
        newSpellData.spellProjectileType = SpellProjectileEffect.Instant;
        //Enemy, Self Area:4 , Frost Damage 15-15
        damageEffect = new DamageEffect(DamageType.Fire, 150, 150);
        aoeEffect = new AoeEffect(damageEffect, AoeMode.Splash, 5);
        newSpellData.Effects.Add(aoeEffect);
        //Add Frost Nova Spell
        SpellLibrary.AddSpellData(newSpellData);
        #endregion

        #region Ice Lance: Spell Id = 13 / Buff Id = 10
        // Fire Blast Spell Data
        newSpellData = new SpellData();
        newSpellData.id = 13;
        newSpellData.spellName = "Ice Lance";
        newSpellData.iconName = "Icons/Mage/Spell_Frost_ChillingBlast";
        newSpellData.effectName = null;
        newSpellData.castSoundName = null;
        newSpellData.hitSoundName = "Sound/Spells/Mage/IceLanceImpact";
        newSpellData.spellProjectileName = "Ice Lance";
        newSpellData.range = 10;
        newSpellData.activationCost = 10;
        newSpellData.baseCooldown = 0;
        newSpellData.baseCastTime = 0;
        newSpellData.spellCastType = SpellCastType.Instant;
        newSpellData.spellTargetMode = SpellTargetMode.SingleTarget;
        newSpellData.spellGlobal = SpellGlobalCooldownMode.Normal;
        newSpellData.spellProjectileType = SpellProjectileEffect.TargetedProjectile;
        //Enemy, Single Target 20-50 Frost Damage 
        damageEffect = new DamageEffect(DamageType.Fire, 10, 20);
        damageEffect.Modifiers.Add(new SpellModifier(1, 4, 50, 2));
        newSpellData.Effects.Add(damageEffect);
        //Add Fire Blast Spell
        SpellLibrary.AddSpellData(newSpellData);
        #endregion

        //Warrior
        #region Charge : Spell Id = 100/ Buff Id = 100
        //New Spell
        newSpellData = new SpellData();
        newSpellData.id = 100;
        newSpellData.spellName = "Charge";
        newSpellData.iconName = "Icons/Warrior/Ability_Warrior_Charge";
        newSpellData.effectName = null;
        newSpellData.castSoundName = "Sound/Spells/Warrior/HeroricLeap";
        newSpellData.hitSoundName = null;
        newSpellData.spellProjectileName = null;
        newSpellData.range = 0;
        newSpellData.activationCost = 0;
        newSpellData.baseCooldown = 4f;
        newSpellData.baseCastTime = 0;
        newSpellData.spellCastType = SpellCastType.Instant;
        newSpellData.spellTargetMode = SpellTargetMode.Self;
        newSpellData.spellGlobal = SpellGlobalCooldownMode.NoCooldown;
        newSpellData.spellProjectileType = SpellProjectileEffect.Instant;
        //Magic Buff for 3 sec
        buff = new Buff(100, 100, "Charge", "Icons/Warrior/Ability_Warrior_Charge", BuffType.Buff, AbilityType.Physical, 3, 0, 0, true, true);
        buff.buffEffectType = BuffEffectType.None;
        //Aura 150% Speed
        buff.Auras.Add(new ParameterModifier(ParameterType.Speed, ParameterValue.BonusMultiplier, 2f));
        //Area Freeze
        applyBuffEffect = new ApplyBuffEffect(buff);
        newSpellData.Effects.Add(applyBuffEffect);
        //Add Frost Nova Spell
        SpellLibrary.AddSpellData(newSpellData);
        #endregion

        #region Mortal Strike : Spell Id = 101 / Buff Id = None
        // Fire Blast Spell Data
        newSpellData = new SpellData();
        newSpellData.id = 101;
        newSpellData.spellName = "Mortal Strike";
        newSpellData.iconName = "Icons/Warrior/Ability_Warrior_SavageBlow";
        newSpellData.effectName = null;
        newSpellData.castSoundName = null;
        newSpellData.hitSoundName = "Sound/Spells/Warrior/m2hSwordHitFlesh1c";
        newSpellData.spellProjectileName = null;
        newSpellData.range = 1.5f;
        newSpellData.activationCost = 10;
        newSpellData.baseCooldown = 0.5f;
        newSpellData.baseCastTime = 0;
        newSpellData.spellCastType = SpellCastType.Instant;
        newSpellData.spellTargetMode = SpellTargetMode.SingleTarget;
        newSpellData.spellGlobal = SpellGlobalCooldownMode.Normal;
        newSpellData.spellProjectileType = SpellProjectileEffect.Instant;

        //Enemy, Single Target ,40-50 Fire Damage 
        damageEffect = new DamageEffect(DamageType.Fire, 20, 40);
        newSpellData.Effects.Add(damageEffect);
        //Add Fire Blast Spell
        SpellLibrary.AddSpellData(newSpellData);
        #endregion

        #endregion

        #region Spell Icons
        SpellIcons = new Dictionary<string, Sprite>();
        for (int i = 0; i < SpellLibrary.Count; i++)
        {
            if(!SpellIcons.ContainsKey(SpellLibrary[i].iconName))
                SpellIcons.Add(SpellLibrary[i].iconName, Resources.Load<Sprite>(SpellLibrary[i].iconName));
        }
        SpellIcons.Add("Default", Resources.Load<Sprite>("Icons/Mage/Spell_Arcane_StudentOfMagic"));
        #endregion       
    }

    void Start()
    {
        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Characters"), LayerMask.NameToLayer("Characters"));

        PlayerUnit.Character.Spells.AddSpell(new Spell(1, "Fire Blast", new Cooldown(SpellLibrary.GetSpell(1).baseCooldown)));
        PlayerUnit.Character.Spells.AddSpell(new Spell(2, "Frost Nova", new Cooldown(SpellLibrary.GetSpell(2).baseCooldown)));
        PlayerUnit.Character.Spells.AddSpell(new Spell(3, "Blazing Speed", new Cooldown(SpellLibrary.GetSpell(3).baseCooldown)));
        PlayerUnit.Character.Spells.AddSpell(new Spell(4, "Pyroblast", new Cooldown(SpellLibrary.GetSpell(4).baseCooldown)));
        PlayerUnit.Character.Spells.AddSpell(new Spell(5, "Ice Barrier", new Cooldown(SpellLibrary.GetSpell(5).baseCooldown)));
        PlayerUnit.Character.Spells.AddSpell(new Spell(6, "Frost Bolt", new Cooldown(SpellLibrary.GetSpell(6).baseCooldown)));
        PlayerUnit.Character.Spells.AddSpell(new Spell(7, "Invicibility", new Cooldown(SpellLibrary.GetSpell(7).baseCooldown)));
        PlayerUnit.Character.Spells.AddSpell(new Spell(8, "Deep Freeze", new Cooldown(SpellLibrary.GetSpell(8).baseCooldown)));
        PlayerUnit.Character.Spells.AddSpell(new Spell(9, "Blink", new Cooldown(SpellLibrary.GetSpell(9).baseCooldown)));
        PlayerUnit.Character.Spells.AddSpell(new Spell(10, "Blast Wave", new Cooldown(SpellLibrary.GetSpell(10).baseCooldown)));
        PlayerUnit.Character.Spells.AddSpell(new Spell(11, "Living Bomb", new Cooldown(SpellLibrary.GetSpell(11).baseCooldown)));
        PlayerUnit.Character.Spells.AddSpell(new Spell(12, "Living Bomb Explosion", new Cooldown(SpellLibrary.GetSpell(12).baseCooldown)));
        PlayerUnit.Character.Spells.AddSpell(new Spell(13, "Ice Lance", new Cooldown(SpellLibrary.GetSpell(13).baseCooldown)));

        PlayerUnit.Character.Spells.AddSpell(new Spell(100, "Charge", new Cooldown(SpellLibrary.GetSpell(100).baseCooldown)));
        PlayerUnit.Character.Spells.AddSpell(new Spell(101, "Mortal Strike", new Cooldown(SpellLibrary.GetSpell(101).baseCooldown)));

        PlayerInterface.Initialize();
    }


    public void OnUnitDead(int index)
    {
        if (arenaUnits[index].character.health.currentValue == 0 && arenaUnits[index].IsDead == false)
        {
            arenaUnits[index].IsDead = true;
            float translateY = ((BoxCollider2D)(arenaUnits[index].GetComponent<Collider2D>())).size.y / 2;
            arenaUnits[index].GetComponent<Rigidbody2D>().isKinematic = true;
            arenaUnits[index].GetComponent<Collider2D>().enabled = false;

            GameObject deadCreature = new GameObject();
            deadCreature.AddComponent<Rigidbody2D>();
            deadCreature.AddComponent<BoxCollider2D>();
            deadCreature.GetComponent<Rigidbody2D>().gravityScale = 2.5f;
            deadCreature.GetComponent<Rigidbody2D>().velocity = new Vector2(UnityEngine.Random.Range(-10, 10), UnityEngine.Random.Range(5, 10));
            deadCreature.GetComponent<Collider2D>().isTrigger = true;
            deadCreature.transform.position = arenaUnits[index].transform.position;
            arenaUnits[index].gameObject.transform.parent = deadCreature.transform;

            arenaUnits[index].transform.Translate(0, -translateY, 0);
            deadCreature.transform.Translate(0, translateY, 0);
            deadCreature.transform.parent = Trash.transform;
            
            if (PlayerUnit.character.target != null)
            {
                if (PlayerUnit.character.target.id == arenaUnits[index].id)
                    PlayerInterface.CheckTargetUnitDeath(arenaUnits[index].id);
            }

            for (int i = 0; i < arenaUnits.Count; i++)
            {
                if (arenaUnits[i].character.target != null && arenaUnits[i].character.target.id == arenaUnits[index].id)
                    arenaUnits[i].character.target = null;
            }
            arenaUnits.RemoveAt(index);

        }
    }

    public void OnPlayerDead()
    {
        
    }


    private bool CheckSpellTargetMode(Unit caster, SpellData spellData)
    {
        if(spellData.spellTargetMode == SpellTargetMode.SingleTarget)
        {
            if (caster.Character.target == null || Vector3.Distance(caster.transform.position, caster.Character.target.transform.position) > spellData.range)
                return false;
        }
        return true;
    }

    private bool CheckSpellCooldown(Unit caster, Spell spell, SpellData spellData)
    {
        if (spell.spellCooldown.HasCooldown)
            return false;

        if (caster.Character.GlobalCooldown.HasCooldown)
        {
            if(spellData.spellGlobal != SpellGlobalCooldownMode.NoCooldown)
                return false;
        }

        return true;
    }

    private bool CheckSpellCastType(Unit caster, Spell spell, SpellData spellData)
    {
        switch (spellData.spellCastType)
        {
            case SpellCastType.Casted:
                if (caster.Character.SpellCast.HasCast)
                    return false;
                else
                {
                    caster.Character.SpellCast = new SpellCast(spell, spellData, caster, caster.Character.target);
                    ApplyGlobalCooldown(caster, spell, spellData);
                    caster.Animator.SetBool("Casting", true);
                    return true;
                }
            case SpellCastType.Instant:
                if (caster.Character.SpellCast.HasCast)
                    return false;
                else
                {
                    if (spellData.spellProjectileName != null)
                    {
                        GameObject effect = Instantiate(Resources.Load("Prefabs/Spells/" + spellData.spellProjectileName), caster.castPoint != null ? caster.castPoint.position :
                            new Vector3(caster.transform.position.x, caster.transform.position.y+ caster.GetComponent<Collider2D>().bounds.size.y * 3 / 4, caster.transform.position.z),
                            Quaternion.identity) as GameObject;

                        TargetedProjectile projectile = effect.GetComponent<TargetedProjectile>();
                        projectile.Initialize(this, caster, caster.character.target, spell);
                        effect.transform.parent = Effects.transform;
                        ApplyGlobalCooldown(caster, spell, spellData);
                        spell.spellCooldown.Apply();
                        caster.Animator.SetTrigger("InstantAttack");

                        GameObject skillUseEvent = MonoBehaviour.Instantiate(Resources.Load("Prefabs/UI/SkillUseEvent")) as GameObject;
                        skillUseEvent.GetComponent<SkillUseUIEvent>().Initialize(spellData, caster, PlayerInterface);
                        return false;
                    }

                    ApplyGlobalCooldown(caster, spell, spellData);
                    spell.spellCooldown.Apply();
                    caster.Animator.SetTrigger("InstantAttack");
                    return true;
                }
        }
        return false;
    }

    private void ApplySpell(Unit caster, Unit target, Spell spell, SpellData spellData)
    {
        if (spellData.hitSoundName != null)
        {
            AudioClip audio = Resources.Load<AudioClip>(spellData.hitSoundName);
            AudioSource.PlayClipAtPoint(audio, caster.transform.position);
        }
        foreach (IEffect effect in spellData.Effects)
        {
            if (effect.AoeMode == AoeMode.None)
            {
                switch (spellData.spellTargetMode)
                {
                    case SpellTargetMode.SingleTarget:
                        if (!target.isDead)
                        {
                            effect.Apply(caster, target, spell, this);
                            if (target.character.health.currentValue == 0)
                            {
                                if (target.IsHumanPlayer == false)
                                    OnUnitDead(arenaUnits.IndexOf(target));
                                else
                                    OnPlayerDead();
                            }
                        }
                        break;
                    case SpellTargetMode.Self:
                        effect.Apply(caster, caster, spell, this);
                        if (caster.character.health.currentValue == 0)
                        {
                            if (target.IsHumanPlayer == false)
                                OnUnitDead(arenaUnits.IndexOf(target));
                            else
                                OnPlayerDead();
                        }
                        break;
                    case SpellTargetMode.NoTarget:
                        effect.Apply(caster, null, spell, this);
                        if (caster.character.health.currentValue == 0)
                        {
                            if (target.IsHumanPlayer == false)
                                OnUnitDead(arenaUnits.IndexOf(target));
                            else
                                OnPlayerDead();
                        }
                        break;
                }

            }
            else
                ApplyAoeEffect(caster, target, effect as AoeEffect, spell);
        }
    }

    private void ApplyGlobalCooldown(Unit caster, Spell spell, SpellData spellData)
    {
        switch (spellData.spellGlobal)
        {
            case SpellGlobalCooldownMode.Normal:
                caster.Character.GlobalCooldown.Apply();
                break;
            case SpellGlobalCooldownMode.NoCooldown:
                break;
        }
    }   

    private void ApplyAoeEffect(Unit caster, Unit target, AoeEffect aoeEffect,Spell spell)
    {
        switch (aoeEffect.AoeMode)
        {
            case AoeMode.Front:
                break;
            case AoeMode.Self:
                break;
            case AoeMode.Splash:
                break;
        }

        for(int i = arenaUnits.Count - 1; i >= 0; i--)
        {
            if (!arenaUnits[i].isDead)
            {
                if (Vector3.Distance(arenaUnits[i].transform.position, caster.transform.position) < aoeEffect.Radius)
                {
                    aoeEffect.Effect.Apply(caster, arenaUnits[i], spell, this);
                    if (arenaUnits[i].character.health.currentValue == 0)
                    {
                        if (arenaUnits[i].IsHumanPlayer == false)
                            OnUnitDead(arenaUnits.IndexOf(arenaUnits[i]));
                        else
                            OnPlayerDead();
                    }
                }
            }
        }
    }
    
    public void ApplyEffect(Unit caster, Unit target, IEffect effect, int spellId)
    {
        SpellData spellData = SpellLibrary.GetSpell(spellId);
        Spell spell = caster.Character.Spells.GetSpell(spellId);
        if (effect.AoeMode == AoeMode.None)
        {
            switch (spellData.spellTargetMode)
            {
                case SpellTargetMode.SingleTarget:
                    if (!target.isDead)
                    {
                        effect.Apply(caster, target, spell, this);
                        if (target.character.health.currentValue == 0)
                        {
                            if (target.IsHumanPlayer == false)
                                OnUnitDead(arenaUnits.IndexOf(target));
                            else
                                OnPlayerDead();
                        }
                    }
                    break;
                case SpellTargetMode.Self:
                    effect.Apply(caster, caster, spell, this);
                    if (caster.character.health.currentValue == 0)
                    {
                        if (target.IsHumanPlayer == false)
                            OnUnitDead(arenaUnits.IndexOf(target));
                        else
                            OnPlayerDead();
                    }
                    break;
                case SpellTargetMode.NoTarget:
                    effect.Apply(caster, null, spell, this);
                    if (caster.character.health.currentValue == 0)
                    {
                        if (target.IsHumanPlayer == false)
                            OnUnitDead(arenaUnits.IndexOf(target));
                        else
                            OnPlayerDead();
                    }
                    break;
            }

        }
        else
            ApplyAoeEffect(caster, target, effect as AoeEffect, spell);
    }

    public bool CastSpell(Unit caster, Unit target, int spellId)
    {
        GameObject skillUseEvent;

        #region If spell not found or caster/target is disposed
        SpellData spellData = SpellLibrary.GetSpell(spellId);
        Spell spell = caster.Character.Spells.GetSpell(spellId);
        if (caster.IsDead)
            return false;
        #endregion

        #region If caster can't cast due to states, cooldown and false target
        if( !CheckSpellCooldown(caster, spell, spellData) || !CheckSpellTargetMode(caster, spellData)
         || (caster.Character.states[EntityStateType.Stun].InEffect && (spellData.flags & SpellFlags.CastableWhileStunned) != SpellFlags.CastableWhileStunned))
            return false;
        #endregion

        #region If spell is instant or casted
        if (CheckSpellCastType(caster, spell, spellData))
        {
            if (spellData.spellCastType == SpellCastType.Casted)
            {
                caster.Animator.SetBool("Casting", true);

                skillUseEvent = MonoBehaviour.Instantiate(Resources.Load("Prefabs/UI/SkillUseEvent")) as GameObject;
                skillUseEvent.GetComponent<SkillUseUIEvent>().Initialize(spellData, caster, PlayerInterface);
                return true;
            }
        }
        else
            return false;
        #endregion

        if (spellData.effectName != null)
        {
            GameObject effect = Instantiate(Resources.Load("Prefabs/Spells/" + spellData.effectName), new Vector3(caster.transform.position.x,
                caster.transform.position.y, caster.transform.position.z), caster.transform.rotation) as GameObject;
            effect.transform.parent = Effects.transform;
        }
        if (spellData.castSoundName != null)
        {
            AudioClip audio = Resources.Load<AudioClip>(spellData.castSoundName);
            AudioSource.PlayClipAtPoint(audio, caster.transform.position);
        }

        skillUseEvent = Instantiate(Resources.Load("Prefabs/UI/SkillUseEvent")) as GameObject;
        skillUseEvent.GetComponent<SkillUseUIEvent>().Initialize(spellData, caster, PlayerInterface);

        ApplySpell(caster, target, spell, spellData);
        return true;
    }

    public bool SpellCastFinished(SpellCast spellCast)
    {
        SpellData spellData = SpellLibrary.GetSpell(spellCast.spell.id);
        Spell spell = spellCast.spell; Unit caster = spellCast.caster;
        Unit target = spellCast.target;

        if (spellData.effectName != null)
        {
            GameObject effect = Instantiate(Resources.Load("Prefabs/Spells/" + spellData.effectName), new Vector3(caster.transform.position.x,
                caster.transform.position.y, caster.transform.position.z), caster.transform.rotation) as GameObject;
            effect.transform.parent = Effects.transform;
        }
        if (spellData.castSoundName != null)
        {
            AudioClip audio = Resources.Load<AudioClip>(spellData.castSoundName);
            AudioSource.PlayClipAtPoint(audio, caster.transform.position);
        }


        if (spellData.spellProjectileName != null)
        {
            GameObject effect = Instantiate(Resources.Load("Prefabs/Spells/" + spellData.spellProjectileName), caster.castPoint != null ? caster.castPoint.position :
                new Vector3(caster.transform.position.x, caster.transform.position.y + caster.GetComponent<Collider2D>().bounds.size.y * 3 / 4, caster.transform.position.z),
                Quaternion.identity) as GameObject;
            TargetedProjectile projectile = effect.GetComponent<TargetedProjectile>();
            projectile.Initialize(this, caster, target, spell);
            effect.transform.parent = Effects.transform;
            caster.Animator.SetTrigger("InstantAttack");
            caster.Animator.SetBool("Casting", false);
            return true;
        }

        if (spellData.hitSoundName != null)
        {
            AudioClip audio = Resources.Load<AudioClip>(spellData.hitSoundName);
            AudioSource.PlayClipAtPoint(audio, caster.transform.position);
        }

        ApplySpell(caster, target, spell, spellData);
        caster.Animator.SetTrigger("InstantAttack");
        caster.Animator.SetBool("Casting", false);
        return true;
    }

    public bool SpellProjectileHit(Unit caster, Unit target, Spell spell)
    {
        SpellData spellData = SpellLibrary.GetSpell(spell.id);

        if (spellData.effectName != null)
        {
            GameObject effect = Instantiate(Resources.Load("Prefabs/Spells/" + spellData.effectName), new Vector3(caster.transform.position.x,
                caster.transform.position.y, caster.transform.position.z), caster.transform.rotation) as GameObject;
            effect.transform.parent = Effects.transform;
        }
        if (spellData.hitSoundName != null)
        {
            AudioClip audio = Resources.Load<AudioClip>(spellData.hitSoundName);
            AudioSource.PlayClipAtPoint(audio, caster.transform.position);
        }
        ApplySpell(caster, target, spell, spellData);
        return true;
    }


    public void Spawn(GameObject enemy, int id, GameObject spawner)
    {
        GameObject newEnemy = Instantiate(enemy, spawner.transform.position, spawner.transform.rotation) as GameObject;
        newEnemy.transform.parent = GameObject.Find("NPCs").transform;
        Unit enemyUnit = newEnemy.GetComponent<Unit>();
        enemyUnit.id = id;
        if (enemyUnit.character.className == "Warrior")
        {
            enemyUnit.character.Spells.AddSpell(new Spell(100, "Charge", new Cooldown(SpellLibrary.GetSpell(100).baseCooldown)));
            enemyUnit.character.Spells.AddSpell(new Spell(101, "Mortal Strike", new Cooldown(SpellLibrary.GetSpell(101).baseCooldown)));
        }
        arenaUnits.Add(enemyUnit);
    }

    public bool GetNearestTarget(Unit targeter, float distance)
    {
        if (targeter.character.PreviousTargets.Count > 10)
            targeter.character.PreviousTargets.Clear();

        for (int i = 0; i < arenaUnits.Count; i++)
        {
            if (!targeter.character.PreviousTargets.Contains(arenaUnits[i].Id))
            {
                if (arenaUnits[i].id == targeter.id || arenaUnits[i].IsDead
                    || arenaUnits[i].Character.states[EntityStateType.Invisible].InEffect
                    || Vector2.Distance(targeter.transform.position, arenaUnits[i].transform.position) > distance)
                    continue;
                targeter.character.PreviousTargets.Add(arenaUnits[i].id);
                targeter.character.target = arenaUnits[i];
                return true;
            }
        }
        if (!targeter.character.PreviousTargets.Contains(PlayerUnit.id))
        {
            if (!(PlayerUnit.id == targeter.id
                || PlayerUnit.IsDead
                || PlayerUnit.Character.states[EntityStateType.Invisible].InEffect 
                || Vector2.Distance(targeter.transform.position, PlayerUnit.transform.position) > distance))
            {
                targeter.character.PreviousTargets.Add(PlayerUnit.id);
                targeter.character.target = PlayerUnit;
                return true;
            }
        }
        targeter.character.PreviousTargets.Clear();
        targeter.character.target = null;
        return false;
    }

    public static bool GetTargetsForPlayer(Unit targeter, float distance)
    {
        if (targeter.character.PreviousTargets.Count > 10)
            targeter.character.PreviousTargets.Clear();

        for (int i = 0; i < ArenaUnits.Count; i++)
        {
            if (!targeter.character.PreviousTargets.Contains(ArenaUnits[i].Id))
            {
                if (ArenaUnits[i].id == targeter.id || ArenaUnits[i].IsDead
                    || ArenaUnits[i].Character.states[EntityStateType.Invisible].InEffect
                    || Vector2.Distance(targeter.transform.position, ArenaUnits[i].transform.position) > distance)
                    continue;
                targeter.character.PreviousTargets.Add(ArenaUnits[i].id);
                PlayerInterface.CheckTargetSelection(ArenaUnits[i]);
                return true;
            }
        }
        /*if (!targeter.character.PreviousTargets.Contains(PlayerUnit.id))
        {
            if (!(PlayerUnit.id == targeter.id
                || PlayerUnit.IsDead
                || PlayerUnit.Character.states[EntityStateType.Invisible].InEffect
                || Vector2.Distance(targeter.transform.position, PlayerUnit.transform.position) > distance))
            {
                targeter.character.PreviousTargets.Add(PlayerUnit.id);
                PlayerInterface.CheckTargetSelection(PlayerUnit);
                return true;
            }
        }*/
        targeter.character.PreviousTargets.Clear();
        PlayerInterface.CheckTargetSelection(null);
        return false;
    }
}