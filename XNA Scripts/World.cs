using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;

using BasicRpgEngine.Characters;
using BasicRpgEngine.Spells;
using BasicRpgEngine.Physics;
using BasicRpgEngine.Graphics;

namespace BasicRpgEngine
{
    public class World
    {
        public Rectangle ScreenRectangle
        { get; private set; }
        public GameMap MainMap
        { get; private set; }
        public NetworkPlayerInterface PlayerUiRef
        { get; set; }
        public Game GameRef
        { get; private set; }

        public List<NetworkPlayer> Characters
        { get; private set; }
        public List<Creature> Creatures
        { get; private set; }
        public List<Creature>[,] PartitionedCreatures
        { get; private set; }
        public List<TargetingProjectile> TargetingProjectiles
        { get; set; }
        public List<StaticProjectile> StaticProjectiles
        { get; set; }
        public Dictionary<string, string> OnCastSoundManager
        { get; private set; }
        public Dictionary<string, string> OnHitSoundManager
        { get; private set; }
        public Dictionary<byte, SpellData> SpellManager
        { get; private set; }
        public Dictionary<byte, string> SpellNames
        { get; private set; }
        public Dictionary<int, SpellModifier> SpellModifierManager
        { get; private set; }
        public Dictionary<string, ProjectileBinding> ProjectileBindingManager
        { get; private set; }
        public Dictionary<string, AnimatedSprite> SpriteCharacterManager
        { get; private set; }
        public Dictionary<string, AnimatedSprite> SpriteSpellManager
        { get; private set; }
        public Dictionary<string, AnimatedSprite> SpriteSpellHitManager
        { get; private set; }

        public World(Rectangle screenRectangle, GameMap mainMap, Game gameRef)
        {
            #region Base Fields
            GameRef = gameRef;
            MainMap = mainMap;
            ScreenRectangle = screenRectangle;
            Characters = new List<NetworkPlayer>();
            Creatures = new List<Creature>();
            TargetingProjectiles = new List<TargetingProjectile>();
            StaticProjectiles = new List<StaticProjectile>();
            SpellManager = new Dictionary<byte, SpellData>();
            ProjectileBindingManager = new Dictionary<string, ProjectileBinding>();
            SpellNames = new Dictionary<byte, string>();
            SpriteSpellManager = new Dictionary<string, AnimatedSprite>();
            SpriteSpellHitManager = new Dictionary<string, AnimatedSprite>();
            SpriteCharacterManager = new Dictionary<string, AnimatedSprite>();
            SpellModifierManager = new Dictionary<int, SpellModifier>();
            PartitionedCreatures = new List<Creature>[(MainMap.MapHeight / 200 + 1), (MainMap.MapWidth / 200 + 1)];
            for (int i = 0; i < PartitionedCreatures.GetLength(0); i++)
                for (int j = 0; j < PartitionedCreatures.GetLength(1); j++)
                    PartitionedCreatures[i, j] = new List<Creature>();
            #endregion

            #region SpellModifiers
            #region Id = 1 : Multiply Damage 4x if Freezed 
            SpellModifier spellModifier = new SpellInfoModifier(1,
                delegate(ITargetable caster, ITargetable target) { return target.Character.Entity.IsFreezed; },
                SpellModifierType.DamageMultiplier, 4);
            SpellModifierManager.Add(spellModifier.Id, spellModifier);
            #endregion
            #region Id = 2 : Multiply Crit Chance 2x if Freezed
            spellModifier = new SpellInfoModifier(2,
                delegate(ITargetable caster, ITargetable target) { return target.Character.Entity.IsFreezed; },
                SpellModifierType.CriticalChanceMultiplier, 2);
            SpellModifierManager.Add(spellModifier.Id, spellModifier);
            #endregion
            #region Id = 3 : Add Crit Chance 50% if Freezed
            spellModifier = new SpellInfoModifier(3,
                delegate(ITargetable caster, ITargetable target) { return target.Character.Entity.IsFreezed; },
                SpellModifierType.CriticalChanceAddition, 50);
            SpellModifierManager.Add(spellModifier.Id, spellModifier);
            #endregion
            #region Id = 4 : Insanity
            spellModifier = new SpellInfoModifier(4,
                delegate(ITargetable caster, ITargetable target) { return target.Character.Entity.Buffs.Find(item => item.Id == 72 && item.TimeLeft.TotalSeconds <= 5) == null; },
                SpellModifierType.CastFailed, 0);
            SpellModifierManager.Add(spellModifier.Id, spellModifier);
            #endregion
            #endregion

            #region Character Animations
            #region Sheep Animation
            Animation animation;
            Dictionary<AnimationKey, Animation> animations = new Dictionary<AnimationKey, Animation>();

            animation = new Animation(10, 153, 180, 153, 0, 30, false, 0.5f, 1);
            animations.Add(AnimationKey.Right, animation);
            animation = new Animation(10, 153, 180, 153, 0, 30, true, 0.5f, 1);
            animations.Add(AnimationKey.Left, animation);

            animation = new Animation(1, 153, 180, 0, 0, 1, false, 0.5f, 1);
            animations.Add(AnimationKey.StandRight, animation);
            animation = new Animation(1, 153, 180, 0, 0, 1, true, 0.5f, 1);
            animations.Add(AnimationKey.StandLeft, animation);

            animation = new Animation(1, 153, 180, 0, 0, 1, false, 0.5f, 1);
            animations.Add(AnimationKey.CastingRight, animation);
            animation = new Animation(1, 153, 180, 0, 0, 1, true, 0.5f, 1);
            animations.Add(AnimationKey.CastingLeft, animation);

            animation = new Animation(1, 153, 180, 0, 0, 1, false, 0.5f, 1);
            animations.Add(AnimationKey.CastRight, animation);
            animation = new Animation(1, 153, 180, 0, 0, 1, true, 0.5f, 1);
            animations.Add(AnimationKey.CastLeft, animation);

            animation = new Animation(1, 153, 180, 0, 0, 1, false, 0.5f, 1);
            animations.Add(AnimationKey.RunCastingRight, animation);
            animation = new Animation(1, 153, 180, 0, 0, 1, true, 0.5f, 1);
            animations.Add(AnimationKey.RunCastingLeft, animation);

            animation = new Animation(1, 153, 180, 0, 0, 1, false, 0.5f, 1);
            animations.Add(AnimationKey.RunCastRight, animation);
            animation = new Animation(1, 153, 180, 0, 0, 1, true, 0.5f, 1);
            animations.Add(AnimationKey.RunCastLeft, animation);

            Texture2D sheep = GameRef.Content.Load<Texture2D>(@"CharacterSprites\Sheep");

            AnimatedSprite sprite = new AnimatedSprite(sheep, animations, 0.9f);
            SpriteCharacterManager.Add("Sheep", sprite);
            #endregion

            #region Shadow Form Animation
            animations = new Dictionary<AnimationKey, Animation>();

            animations = new Dictionary<AnimationKey, Animation>();
            animation = new Animation(10, 150, 180, 150, 0, 20, false, 0.5f, 1);
            animations.Add(AnimationKey.Right, animation);
            animation = new Animation(10, 150, 180, 150, 0, 20, true, 0.5f, 1);
            animations.Add(AnimationKey.Left, animation);

            animation = new Animation(1, 150, 180, 0, 0, 1, false, 0.5f, 1);
            animations.Add(AnimationKey.StandRight, animation);
            animation = new Animation(1, 150, 180, 0, 0, 1, true, 0.5f, 1);
            animations.Add(AnimationKey.StandLeft, animation);

            animation = new Animation(10, 160, 180, 0, 180, 25, false, 0.5f, 1);
            animations.Add(AnimationKey.CastRight, animation);
            animation = new Animation(10, 160, 180, 0, 180, 25, true, 0.5f, 1);
            animations.Add(AnimationKey.CastLeft, animation);

            animation = new Animation(10, 160, 180, 0, 360, 20, false, 0.5f, 1);
            animations.Add(AnimationKey.RunCastRight, animation);
            animation = new Animation(10, 160, 180, 0, 360, 20, true, 0.5f, 1);
            animations.Add(AnimationKey.RunCastLeft, animation);

            animation = new Animation(10, 160, 180, 0, 540, 25, false, 0.5f, 1);
            animations.Add(AnimationKey.CastingRight, animation);
            animation = new Animation(10, 160, 180, 0, 540, 25, true, 0.5f, 1);
            animations.Add(AnimationKey.CastingLeft, animation);

            animation = new Animation(10, 160, 180, 0, 720, 20, false, 0.5f, 1);
            animations.Add(AnimationKey.RunCastingRight, animation);
            animation = new Animation(10, 160, 180, 0, 720, 20, true, 0.5f, 1);
            animations.Add(AnimationKey.RunCastingLeft, animation);

            Texture2D shadow = GameRef.Content.Load<Texture2D>(@"CharacterSprites\ShadowFormCaster");

            sprite = new AnimatedSprite(shadow, animations, 0.9f);
            SpriteCharacterManager.Add("Shadow Form", sprite);
            #endregion
            #endregion

            #region Spell Animations
            #region Frost Bolt Test Animation
            animations = new Dictionary<AnimationKey, Animation>();
            animation = new Animation(5, 120, 62, 0, 0, 30, true, 0.5f, 1);
            animations.Add(AnimationKey.Left, animation);
            Texture2D frostbolt = GameRef.Content.Load<Texture2D>(@"Effects\frostball");
            sprite = new AnimatedSprite(frostbolt, animations, 1.5f);
            SpriteSpellManager.Add("Frost Bolt", sprite);
            #endregion

            #region Ice Lance Test Animation
            animations = new Dictionary<AnimationKey, Animation>();
            animation = new Animation(6, 200, 100, 0, 0, 10, true, 0.5f, 2);
            animations.Add(AnimationKey.Left, animation);
            Texture2D icelance = GameRef.Content.Load<Texture2D>(@"Effects\icelance");
            sprite = new AnimatedSprite(icelance, animations, 0.6f);
            SpriteSpellManager.Add("Ice Lance", sprite);
            #endregion

            #region Pyroblast Test Animation
            animations = new Dictionary<AnimationKey, Animation>();
            animation = new Animation(6, 230, 220, 0, 0, 20, true, 0.5f, 4);
            animations.Add(AnimationKey.Left, animation);
            Texture2D pyroblast = GameRef.Content.Load<Texture2D>(@"Effects\pyroblast");
            sprite = new AnimatedSprite(pyroblast, animations, 1f);
            SpriteSpellManager.Add("Pyroblast", sprite);
            #endregion

            #region Blast Wave Test Animation
            animations = new Dictionary<AnimationKey, Animation>();
            animation = new Animation(3, 600, 220, 0, 0, 20, false, 0.5f, 5);
            animations.Add(AnimationKey.Left, animation);
            Texture2D blastWave = GameRef.Content.Load<Texture2D>(@"Effects\firenova");
            sprite = new AnimatedSprite(blastWave, animations, 1.5f);
            SpriteSpellManager.Add("Blast Wave", sprite);
            #endregion

            #region Frost Nova Test Animation
            animations = new Dictionary<AnimationKey, Animation>();
            animation = new Animation(3, 600, 180, 0, 0, 20, false, 0.5f, 5);
            animations.Add(AnimationKey.Left, animation);
            Texture2D frostnova = GameRef.Content.Load<Texture2D>(@"Effects\frostnova");
            sprite = new AnimatedSprite(frostnova, animations, 1.5f);
            SpriteSpellManager.Add("Frost Nova", sprite);
            #endregion

            #region Frost Nova Hit Test Animation
            animations = new Dictionary<AnimationKey, Animation>();
            animation = new Animation(5, 400, 170, 0, 0, 20, false, 0.5f, 2);
            animations.Add(AnimationKey.Left, animation);
            animation = new Animation(1, 8, 9, 0, 0, 0, false, 0, 1);
            animations.Add(AnimationKey.Right, animation);
            Texture2D frostnovahit = GameRef.Content.Load<Texture2D>(@"Effects\frostnovaimpact");
            sprite = new AnimatedSprite(frostnovahit, animations, 1.5f);
            SpriteSpellHitManager.Add("Frost Nova", sprite);
            ProjectileBindingManager.Add("Frost Nova", ProjectileBinding.CenterToBottom);
            SpriteSpellHitManager.Add("Frostjaw", sprite);
            ProjectileBindingManager.Add("Frostjaw", ProjectileBinding.CenterToBottom);
            #endregion

            #region Deep Freeze Hit Test Animation
            animations = new Dictionary<AnimationKey, Animation>();
            animation = new Animation(8, 180, 250, 0, 0, 20, false, 0.5f, 1);
            animations.Add(AnimationKey.Left, animation);
            animation = new Animation(1, 6, 7, 0, 0, 0, false, 0, 1);
            animations.Add(AnimationKey.Right, animation);
            Texture2D deepfreeze = GameRef.Content.Load<Texture2D>(@"Effects\deepfreeze");
            sprite = new AnimatedSprite(deepfreeze, animations, 0.9f);
            SpriteSpellHitManager.Add("Deep Freeze", sprite);
            ProjectileBindingManager.Add("Deep Freeze", ProjectileBinding.BottomToBottom);
            SpriteSpellHitManager.Add("Ice Block", sprite);
            ProjectileBindingManager.Add("Ice Block", ProjectileBinding.BottomToBottom);
            #endregion

            #region Ice Barrier Hit Test Animation
            animations = new Dictionary<AnimationKey, Animation>();
            animation = new Animation(7, 150, 230, 0, 0, 15, false, 0.5f, 2);
            animations.Add(AnimationKey.Left, animation);
            animation = new Animation(1, 6, 13, 0, 0, 0, false, 0, 1);
            animations.Add(AnimationKey.Right, animation);
            Texture2D icebarrier = GameRef.Content.Load<Texture2D>(@"Effects\icebarrier");
            sprite = new AnimatedSprite(icebarrier, animations, 0.9f);
            SpriteSpellHitManager.Add("Ice Barrier", sprite);
            ProjectileBindingManager.Add("Ice Barrier", ProjectileBinding.CenterToCenter);
            #endregion
            #endregion

            #region Sound Load
            OnCastSoundManager = new Dictionary<string, string>();
            OnCastSoundManager.Add("Deep Freeze", @"Sound\Spells\Mage\DeepFreeze");
            OnCastSoundManager.Add("Invisibility", @"Sound\Spells\Mage\Invisibility_Impact_Chest");
            OnCastSoundManager.Add("Blazing Speed", @"Sound\Spells\Mage\BlazingSpeed");
            OnCastSoundManager.Add("Charge", @"Sound\Spells\Mage\demo_charge_windup2");
            OnCastSoundManager.Add("Throwdown", @"Sound\Spells\Warrior\KnockDown");
            OnCastSoundManager.Add("Shockwave", @"Sound\Spells\Warrior\BoulderImpact1");
            OnCastSoundManager.Add("Frost Nova", @"Sound\Spells\Mage\FrostNova");
            OnCastSoundManager.Add("Living Bomb", @"Sound\Spells\Mage\LivingBombCast");
            OnCastSoundManager.Add("Fire Blast", @"Sound\Spells\Mage\FireBlast");
            OnCastSoundManager.Add("Pyromanser", @"Sound\Spells\Mage\Flamestrike");
            OnCastSoundManager.Add("Blast Wave", @"Sound\Spells\Mage\FireNova");
            OnCastSoundManager.Add("Ice Barrier", @"Sound\Spells\Mage\IceBarrirerImpact");
            OnCastSoundManager.Add("Cone of Cold", @"Sound\Spells\Mage\ConeofColdHand");
            OnCastSoundManager.Add("Icy Veins", @"Sound\Spells\Mage\IceVeins");
            OnCastSoundManager.Add("Ice Block", @"Sound\Spells\Mage\IceBarrirerState");
            OnCastSoundManager.Add("Blink", @"Sound\Spells\Mage\Teleport");
            OnCastSoundManager.Add("Pyroblast!", @"Sound\Spells\Mage\LavaBurstImpact1");
            OnCastSoundManager.Add("Counter Spell", @"Sound\Spells\Mage\CounterSpell");
            OnCastSoundManager.Add("Bleed", @"Sound\Spells\Warrior\RendTarget");
            OnCastSoundManager.Add("Battle Shout", @"Sound\Spells\Warrior\BattleShoutTarget");
            OnCastSoundManager.Add("Thunder Clap", @"Sound\Spells\Warrior\ThunderClap");
            OnCastSoundManager.Add("Hamstring", @"Sound\Spells\Warrior\MaimImpact");
            OnCastSoundManager.Add("Mortal Strike", @"Sound\Spells\Warrior\m2hSwordHitFleshCrit");
            OnCastSoundManager.Add("Shield Wall", @"Sound\Spells\Warrior\ShieldWallTarget");
            OnCastSoundManager.Add("Cleave", @"Sound\Spells\Warrior\CleaveTarget");
            OnCastSoundManager.Add("Avatar", @"Sound\Spells\Warrior\Cower");
            OnCastSoundManager.Add("Psychic Scream", @"Sound\Spells\Priest\Fear");
            OnHitSoundManager = new Dictionary<string, string>();
            OnHitSoundManager.Add("Ice Lance", @"Sound\Spells\Mage\IceLanceImpact");
            OnHitSoundManager.Add("Frost Fire Bolt", @"Sound\Spells\Mage\IceImpact");
            OnHitSoundManager.Add("Frostjaw", @"Sound\Spells\Mage\IceImpact");
            OnHitSoundManager.Add("Frost Bolt", @"Sound\Spells\Mage\IceImpact");
            OnHitSoundManager.Add("Polymorph", @"Sound\Spells\Mage\PolyMorphTarget");
            OnHitSoundManager.Add("Mass Polymorph", @"Sound\Spells\Mage\PolyMorphTarget");
            OnHitSoundManager.Add("Living Bomb Area", @"Sound\Spells\Mage\LivingBombArea");
            OnHitSoundManager.Add("Combustion Area", @"Sound\Spells\Mage\LivingBombArea");
            OnHitSoundManager.Add("Pyroblast", @"Sound\Spells\Mage\LavaBurstImpact1");
            OnHitSoundManager.Add("Scorch", @"Sound\Spells\Mage\FireBlast");
            #endregion

            #region Spells
            AoeEffect aoeEffect;
            DamageEffect damageEffect;
            KnockBackEffect knockBackEffect;
            SpellData newSpellData;
            SpellData newScriptSpellData;
            SpellData newScriptSpellData1;
            Buff buff;
            Buff scriptBuff;

            //Mage
            #region Cone of Cold : SpellId = 1 / BuffId = 1
            //Spell Data
            newSpellData = new SpellData();
            newSpellData.ID = 1;
            newSpellData.Range = 0;
            newSpellData.ActivationCost = 0;
            newSpellData.Name = "Cone of Cold";
            newSpellData.SpellCastType = SpellCastType.Instant;
            newSpellData.BaseCastTime = 0;
            newSpellData.SpellTargetMode = SpellTargetMode.Self;
            newSpellData.SpellGlobal = SpellGlobalCooldownMode.Normal;
            newSpellData.SpellCooldown = new Cooldown(8);
            newSpellData.IsHarmful = true;
            //Enemy Area Cone 300-Radius Frost Damage 20-30 Effect
            damageEffect = new DamageEffect(DamageType.Frost, 20, 30, 0);
            newSpellData.Effects.Add(new AoeEffect(damageEffect, 300, AoeMode.Cone));
            //Buff
            buff = new Buff(1, "Cone of Cold", BuffType.Debuff, MagicType.Magic, AoeMode.Cone, 300, 8, 0, 0, false, false, false);
            //Enemy Area Cone 300-Radius Snare 60 % for 8 sec Aura
            buff.Auras.Add(new AuraModifier(AuraType.Speed, 0, 0.4f, 8));
            //Add New Spell
            newSpellData.Buff = (Buff)buff.Clone();
            SpellManager.Add(newSpellData.ID, newSpellData);
            SpellNames.Add(newSpellData.ID, newSpellData.Name);
            #endregion
            #region Ice Barrier : SpellId = 2 / BuffId = 2
            //Spell Data
            newSpellData = new SpellData();
            newSpellData.ID = 2;
            newSpellData.Range = 0;
            newSpellData.ActivationCost = 0;
            newSpellData.Name = "Ice Barrier";
            newSpellData.SpellCastType = SpellCastType.Instant;
            newSpellData.BaseCastTime = 0;
            newSpellData.SpellTargetMode = SpellTargetMode.Self;
            newSpellData.SpellGlobal = SpellGlobalCooldownMode.Normal;
            newSpellData.SpellCooldown = new Cooldown(20f);
            newSpellData.IsHarmful = false;
            //Buff
            buff = new Buff(2, "Ice Barrier", BuffType.Buff, MagicType.Magic, AoeMode.None, 0, 60, 0, 0, false, false, false, true);
            //Self Single Absorb Shield 320 Damage for 60 sec Aura
            buff.Auras.Add(new AuraAbsorbDamage(true, true, true, true, true, true, true, 320, 60));
            //Add New Spell
            newSpellData.Buff = (Buff)buff.Clone();
            SpellManager.Add(newSpellData.ID, newSpellData);
            SpellNames.Add(newSpellData.ID, newSpellData.Name);
            #endregion
            #region Blink : SpellId = 3 / BuffId = None
            //Spell Data
            newSpellData = new SpellData();
            newSpellData.ID = 3;
            newSpellData.Range = 0;
            newSpellData.ActivationCost = 0;
            newSpellData.Name = "Blink";
            newSpellData.SpellCastType = SpellCastType.Instant;
            newSpellData.BaseCastTime = 0;
            newSpellData.SpellTargetMode = SpellTargetMode.Self;
            newSpellData.SpellGlobal = SpellGlobalCooldownMode.NoCooldown;
            newSpellData.SpellCooldown = new Cooldown(12f);
            newSpellData.CastableInStun = true;
            newSpellData.CastableInControl = true;
            newSpellData.IsHarmful = false;
            //Self Single Teleport Forward 500-Range
            newSpellData.Effects.Add(new BuffRemovalEffect(item => item.Auras.Find(aura => aura.ModifierType == AuraType.Stun) != null));
            newSpellData.Effects.Add(new TeleportDirectEffect(500, 0));
            //Add New Spell
            newSpellData.Buff = null;
            SpellManager.Add(newSpellData.ID, newSpellData);
            SpellNames.Add(newSpellData.ID, newSpellData.Name);
            #endregion
            #region Icy Veins : SpellId = 4 / BuffId = 3
            //Spell Data
            newSpellData = new SpellData();
            newSpellData.ID = 4;
            newSpellData.Range = 0;
            newSpellData.ActivationCost = 0;
            newSpellData.Name = "Icy Veins";
            newSpellData.SpellCastType = SpellCastType.Instant;
            newSpellData.BaseCastTime = 0;
            newSpellData.SpellTargetMode = SpellTargetMode.Self;
            newSpellData.SpellGlobal = SpellGlobalCooldownMode.NoCooldown;
            newSpellData.SpellCooldown = new Cooldown(30f);
            newSpellData.IsHarmful = false;
            //Buff
            buff = new Buff(3, "Icy Veins", BuffType.Buff, MagicType.Magic, AoeMode.None, 0, 15, 0, 0, false, false, false);
            //Self Single Haste Rating Increase 100% for 15 sec
            buff.Auras.Add(new AuraModifier(AuraType.HasteRating, 100f, 0, 15));
            //Add New Spell
            newSpellData.Buff = (Buff)buff.Clone();
            SpellManager.Add(newSpellData.ID, newSpellData);
            SpellNames.Add(newSpellData.ID, newSpellData.Name);
            #endregion
            #region Ice Lance : SpellId = 5 / BuffId = None
            //Spell Data
            newSpellData = new SpellData();
            newSpellData.ID = 5;
            newSpellData.Range = 800;
            newSpellData.ActivationCost = 0;
            newSpellData.Name = "Ice Lance";
            newSpellData.SpellCastType = SpellCastType.Instant;
            newSpellData.BaseCastTime = 0;
            newSpellData.SpellTargetMode = SpellTargetMode.Target;
            newSpellData.SpellGlobal = SpellGlobalCooldownMode.Normal;
            newSpellData.SpellCooldown = new Cooldown(0);
            newSpellData.IsHarmful = true;
            //Enemy Single Target Frost Damage 25-25
            damageEffect = new DamageEffect(DamageType.Frost, 25, 25, 0);
            newSpellData.Effects.Add(damageEffect);
            //Add New Spell
            newSpellData.Buff = null;
            SpellManager.Add(newSpellData.ID, newSpellData);
            SpellNames.Add(newSpellData.ID, newSpellData.Name);
            #endregion
            #region Frostjaw : SpellId = 6 / BuffId = 4
            //Spell Data
            newSpellData = new SpellData();
            newSpellData.ID = 6;
            newSpellData.Range = 800;
            newSpellData.ActivationCost = 0;
            newSpellData.Name = "Frostjaw";
            newSpellData.SpellCastType = SpellCastType.Casted;
            newSpellData.BaseCastTime = 1f;
            newSpellData.SpellTargetMode = SpellTargetMode.Target;
            newSpellData.SpellGlobal = SpellGlobalCooldownMode.NoCooldown;
            newSpellData.SpellCooldown = new Cooldown(35f);
            newSpellData.IsHarmful = true;
            //Buff
            buff = new Buff(4, "Frostjaw", BuffType.Debuff, MagicType.Magic, AoeMode.None, 0, 2, 0, 0, false, false, false);
            //Enemy Single Target Silence 2 sec
            buff.Auras.Add(new AuraModifier(AuraType.Silence, 0, 0, 2));
            //Enemy Single Target Root 2 sec
            buff.Auras.Add(new AuraModifier(AuraType.Root, 0, 0, 2));
            //Enemy Single Target Freeze 2 sec
            buff.Auras.Add(new AuraModifier(AuraType.Freeze, 0, 0, 2));
            //Add New Spell
            newSpellData.Buff = (Buff)buff.Clone();
            SpellManager.Add(newSpellData.ID, newSpellData);
            SpellNames.Add(newSpellData.ID, newSpellData.Name);
            #endregion
            #region Counter Spell : SpellId = 7 / BuffId = 5
            //Spell Data
            newSpellData = new SpellData();
            newSpellData.ID = 7;
            newSpellData.Range = 800;
            newSpellData.ActivationCost = 0;
            newSpellData.Name = "Counter Spell";
            newSpellData.SpellCastType = SpellCastType.Instant;
            newSpellData.BaseCastTime = 0;
            newSpellData.SpellTargetMode = SpellTargetMode.Target;
            newSpellData.SpellGlobal = SpellGlobalCooldownMode.NoCooldown;
            newSpellData.SpellCooldown = new Cooldown(20f);
            newSpellData.IsHarmful = true;
            //Buff
            buff = new Buff(5, "Counter Spell", BuffType.Debuff, MagicType.Magic, AoeMode.None, 0, 4, 0, 0, false, false, false);
            //Enemy Single Target Silence 4 sec
            buff.Auras.Add(new AuraModifier(AuraType.Silence, 0, 0, 4));
            //Add Spell
            newSpellData.Buff = (Buff)buff.Clone();
            SpellManager.Add(newSpellData.ID, newSpellData);
            SpellNames.Add(newSpellData.ID, newSpellData.Name);
            #endregion
            #region Blast Wave : SpellId = 8 / BuffId = None
            //Spell Data
            newSpellData = new SpellData();
            newSpellData.ID = 8;
            newSpellData.Range = 0;
            newSpellData.ActivationCost = 0;
            newSpellData.Name = "Blast Wave";
            newSpellData.SpellCastType = SpellCastType.Instant;
            newSpellData.BaseCastTime = 0;
            newSpellData.SpellTargetMode = SpellTargetMode.Self;
            newSpellData.SpellGlobal = SpellGlobalCooldownMode.Normal;
            newSpellData.SpellCooldown = new Cooldown(20f);
            newSpellData.IsHarmful = true;
            //Enemy Area 400-Circle Fire Damage 50-60 Effect
            damageEffect = new DamageEffect(DamageType.Fire, 50, 60, 0);
            aoeEffect = new AoeEffect(damageEffect, 400, AoeMode.Self);
            newSpellData.Effects.Add(aoeEffect);
            //Enemy Area 400-Circle KnockBack 10-6 Effect
            knockBackEffect = new KnockBackEffect(10, 6);
            aoeEffect = new AoeEffect(knockBackEffect, 400, AoeMode.Self);
            newSpellData.Effects.Add(aoeEffect);
            //Add New Spell
            newSpellData.Buff = null;
            SpellManager.Add(newSpellData.ID, newSpellData);
            SpellNames.Add(newSpellData.ID, newSpellData.Name);
            #endregion
            #region Fire Blast : SpellId = 9 / BuffId = None
            //New Spell
            newSpellData = new SpellData();
            newSpellData.ID = 9;
            newSpellData.Range = 600;
            newSpellData.ActivationCost = 0;
            newSpellData.Name = "Fire Blast";
            newSpellData.SpellCastType = SpellCastType.Instant;
            newSpellData.BaseCastTime = 0;
            newSpellData.SpellTargetMode = SpellTargetMode.Target;
            newSpellData.SpellGlobal = SpellGlobalCooldownMode.Normal;
            newSpellData.SpellCooldown = new Cooldown(3);
            newSpellData.IsHarmful = true;
            //Enemy Single Target Fire Damage 40-50 Effect
            damageEffect = new DamageEffect(DamageType.Fire, 40, 50, 0);
            newSpellData.Effects.Add(damageEffect);
            //Add New Spell
            newSpellData.Buff = null;
            SpellManager.Add(newSpellData.ID, newSpellData);
            SpellNames.Add(newSpellData.ID, newSpellData.Name);
            #endregion
            #region Pyroblast : SpellId = 10 / BuffId = 6
            //Spell Data
            newSpellData = new SpellData();
            newSpellData.ID = 10;
            newSpellData.Range = 1200;
            newSpellData.ActivationCost = 0;
            newSpellData.Name = "Pyroblast";
            newSpellData.SpellCastType = SpellCastType.Instant;
            newSpellData.BaseCastTime = 3;
            newSpellData.SpellTargetMode = SpellTargetMode.Target;
            newSpellData.SpellGlobal = SpellGlobalCooldownMode.NoCooldown;
            newSpellData.SpellCooldown = new Cooldown(6f);
            newSpellData.IsHarmful = true;
            //Enemy Single Target Fire Damage 120-150 Effect
            damageEffect = new DamageEffect(DamageType.Fire, 120, 150, 0);
            newSpellData.Effects.Add(damageEffect);
            //Buff
            buff = new Buff(6, "Pyroblast", BuffType.Debuff, MagicType.Magic, AoeMode.None, 0, 4, 0, 0, false, false, false);
            //Single Target Dot 120 Damage for 9 sec with 3 sec tick
            buff.Auras.Add(new AuraPeriodicDamage(DamageType.Fire, 120, 4, 0, 4));
            //Add Spell
            newSpellData.Buff = (Buff)buff.Clone();
            SpellManager.Add(newSpellData.ID, newSpellData);
            SpellNames.Add(newSpellData.ID, newSpellData.Name);
            #endregion
            #region Scorch : SpellId = 11 / BuffId = None
            //Spell Data
            newSpellData = new SpellData();
            newSpellData.ID = 11;
            newSpellData.Range = 1000;
            newSpellData.ActivationCost = 0;
            newSpellData.Name = "Scorch";
            newSpellData.SpellCastType = SpellCastType.Casted;
            newSpellData.BaseCastTime = 1f;
            newSpellData.SpellTargetMode = SpellTargetMode.Target;
            newSpellData.SpellGlobal = SpellGlobalCooldownMode.NoCooldown;
            newSpellData.SpellCooldown = new Cooldown(0);
            newSpellData.IsHarmful = true;
            //Enemy Single Target Fire Damage 35-45 Effect
            damageEffect = new DamageEffect(DamageType.Fire, 35, 45, 0);
            newSpellData.Effects.Add(damageEffect);
            //Add Spell
            newSpellData.Buff = null;
            SpellManager.Add(newSpellData.ID, newSpellData);
            SpellNames.Add(newSpellData.ID, newSpellData.Name);
            #endregion
            #region Combustion : SpellId = 12,13 / BuffId = 7
            //Spell Data
            newSpellData = new SpellData();
            newSpellData.ID = 12;
            newSpellData.Range = 0;
            newSpellData.ActivationCost = 0;
            newSpellData.Name = "Combustion";
            newSpellData.SpellCastType = SpellCastType.Instant;
            newSpellData.BaseCastTime = 0;
            newSpellData.SpellTargetMode = SpellTargetMode.Self;
            newSpellData.SpellGlobal = SpellGlobalCooldownMode.NoCooldown;
            newSpellData.SpellCooldown = new Cooldown(10);
            newSpellData.IsHarmful = false;
            //Buff
            buff = new Buff(7, "Combustion", BuffType.Fixed, MagicType.Physical, AoeMode.None, 0, 5, 0, 0, false, false, false);
            {
                //Aoe Target Script Instant Fire Damage
                newScriptSpellData = new SpellData();
                newScriptSpellData.ID = 13;
                newScriptSpellData.ActivationCost = 0;
                newScriptSpellData.Name = "Combustion Area";
                newScriptSpellData.SpellCastType = SpellCastType.Instant;
                newScriptSpellData.BaseCastTime = 0;
                newScriptSpellData.SpellTargetMode = SpellTargetMode.Target;
                newScriptSpellData.SpellGlobal = SpellGlobalCooldownMode.NoCooldown;
                newScriptSpellData.SpellCooldown = new Cooldown(0);
                newScriptSpellData.IsHarmful = true;
                //Enemy Area 400-Circle Fire Damage 150-200 Effect
                damageEffect = new DamageEffect(DamageType.Fire, 150, 200, 0);
                aoeEffect = new AoeEffect(damageEffect, 400, AoeMode.Cleave);
                newScriptSpellData.Effects.Add(aoeEffect);
                newScriptSpellData.Buff = null;
                buff.SpellTriggers.Add(new SpellTrigger("Fire Blast", 100, 13));
            }
            //Add New Spell
            newSpellData.Buff = (Buff)buff.Clone();
            SpellManager.Add(newSpellData.ID, newSpellData);
            SpellManager.Add(newScriptSpellData.ID, newScriptSpellData);
            SpellNames.Add(newSpellData.ID, newSpellData.Name);
            SpellNames.Add(newScriptSpellData.ID, newScriptSpellData.Name);
            #endregion
            #region Deep Freeze : SpellId = 14 / BuffId = 8
            //Spell Data
            newSpellData = new SpellData();
            newSpellData.ID = 14;
            newSpellData.Range = 800;
            newSpellData.ActivationCost = 0;
            newSpellData.Name = "Deep Freeze";
            newSpellData.SpellCastType = SpellCastType.Instant;
            newSpellData.BaseCastTime = 0;
            newSpellData.SpellTargetMode = SpellTargetMode.Target;
            newSpellData.SpellGlobal = SpellGlobalCooldownMode.NoCooldown;
            newSpellData.SpellCooldown = new Cooldown(30f);
            newSpellData.IsHarmful = true;
            //Buff
            buff = new Buff(8, "Deep Freeze", BuffType.Debuff, MagicType.Magic, AoeMode.None, 0, 4, 0, 0, false, false, true);
            //Enemy Single Target Stun for 4 sec Aura
            buff.Auras.Add(new AuraModifier(AuraType.Stun, 0, 0, 4));
            //Enemy Single Target Freeze 4 sec Aura
            buff.Auras.Add(new AuraModifier(AuraType.Freeze, 0, 0, 4));
            //Add New Spell
            newSpellData.Buff = (Buff)buff.Clone();
            SpellManager.Add(newSpellData.ID, newSpellData);
            SpellNames.Add(newSpellData.ID, newSpellData.Name);
            #endregion
            #region Mass Polymorph : SpellId = 15 / BuffId = 9
            //Spell Data
            newSpellData = new SpellData();
            newSpellData.ID = 15;
            newSpellData.Range = 600;
            newSpellData.ActivationCost = 0;
            newSpellData.Name = "Mass Polymorph";
            newSpellData.SpellCastType = SpellCastType.Casted;
            newSpellData.BaseCastTime = 1.5f;
            newSpellData.SpellTargetMode = SpellTargetMode.Target;
            newSpellData.SpellGlobal = SpellGlobalCooldownMode.Normal;
            newSpellData.SpellCooldown = new Cooldown(50f);
            newSpellData.IsHarmful = true;
            //Animation
            if (SpriteCharacterManager.TryGetValue("Sheep", out sprite))
            {
                aoeEffect = new AoeEffect(new ModelChangeEffect(sprite, "Sheep", AoeMode.Cleave), 200, AoeMode.Cleave);
                newSpellData.Effects.Add(aoeEffect);
            }
            //Buff
            buff = new Buff(9, "Mass Polymorph", BuffType.Debuff, MagicType.Magic, AoeMode.Cleave, 200, 3, 0, 0, false, false, false);
            //Enemy Area Target Cleave-200 Polymorph for 3 sec Aura
            buff.Auras.Add(new AuraModifier(AuraType.Control, 0, 0, 3, AuraControlEffect.Polymorph));
            buff.Auras.Add(new AuraChangeModel("Sheep", 3));
            //Add Spell
            newSpellData.Buff = (Buff)buff.Clone();
            SpellManager.Add(newSpellData.ID, newSpellData);
            SpellNames.Add(newSpellData.ID, newSpellData.Name);
            #endregion
            #region Polymorph : SpellId = 16 / BuffId = 10
            //Spell Data
            newSpellData = new SpellData();
            newSpellData.ID = 16;
            newSpellData.Range = 600;
            newSpellData.ActivationCost = 0;
            newSpellData.Name = "Polymorph";
            newSpellData.SpellCastType = SpellCastType.Casted;
            newSpellData.BaseCastTime = 1.5f;
            newSpellData.SpellTargetMode = SpellTargetMode.Target;
            newSpellData.SpellGlobal = SpellGlobalCooldownMode.Normal;
            newSpellData.SpellCooldown = new Cooldown(30f);
            newSpellData.IsHarmful = true;
            //Animation
            if (SpriteCharacterManager.TryGetValue("Sheep", out sprite))
                newSpellData.Effects.Add(new ModelChangeEffect(sprite, "Sheep", AoeMode.None));
            //Buff
            buff = new Buff(10, "Polymorph", BuffType.Debuff, MagicType.Magic, AoeMode.None, 0, 3, 0, 0, false, false, false);
            //Enemy Single Target Polymorph for 3 sec Aura
            buff.Auras.Add(new AuraModifier(AuraType.Control, 0, 0, 3, AuraControlEffect.Polymorph));
            buff.Auras.Add(new AuraChangeModel("Sheep", 3));
            //Add New Spell
            newSpellData.Buff = (Buff)buff.Clone();
            SpellManager.Add(newSpellData.ID, newSpellData);
            SpellNames.Add(newSpellData.ID, newSpellData.Name);
            #endregion
            #region Frost Nova : SpellId = 17 / BuffId = 11
            //New Spell
            newSpellData = new SpellData();
            newSpellData.ID = 17;
            newSpellData.Range = 0;
            newSpellData.ActivationCost = 0;
            newSpellData.Name = "Frost Nova";
            newSpellData.SpellCastType = SpellCastType.Instant;
            newSpellData.BaseCastTime = 0;
            newSpellData.SpellTargetMode = SpellTargetMode.Self;
            newSpellData.SpellGlobal = SpellGlobalCooldownMode.Normal;
            newSpellData.SpellCooldown = new Cooldown(15f);
            newSpellData.IsHarmful = true;
            //Buff
            buff = new Buff(11, "Frost Nova", BuffType.Debuff, MagicType.Magic, AoeMode.Self, 400, 3, 0, 0, false, false, false);
            //Enemy Area 300-Circle Frost Damage 15-15 Effect
            damageEffect = new DamageEffect(DamageType.Frost, 15, 15, 0);
            aoeEffect = new AoeEffect(damageEffect, 400, AoeMode.Self);
            newSpellData.Effects.Add(aoeEffect);
            //Enemy Area NoTarget Root 3 sec Aura
            buff.Auras.Add(new AuraModifier(AuraType.Root, 0, 0, 3));
            //Enemy Area NoTarget Freeze 3 sec Aura
            buff.Auras.Add(new AuraModifier(AuraType.Freeze, 0, 0, 3));
            //Add Spell
            newSpellData.Buff = (Buff)buff.Clone();
            SpellManager.Add(newSpellData.ID, newSpellData);
            SpellNames.Add(newSpellData.ID, newSpellData.Name);
            #endregion
            #region Frost Bolt : SpellId = 18 / BUffId = 12
            //New Spell
            newSpellData = new SpellData();
            newSpellData.ID = 18;
            newSpellData.Range = 1200;
            newSpellData.ActivationCost = 0;
            newSpellData.Name = "Frost Bolt";
            newSpellData.SpellCastType = SpellCastType.Casted;
            newSpellData.BaseCastTime = 1.5f;
            newSpellData.SpellTargetMode = SpellTargetMode.Target;
            newSpellData.SpellGlobal = SpellGlobalCooldownMode.Normal;
            newSpellData.SpellCooldown = new Cooldown(0);
            newSpellData.IsHarmful = true;
            //Buff
            buff = new Buff(12, "Frost Bolt", BuffType.Debuff, MagicType.Magic, AoeMode.None, 0, 8, 0, 0, false, false, false);
            //Enemy Single Target Frost Damage 45-60 Effect
            damageEffect = new DamageEffect(DamageType.Frost, 45, 60, 0);
            newSpellData.Effects.Add(damageEffect);
            //Enemy Single Target Snare 8 sec
            buff.Auras.Add(new AuraModifier(AuraType.Speed, 0, 0.6f, 8));
            //Add Spell
            newSpellData.Buff = (Buff)buff.Clone();
            SpellManager.Add(newSpellData.ID, newSpellData);
            SpellNames.Add(newSpellData.ID, newSpellData.Name);
            #endregion
            #region Frost Fire Bolt : SpellId = 19 / BuffId = 13
            //Spell Data
            newSpellData = new SpellData();
            newSpellData.ID = 19;
            newSpellData.Range = 1200;
            newSpellData.ActivationCost = 0;
            newSpellData.Name = "Frost Fire Bolt";
            newSpellData.SpellCastType = SpellCastType.Casted;
            newSpellData.BaseCastTime = 1.8f;
            newSpellData.SpellTargetMode = SpellTargetMode.Target;
            newSpellData.SpellGlobal = SpellGlobalCooldownMode.Normal;
            newSpellData.SpellCooldown = new Cooldown(0);
            newSpellData.IsHarmful = true;
            //Buff
            buff = new Buff(13, "Frost Fire Bolt", BuffType.Debuff, MagicType.Magic, AoeMode.None, 0, 8, 0, 0, false, false, false);
            //Enemy Single Target Frost Damage 40-60
            damageEffect = new DamageEffect(DamageType.Frost, 40, 60, 0);
            newSpellData.Effects.Add(damageEffect);
            //Enemy Single Target Snare 50% for 8 sec
            buff.Auras.Add(new AuraModifier(AuraType.Speed, 0, 0.5f, 8));
            //Add Spell
            newSpellData.Buff = (Buff)buff.Clone();
            SpellManager.Add(newSpellData.ID, newSpellData);
            SpellNames.Add(newSpellData.ID, newSpellData.Name);
            #endregion
            #region Blazing Speed : SpellId = 20 / BuffId = 14
            //New Spell
            newSpellData = new SpellData();
            newSpellData.ID = 20;
            newSpellData.Range = 0;
            newSpellData.ActivationCost = 0;
            newSpellData.Name = "Blazing Speed";
            newSpellData.SpellCastType = SpellCastType.Instant;
            newSpellData.BaseCastTime = 0;
            newSpellData.SpellTargetMode = SpellTargetMode.Self;
            newSpellData.SpellGlobal = SpellGlobalCooldownMode.NoCooldown;
            newSpellData.SpellCooldown = new Cooldown(5f);
            newSpellData.IsHarmful = false;
            //Buff
            buff = new Buff(14, "Blazing Speed", BuffType.Buff, MagicType.Magic, AoeMode.None, 0, 1, 0, 0, false, false, false);
            //Self Single NoTarget Speed Boost 150% for 1 sec Aura
            buff.Auras.Add(new AuraModifier(AuraType.Speed, 0, 2.5f, 1));
            buff.Auras.Add(new AuraModifier(AuraType.SnareSupression, 0, 0, 1));
            //Add Spell
            newSpellData.Buff = (Buff)buff.Clone();
            SpellManager.Add(newSpellData.ID, newSpellData);
            SpellNames.Add(newSpellData.ID, newSpellData.Name);
            #endregion
            #region Living Bomb : SpellId = 21,22 / BuffId = 15
            //New Spell
            newSpellData = new SpellData();
            newSpellData.ID = 21;
            newSpellData.Range = 1000;
            newSpellData.ActivationCost = 0;
            newSpellData.Name = "Living Bomb";
            newSpellData.SpellCastType = SpellCastType.Instant;
            newSpellData.BaseCastTime = 0;
            newSpellData.SpellTargetMode = SpellTargetMode.Target;
            newSpellData.SpellGlobal = SpellGlobalCooldownMode.Normal;
            newSpellData.SpellCooldown = new Cooldown(0);
            newSpellData.IsHarmful = true;
            //Buff
            buff = new Buff(15, "Living Bomb", BuffType.Debuff, MagicType.Magic, AoeMode.None, 0, 8, 0, 0, false, false, true);
            {
                //Single Target Dot Damage
                buff.Auras.Add(new AuraPeriodicDamage(DamageType.Fire, 160, 4, 0, 8));
                //Aoe Target Script Instant Fire Damage
                newScriptSpellData = new SpellData();
                newScriptSpellData.ID = 22;
                newScriptSpellData.ActivationCost = 0;
                newScriptSpellData.Name = "Living Bomb Area";
                newScriptSpellData.SpellCastType = SpellCastType.Instant;
                newScriptSpellData.BaseCastTime = 0;
                newScriptSpellData.SpellTargetMode = SpellTargetMode.Target;
                newScriptSpellData.SpellGlobal = SpellGlobalCooldownMode.NoCooldown;
                newScriptSpellData.SpellCooldown = new Cooldown(0);
                newScriptSpellData.IsHarmful = true;
                //Enemy Area Self-400 Fire Damage 120-150 Effect
                damageEffect = new DamageEffect(DamageType.Fire, 120, 150, 0);
                aoeEffect = new AoeEffect(damageEffect, 400, AoeMode.Cleave);
                newScriptSpellData.Effects.Add(aoeEffect);
                newScriptSpellData.Buff = null;
                buff.Scripts.Add(new ScriptOnBuffExpired(new Spell(newScriptSpellData.ID, newScriptSpellData.Name, newScriptSpellData.SpellCooldown)));
            }
            //Add Spell
            newSpellData.Buff = (Buff)buff.Clone();
            SpellManager.Add(newSpellData.ID, newSpellData);
            SpellManager.Add(newScriptSpellData.ID, newScriptSpellData);
            SpellNames.Add(newSpellData.ID, newSpellData.Name);
            SpellNames.Add(newScriptSpellData.ID, newScriptSpellData.Name);
            #endregion
            #region Path of Fire : SpellId = 23,24,25 / BuffId = 16,17,18
            //New Spell
            newSpellData = new SpellData();
            newSpellData.ID = 23;
            newSpellData.Range = 0;
            newSpellData.ActivationCost = 0;
            newSpellData.Name = "Path of Fire";
            newSpellData.SpellCastType = SpellCastType.Instant;
            newSpellData.BaseCastTime = 0;
            newSpellData.SpellTargetMode = SpellTargetMode.Self;
            newSpellData.SpellGlobal = SpellGlobalCooldownMode.NoCooldown;
            newSpellData.SpellCooldown = new Cooldown(0);
            newSpellData.IsHarmful = false;
            //Buff
            buff = new Buff(16, "Path of Fire", BuffType.Fixed, MagicType.Physical, AoeMode.None, 0, -1, 0, 0, false, false, false);
            {
                //Aoe Target Script Instant Fire Damage
                newScriptSpellData = new SpellData();
                newScriptSpellData.ID = 24;
                newScriptSpellData.ActivationCost = 0;
                newScriptSpellData.Name = "Heating Up!";
                newScriptSpellData.SpellCastType = SpellCastType.Instant;
                newScriptSpellData.BaseCastTime = 0;
                newScriptSpellData.SpellTargetMode = SpellTargetMode.Self;
                newScriptSpellData.SpellGlobal = SpellGlobalCooldownMode.NoCooldown;
                newScriptSpellData.SpellCooldown = new Cooldown(0);
                scriptBuff = new Buff(17, "Heating Up!", BuffType.Fixed, MagicType.Magic, AoeMode.None, 0, 12, 0, 0, false, false, false);
                newScriptSpellData.Buff = (Buff)scriptBuff.Clone();
                {
                    newScriptSpellData1 = new SpellData();
                    newScriptSpellData1.ID = 25;
                    newScriptSpellData1.ActivationCost = 0;
                    newScriptSpellData1.Name = "Pyroblast!";
                    newScriptSpellData1.SpellCastType = SpellCastType.Instant;
                    newScriptSpellData1.BaseCastTime = 0;
                    newScriptSpellData1.SpellTargetMode = SpellTargetMode.Self;
                    newScriptSpellData1.SpellGlobal = SpellGlobalCooldownMode.NoCooldown;
                    newScriptSpellData1.SpellCooldown = new Cooldown(0);
                    newScriptSpellData1.Buff = new Buff(18, "Pyroblast!", BuffType.Buff, MagicType.Magic, AoeMode.None, 0, 12, 1, 1, true, false, false);
                    newScriptSpellData1.Buff.SpellModifiers.Add(new SpellInfoModifierFromBuff(2,
                        delegate(ITargetable caster, ITargetable target) { return true; },
                        SpellModifierType.InstantCast, 1, "Pyroblast"));
                }
                buff.Scripts.Add(new PyroblastScript(new Spell(newScriptSpellData1.ID, newScriptSpellData1.Name, newScriptSpellData1.SpellCooldown),
                    new Spell(newScriptSpellData.ID, newScriptSpellData.Name, newScriptSpellData.SpellCooldown)));
            }
            //Add New Spell
            newSpellData.Buff = (Buff)buff.Clone();
            SpellManager.Add(newSpellData.ID, newSpellData);
            SpellManager.Add(newScriptSpellData.ID, newScriptSpellData);
            SpellManager.Add(newScriptSpellData1.ID, newScriptSpellData1);
            SpellNames.Add(newSpellData.ID, newSpellData.Name);
            SpellNames.Add(newScriptSpellData.ID, newScriptSpellData.Name);
            SpellNames.Add(newScriptSpellData1.ID, newScriptSpellData1.Name);
            #endregion
            #region Ice Block : SpellId = 27 / BuffId = 19
            //New Spell
            newSpellData = new SpellData();
            newSpellData.ID = 27;
            newSpellData.Range = 0;
            newSpellData.ActivationCost = 0;
            newSpellData.Name = "Ice Block";
            newSpellData.SpellCastType = SpellCastType.Instant;
            newSpellData.BaseCastTime = 0;
            newSpellData.SpellTargetMode = SpellTargetMode.Self;
            newSpellData.SpellGlobal = SpellGlobalCooldownMode.NoCooldown;
            newSpellData.SpellCooldown = new Cooldown(25f);
            newSpellData.Effects.Add(new BuffRemovalEffect(item => item.BuffType == BuffType.Debuff));
            newSpellData.CastableInStun = true;
            newSpellData.CastableInControl = true;
            newSpellData.CastableInSilence = true;
            newSpellData.IsHarmful = false;
            //Buff
            buff = new Buff(19, "Ice Block", BuffType.Buff, MagicType.Magic, AoeMode.None, 0, 5, 0, 0, false, false, false);
            //Self Single NoTarget Root for 5 sec Aura
            buff.Auras.Add(new AuraModifier(AuraType.Root, 0, 0, 5));
            //Self Single NoTarget Pacify for 5 sec Aura
            buff.Auras.Add(new AuraModifier(AuraType.Pacify, 0, 0, 5));
            //Self Single NoTarget Immunity for 5 sec Aura
            buff.Auras.Add(new AuraModifier(AuraType.Invulnerability, 0, 0, 5));
            //Add Spell
            newSpellData.Buff = (Buff)buff.Clone();
            SpellManager.Add(newSpellData.ID, newSpellData);
            SpellNames.Add(newSpellData.ID, newSpellData.Name);
            #endregion
            #region Invisibility : SpellId = 28 / BuffId = 20
            //New Spell
            newSpellData = new SpellData();
            newSpellData.ID = 28;
            newSpellData.Range = 0;
            newSpellData.ActivationCost = 0;
            newSpellData.Name = "Invisibility";
            newSpellData.SpellCastType = SpellCastType.Instant;
            newSpellData.BaseCastTime = 0;
            newSpellData.SpellTargetMode = SpellTargetMode.Self;
            newSpellData.SpellGlobal = SpellGlobalCooldownMode.NoCooldown;
            newSpellData.SpellCooldown = new Cooldown(25f);
            newSpellData.IsHarmful = false;
            //Buff
            buff = new Buff(20, "Invisibility", BuffType.Buff, MagicType.Magic, AoeMode.None, 0, 5, 0, 0, false, false, false);
            //Self NoTargetable for 5 sec
            buff.Auras.Add(new AuraModifier(AuraType.NoTargetable, 0, 0, 5));
            //Self Pacify for 5 sec
            buff.Auras.Add(new AuraModifier(AuraType.Pacify, 0, 0, 5));
            //Self Immunity for 5 sec
            buff.Auras.Add(new AuraModifier(AuraType.Invulnerability, 0, 0, 5));
            //Self Invisibility for 5 sec
            buff.Auras.Add(new AuraModifier(AuraType.Invisibility, 0, 0, 5));
            //Add Spell
            newSpellData.Buff = (Buff)buff.Clone();
            SpellManager.Add(newSpellData.ID, newSpellData);
            SpellNames.Add(newSpellData.ID, newSpellData.Name);
            #endregion

            //Warrior
            #region Charge : SpellId = 40 / BuffId = 25
            //New Spell
            newSpellData = new SpellData();
            newSpellData.ID = 40;
            newSpellData.Range = 0;
            newSpellData.ActivationCost = 0;
            newSpellData.Name = "Charge";
            newSpellData.SpellCastType = SpellCastType.Instant;
            newSpellData.BaseCastTime = 0;
            newSpellData.SpellTargetMode = SpellTargetMode.Self;
            newSpellData.SpellGlobal = SpellGlobalCooldownMode.NoCooldown;
            newSpellData.SpellCooldown = new Cooldown(5f);
            newSpellData.IsHarmful = false;
            newSpellData.Effects.Add(new RootRemovalEffect(true, true));
            //Buff
            buff = new Buff(25, "Charge", BuffType.Buff, MagicType.Physical, AoeMode.None, 0, 1, 0, 0, false, false, false);
            //Self Single NoTarget Speed Boost 200% for 1 sec Aura
            buff.Auras.Add(new AuraModifier(AuraType.Speed, 0, 3f, 1));
            buff.Auras.Add(new AuraModifier(AuraType.SnareSupression, 0, 3f, 1)); 
            //Add Spell
            newSpellData.Buff = (Buff)buff.Clone();
            SpellManager.Add(newSpellData.ID, newSpellData);
            SpellNames.Add(newSpellData.ID, newSpellData.Name);
            #endregion
            #region Throwdown : SpellId = 41 / BuffId = 26
            //Spell Data
            newSpellData = new SpellData();
            newSpellData.ID = 41;
            newSpellData.Range = 200;
            newSpellData.ActivationCost = 0;
            newSpellData.Name = "Throwdown";
            newSpellData.SpellCastType = SpellCastType.Instant;
            newSpellData.BaseCastTime = 0;
            newSpellData.SpellTargetMode = SpellTargetMode.Target;
            newSpellData.SpellGlobal = SpellGlobalCooldownMode.NoCooldown;
            newSpellData.SpellCooldown = new Cooldown(10);
            newSpellData.IsHarmful = true;
            newSpellData.Effects.Add(new KnockBackEffect(3, 8));
            //Buff
            buff = new Buff(26, "Throwdown", BuffType.Debuff, MagicType.Physical, AoeMode.None, 0, 3, 0, 0, false, false, false);
            //Enemy Single Target Stun for 3 sec Aura
            buff.Auras.Add(new AuraModifier(AuraType.Stun, 0, 0, 3));
            //Add New Spell
            newSpellData.Buff = (Buff)buff.Clone();
            SpellManager.Add(newSpellData.ID, newSpellData);
            SpellNames.Add(newSpellData.ID, newSpellData.Name);
            #endregion
            #region Shockwave : SpellId = 42 / BuffId = None
            //Spell Data
            newSpellData = new SpellData();
            newSpellData.ID = 42;
            newSpellData.Range = 0;
            newSpellData.ActivationCost = 0;
            newSpellData.Name = "Shockwave";
            newSpellData.SpellCastType = SpellCastType.Instant;
            newSpellData.BaseCastTime = 0;
            newSpellData.SpellTargetMode = SpellTargetMode.Self;
            newSpellData.SpellGlobal = SpellGlobalCooldownMode.NoCooldown;
            newSpellData.SpellCooldown = new Cooldown(5f);
            newSpellData.IsHarmful = true;
            //Enemy Area NoTarget Fire Damage 80-90 Effect
            damageEffect = new DamageEffect(DamageType.Physical, 80, 90, 0);
            aoeEffect = new AoeEffect(damageEffect, 200, AoeMode.Cone);
            newSpellData.Effects.Add(aoeEffect);
            //Enemy Area NoTarget KnockBack 16-6 Effect
            knockBackEffect = new KnockBackEffect(3, 7);
            aoeEffect = new AoeEffect(knockBackEffect, 200, AoeMode.Cone);
            newSpellData.Effects.Add(aoeEffect);
            //Add New Spell
            newSpellData.Buff = null;
            SpellManager.Add(newSpellData.ID, newSpellData);
            SpellNames.Add(newSpellData.ID, newSpellData.Name);
            #endregion
            #region Bleed : SpellId = 43 / BuffId = 27
            //Spell Data
            newSpellData = new SpellData();
            newSpellData.ID = 43;
            newSpellData.Range = 200;
            newSpellData.ActivationCost = 0;
            newSpellData.Name = "Bleed";
            newSpellData.SpellCastType = SpellCastType.Instant;
            newSpellData.BaseCastTime = 0;
            newSpellData.SpellTargetMode = SpellTargetMode.Target;
            newSpellData.SpellGlobal = SpellGlobalCooldownMode.NoCooldown;
            newSpellData.SpellCooldown = new Cooldown(3.5f);
            newSpellData.IsHarmful = true;
            //Buff
            buff = new Buff(27, "Bleed", BuffType.Debuff, MagicType.Physical, AoeMode.None, 0, 9, 0, 0, false, false, true);
            //Enemy Single Target DoT for 5 sec Aura
            buff.Auras.Add(new AuraPeriodicDamage(DamageType.Physical,180, 3, 0, 9));
            //Add New Spell
            newSpellData.Buff = (Buff)buff.Clone();
            SpellManager.Add(newSpellData.ID, newSpellData);
            SpellNames.Add(newSpellData.ID, newSpellData.Name);
            #endregion
            #region Battle Shout : SpellId = 44 / BuffId = 28
            //New Spell
            newSpellData = new SpellData();
            newSpellData.ID = 44;
            newSpellData.Range = 0;
            newSpellData.ActivationCost = 0;
            newSpellData.Name = "Battle Shout";
            newSpellData.SpellCastType = SpellCastType.Instant;
            newSpellData.BaseCastTime = 0;
            newSpellData.SpellTargetMode = SpellTargetMode.Self;
            newSpellData.SpellGlobal = SpellGlobalCooldownMode.NoCooldown;
            newSpellData.SpellCooldown = new Cooldown(25f);
            newSpellData.IsHarmful = false;
            //Buff
            buff = new Buff(28, "Battle Shout", BuffType.Buff, MagicType.Physical, AoeMode.None, 0, 12, 0, 0, false, false, false);
            //Self Single NoTarget Crit Chance Modifier 30% for 12 sec Aura
            buff.Auras.Add(new AuraModifier(AuraType.CritChance, 30, 0, 12));
            //Add Spell
            newSpellData.Buff = (Buff)buff.Clone();
            SpellManager.Add(newSpellData.ID, newSpellData);
            SpellNames.Add(newSpellData.ID, newSpellData.Name);
            #endregion
            #region Thunder Clap : SpellId = 45 / BuffId = 29
            //New Spell
            newSpellData = new SpellData();
            newSpellData.ID = 45;
            newSpellData.Range = 0;
            newSpellData.ActivationCost = 0;
            newSpellData.Name = "Thunder Clap";
            newSpellData.SpellCastType = SpellCastType.Instant;
            newSpellData.BaseCastTime = 0;
            newSpellData.SpellTargetMode = SpellTargetMode.Self;
            newSpellData.SpellGlobal = SpellGlobalCooldownMode.NoCooldown;
            newSpellData.SpellCooldown = new Cooldown(1.5f);
            newSpellData.IsHarmful = true;
            //Buff
            buff = new Buff(29, "Thunder Clap", BuffType.Debuff, MagicType.Physical, AoeMode.Self, 300, 3, 0, 0, false, false, false);
            //Enemy Area NoTarget Self-300 Ppysical Damage 45-45 Effect
            damageEffect = new DamageEffect(DamageType.Physical, 45, 45, 0);
            aoeEffect = new AoeEffect(damageEffect, 300, AoeMode.Self);
            newSpellData.Effects.Add(aoeEffect);
            //Enemy Area NoTarget Root 3 sec Aura
            buff.Auras.Add(new AuraModifier(AuraType.Speed, 0, 0.7f, 3));
            //Enemy Area NoTarget DoT 3 sec Aura
            buff.Auras.Add(new AuraPeriodicDamage(DamageType.Physical, 90, 6, 0, 3));
            //Add Spell
            newSpellData.Buff = (Buff)buff.Clone();
            SpellManager.Add(newSpellData.ID, newSpellData);
            SpellNames.Add(newSpellData.ID, newSpellData.Name);
            #endregion
            #region Hamstring : SpellId = 46 / BuffId = 30
            //New Spell
            newSpellData = new SpellData();
            newSpellData.ID = 46;
            newSpellData.Range = 200;
            newSpellData.ActivationCost = 0;
            newSpellData.Name = "Hamstring";
            newSpellData.SpellCastType = SpellCastType.Instant;
            newSpellData.BaseCastTime = 0;
            newSpellData.SpellTargetMode = SpellTargetMode.Target;
            newSpellData.SpellGlobal = SpellGlobalCooldownMode.Normal;
            newSpellData.SpellCooldown = new Cooldown(0);
            newSpellData.IsHarmful = true;
            //Buff
            buff = new Buff(30, "Hamstring", BuffType.Debuff, MagicType.Physical, AoeMode.None, 0, 6, 0, 0, false, false, false);
            //Enemy Area NoTarget Speed Reduction 70% 6 sec Aura
            buff.Auras.Add(new AuraModifier(AuraType.Speed, 0, 0.3f, 6));
            //Add Spell
            newSpellData.Buff = (Buff)buff.Clone();
            SpellManager.Add(newSpellData.ID, newSpellData);
            SpellNames.Add(newSpellData.ID, newSpellData.Name);
            #endregion
            #region Mortal Strike : SpellId = 47 / BuffId = None
            //Spell Data
            newSpellData = new SpellData();
            newSpellData.ID = 47;
            newSpellData.Range = 200;
            newSpellData.ActivationCost = 0;
            newSpellData.Name = "Mortal Strike";
            newSpellData.SpellCastType = SpellCastType.Instant;
            newSpellData.BaseCastTime = 0;
            newSpellData.SpellTargetMode = SpellTargetMode.Target;
            newSpellData.SpellGlobal = SpellGlobalCooldownMode.NoCooldown;
            newSpellData.SpellCooldown = new Cooldown(1.2f);
            newSpellData.IsHarmful = true;
            //Enemy Single Target Physical Damage 50-70 Effect
            damageEffect = new DamageEffect(DamageType.Physical, 50, 70, 0);
            newSpellData.Effects.Add(damageEffect);
            //Add New Spell
            newSpellData.Buff = null;
            SpellManager.Add(newSpellData.ID, newSpellData);
            SpellNames.Add(newSpellData.ID, newSpellData.Name);
            #endregion
            #region Shield Wall : SpellId = 48 / BuffId = 31
            //New Spell
            newSpellData = new SpellData();
            newSpellData.ID = 48;
            newSpellData.Range = 0;
            newSpellData.ActivationCost = 0;
            newSpellData.Name = "Shield Wall";
            newSpellData.SpellCastType = SpellCastType.Instant;
            newSpellData.BaseCastTime = 0;
            newSpellData.SpellTargetMode = SpellTargetMode.Self;
            newSpellData.SpellGlobal = SpellGlobalCooldownMode.NoCooldown;
            newSpellData.SpellCooldown = new Cooldown(15f);
            newSpellData.IsHarmful = false;
            //Buff
            buff = new Buff(31, "Shield Wall", BuffType.Buff, MagicType.Physical, AoeMode.None, 0, 9, 0, 0, false, false, false);
            //Self Single NoTarget Damage Reduction 40% for 9 sec
            buff.Auras.Add(new AuraModifier(AuraType.DamageReduction, 0.4f, 0, 9));
            //Add Spell
            newSpellData.Buff = (Buff)buff.Clone();
            SpellManager.Add(newSpellData.ID, newSpellData);
            SpellNames.Add(newSpellData.ID, newSpellData.Name);
            #endregion
            #region Avatar : SpellId = 49 / BuffId = 32
            //New Spell
            newSpellData = new SpellData();
            newSpellData.ID = 49;
            newSpellData.Range = 0;
            newSpellData.ActivationCost = 0;
            newSpellData.Name = "Avatar";
            newSpellData.SpellCastType = SpellCastType.Instant;
            newSpellData.BaseCastTime = 0;
            newSpellData.SpellTargetMode = SpellTargetMode.Self;
            newSpellData.SpellGlobal = SpellGlobalCooldownMode.NoCooldown;
            newSpellData.SpellCooldown = new Cooldown(15f);
            newSpellData.IsHarmful = false;
            //Buff
            buff = new Buff(32, "Avatar", BuffType.Buff, MagicType.Physical, AoeMode.None, 0, 12, 0, 0, false, false, false);
            //Self Single NoTarget Damage 60% for 12 sec
            buff.Auras.Add(new AuraModifier(AuraType.PhysicalDamageMultiplier, 0, 1.6f, 12));
            //Add Spell
            newSpellData.Buff = (Buff)buff.Clone();
            SpellManager.Add(newSpellData.ID, newSpellData);
            SpellNames.Add(newSpellData.ID, newSpellData.Name);
            #endregion
            #region Cleave : SpellId = 50 / BuffId = None
            //New Spell
            newSpellData = new SpellData();
            newSpellData.ID = 50;
            newSpellData.Range = 100;
            newSpellData.ActivationCost = 0;
            newSpellData.Name = "Cleave";
            newSpellData.SpellCastType = SpellCastType.Instant;
            newSpellData.BaseCastTime = 0;
            newSpellData.SpellTargetMode = SpellTargetMode.Target;
            newSpellData.SpellGlobal = SpellGlobalCooldownMode.Normal;
            newSpellData.SpellCooldown = new Cooldown(3f);
            newSpellData.IsHarmful = true;
            //Self Aoe NoTarget Damage 100% for 24 sec
            newSpellData.Effects.Add(new AoeEffect(new DamageEffect(DamageType.Physical, 30, 40, 0), 300, AoeMode.Cleave));
            //Add Spell
            newSpellData.Buff = null;
            SpellManager.Add(newSpellData.ID, newSpellData);
            SpellNames.Add(newSpellData.ID, newSpellData.Name);
            #endregion

            //Priest
            #region Shadow Form : SpellId = 91 / BuffId = 65
            //Spell Data
            newSpellData = new SpellData();
            newSpellData.ID = 91;
            newSpellData.Range = 0;
            newSpellData.ActivationCost = 0;
            newSpellData.Name = "Shadow Form";
            newSpellData.SpellCastType = SpellCastType.Instant;
            newSpellData.BaseCastTime = 0;
            newSpellData.SpellTargetMode = SpellTargetMode.Self;
            newSpellData.SpellGlobal = SpellGlobalCooldownMode.NoCooldown;
            newSpellData.SpellCooldown = new Cooldown(3);
            newSpellData.IsHarmful = false;
            //Animation
            if (SpriteCharacterManager.TryGetValue("Shadow Form", out sprite))
                newSpellData.Effects.Add(new ModelChangeEffect(sprite, "Shadow Form", AoeMode.None));
            //Buff
            buff = new Buff(65, "Shadow Form", BuffType.Fixed, MagicType.Magic, AoeMode.None, 0, 10, 0, 0, false, false, false);
            //Self Single NoTarget Buff for 10 sec Aura
            buff.Auras.Add(new AuraModifier(AuraType.DamageReduction, 0.15f, 0, 10));
            buff.Auras.Add(new AuraModifier(AuraType.DamageReduction, 0.15f, 0, 10));
            buff.Auras.Add(new AuraChangeModel("Shadow Form", 10));
            //Add New Spell
            newSpellData.Buff = (Buff)buff.Clone();
            SpellManager.Add(newSpellData.ID, newSpellData);
            SpellNames.Add(newSpellData.ID, newSpellData.Name);
            #endregion
            #region Psychic Scream : SpellId = 92 / BuffId = 66
            //Spell Data
            newSpellData = new SpellData();
            newSpellData.ID = 92;
            newSpellData.Range = 0;
            newSpellData.ActivationCost = 0;
            newSpellData.Name = "Psychic Scream";
            newSpellData.SpellCastType = SpellCastType.Instant;
            newSpellData.BaseCastTime = 0;
            newSpellData.SpellTargetMode = SpellTargetMode.Self;
            newSpellData.SpellGlobal = SpellGlobalCooldownMode.NoCooldown;
            newSpellData.SpellCooldown = new Cooldown(1);
            newSpellData.IsHarmful = true;
            //Buff
            buff = new Buff(66, "Psychic Scream", BuffType.Debuff, MagicType.Magic, AoeMode.Self, 300, 5, 0, 0, false, false, false);
            //Self Single NoTarget Buff for 10 sec Aura
            buff.Auras.Add(new AuraModifier(AuraType.Control, 0, 0, 5, AuraControlEffect.Fear));
            //Add New Spell
            newSpellData.Buff = (Buff)buff.Clone();
            SpellManager.Add(newSpellData.ID, newSpellData);
            SpellNames.Add(newSpellData.ID, newSpellData.Name);
            #endregion
            #region Silence : SpellId = 93 / BuffId = 67
            //Spell Data
            newSpellData = new SpellData();
            newSpellData.ID = 93;
            newSpellData.Range = 800;
            newSpellData.ActivationCost = 0;
            newSpellData.Name = "Silence";
            newSpellData.SpellCastType = SpellCastType.Instant;
            newSpellData.BaseCastTime = 0;
            newSpellData.SpellTargetMode = SpellTargetMode.Target;
            newSpellData.SpellGlobal = SpellGlobalCooldownMode.NoCooldown;
            newSpellData.SpellCooldown = new Cooldown(4);
            newSpellData.IsHarmful = true;
            //Buff
            buff = new Buff(67, "Silence", BuffType.Debuff, MagicType.Magic, AoeMode.None, 0, 5, 0, 0, false, false, false);
            //Enemy Single Target Silence 5 sec
            buff.Auras.Add(new AuraModifier(AuraType.Silence, 0, 0, 5));
            //Add Spell
            newSpellData.Buff = (Buff)buff.Clone();
            SpellManager.Add(newSpellData.ID, newSpellData);
            SpellNames.Add(newSpellData.ID, newSpellData.Name);
            #endregion
            #region Mind Spike : SpellId = 94 / BuffId = None
            //Spell Data
            newSpellData = new SpellData();
            newSpellData.ID = 94;
            newSpellData.Range = 1000;
            newSpellData.ActivationCost = 0;
            newSpellData.Name = "Mind Spike";
            newSpellData.SpellCastType = SpellCastType.Casted;
            newSpellData.BaseCastTime = 1;
            newSpellData.SpellTargetMode = SpellTargetMode.Target;
            newSpellData.SpellGlobal = SpellGlobalCooldownMode.NoCooldown;
            newSpellData.SpellCooldown = new Cooldown(0);
            newSpellData.IsHarmful = true;
            //Enemy Single Target Fire Damage 20-30 Effect
            damageEffect = new DamageEffect(DamageType.Shadow, 20, 30, 0);
            newSpellData.Effects.Add(damageEffect);
            //Add Spell
            newSpellData.Buff = null;
            SpellManager.Add(newSpellData.ID, newSpellData);
            SpellNames.Add(newSpellData.ID, newSpellData.Name);
            #endregion
            #region Pain Suppression : SpellId = 95 / BuffId = 68
            //New Spell
            newSpellData = new SpellData();
            newSpellData.ID = 95;
            newSpellData.Range = 0;
            newSpellData.ActivationCost = 0;
            newSpellData.Name = "Pain Suppression";
            newSpellData.SpellCastType = SpellCastType.Instant;
            newSpellData.BaseCastTime = 0;
            newSpellData.SpellTargetMode = SpellTargetMode.Self;
            newSpellData.SpellGlobal = SpellGlobalCooldownMode.NoCooldown;
            newSpellData.SpellCooldown = new Cooldown(5);
            newSpellData.IsHarmful = false;
            //Buff
            buff = new Buff(68, "Pain Suppression", BuffType.Buff, MagicType.Physical, AoeMode.None, 0, 8, 0, 0, false, false, false);
            //Self Single NoTarget Damage Reduction 40% for 8 sec
            buff.Auras.Add(new AuraModifier(AuraType.DamageReduction, 0.40f, 0, 8));
            //Add Spell
            newSpellData.Buff = (Buff)buff.Clone();
            SpellManager.Add(newSpellData.ID, newSpellData);
            SpellNames.Add(newSpellData.ID, newSpellData.Name);
            #endregion
            #region Greater Heal : SpellId = 96 / BuffId = None
            //Spell Data
            newSpellData = new SpellData();
            newSpellData.ID = 96;
            newSpellData.Range = 1000;
            newSpellData.ActivationCost = 0;
            newSpellData.Name = "Greater Heal";
            newSpellData.SpellCastType = SpellCastType.Casted;
            newSpellData.BaseCastTime = 2;
            newSpellData.SpellTargetMode = SpellTargetMode.Target;
            newSpellData.SpellGlobal = SpellGlobalCooldownMode.NoCooldown;
            newSpellData.SpellCooldown = new Cooldown(0);
            newSpellData.IsHarmful = false;
            //Enemy Single Target Heal 160-200 Effect
            newSpellData.Effects.Add(new HealEffect(160, 200, 0));
            //Add Spell
            newSpellData.Buff = null;
            SpellManager.Add(newSpellData.ID, newSpellData);
            SpellNames.Add(newSpellData.ID, newSpellData.Name);
            #endregion
            #region Phantasm : SpellId = 97 / BuffId = 69
            //New Spell
            newSpellData = new SpellData();
            newSpellData.ID = 97;
            newSpellData.Range = 0;
            newSpellData.ActivationCost = 0;
            newSpellData.Name = "Phantasm";
            newSpellData.SpellCastType = SpellCastType.Instant;
            newSpellData.BaseCastTime = 0;
            newSpellData.SpellTargetMode = SpellTargetMode.Self;
            newSpellData.SpellGlobal = SpellGlobalCooldownMode.NoCooldown;
            newSpellData.SpellCooldown = new Cooldown(1.5f);
            newSpellData.IsHarmful = false;
            //Buff
            buff = new Buff(69, "Phantasm", BuffType.Buff, MagicType.Magic, AoeMode.None, 0, 5, 0, 0, false, false, false);
            //Self Single NoTarget Speed Boost 150% for 1 sec Aura
            buff.Auras.Add(new AuraModifier(AuraType.SnareSupression, 0, 0, 5));
            //Add Spell
            newSpellData.Buff = (Buff)buff.Clone();
            SpellManager.Add(newSpellData.ID, newSpellData);
            SpellNames.Add(newSpellData.ID, newSpellData.Name);
            #endregion
            #region Vampiric Touch: SpellId = 98 / BuffId = 70
            //New Spell
            newSpellData = new SpellData();
            newSpellData.ID = 98;
            newSpellData.Range = 800;
            newSpellData.ActivationCost = 0;
            newSpellData.Name = "Vampiric Touch";
            newSpellData.SpellCastType = SpellCastType.Casted;
            newSpellData.BaseCastTime = 1.5f;
            newSpellData.SpellTargetMode = SpellTargetMode.Target;
            newSpellData.SpellGlobal = SpellGlobalCooldownMode.Normal;
            newSpellData.SpellCooldown = new Cooldown(0);
            newSpellData.IsHarmful = true;
            //Buff
            buff = new Buff(70, "Vampiric Touch", BuffType.Debuff, MagicType.Magic, AoeMode.None, 0, 15, 0, 0, false, false, false);
            //Enemy Single Target DoT Damage Aura
            buff.Auras.Add(new AuraPeriodicDamage(DamageType.Shadow, 120, 5, 0, 15));
            //Add Spell
            newSpellData.Buff = (Buff)buff.Clone();
            SpellManager.Add(newSpellData.ID, newSpellData);
            SpellNames.Add(newSpellData.ID, newSpellData.Name);
            #endregion
            #region Renew: SpellId = 99 / BuffId = 71
            //New Spell
            newSpellData = new SpellData();
            newSpellData.ID = 99;
            newSpellData.Range = 200;
            newSpellData.ActivationCost = 0;
            newSpellData.Name = "Renew";
            newSpellData.SpellCastType = SpellCastType.Instant;
            newSpellData.BaseCastTime = 0;
            newSpellData.SpellTargetMode = SpellTargetMode.Target;
            newSpellData.SpellGlobal = SpellGlobalCooldownMode.Normal;
            newSpellData.SpellCooldown = new Cooldown(0);
            newSpellData.IsHarmful = false;
            //Buff
            buff = new Buff(71, "Renew", BuffType.Debuff, MagicType.Magic, AoeMode.None, 0, 12, 0, 0, false, false, false);
            //Enemy Single Target DoT Damage Aura
            buff.Auras.Add(new AuraPeriodicHealing(120, 4, 0, 12));
            //Add Spell
            newSpellData.Buff = (Buff)buff.Clone();
            SpellManager.Add(newSpellData.ID, newSpellData);
            SpellNames.Add(newSpellData.ID, newSpellData.Name);
            #endregion
            #region Mind Blast : SpellId = 100 / BuffId = None
            //Spell Data
            newSpellData = new SpellData();
            newSpellData.ID = 100;
            newSpellData.Range = 1000;
            newSpellData.ActivationCost = 0;
            newSpellData.Name = "Mind Blast";
            newSpellData.SpellCastType = SpellCastType.Casted;
            newSpellData.BaseCastTime = 1;
            newSpellData.SpellTargetMode = SpellTargetMode.Target;
            newSpellData.SpellGlobal = SpellGlobalCooldownMode.NoCooldown;
            newSpellData.SpellCooldown = new Cooldown(0);
            newSpellData.IsHarmful = true;
            //Enemy Single Target Fire Damage 20-30 Effect
            damageEffect = new DamageEffect(DamageType.Shadow, 90, 120, 0);
            newSpellData.Effects.Add(damageEffect);
            //Add Spell
            newSpellData.Buff = null;
            SpellManager.Add(newSpellData.ID, newSpellData);
            SpellNames.Add(newSpellData.ID, newSpellData.Name);
            #endregion
            #region Shadow Word: Pain : SpellId = 101 / BuffId = 72
            //New Spell
            newSpellData = new SpellData();
            newSpellData.ID = 101;
            newSpellData.Range = 800;
            newSpellData.ActivationCost = 0;
            newSpellData.Name = "Shadow Word: Pain";
            newSpellData.SpellCastType = SpellCastType.Instant;
            newSpellData.BaseCastTime = 0;
            newSpellData.SpellTargetMode = SpellTargetMode.Target;
            newSpellData.SpellGlobal = SpellGlobalCooldownMode.Normal;
            newSpellData.SpellCooldown = new Cooldown(0);
            newSpellData.IsHarmful = true;
            //Buff
            buff = new Buff(72, "Shadow Word: Pain", BuffType.Debuff, MagicType.Magic, AoeMode.None, 0, 15, 0, 0, false, false, false);
            //Enemy Single Target DoT Damage Aura
            buff.Auras.Add(new AuraPeriodicDamage(DamageType.Shadow, 180, 6, 0, 18));
            //Add Spell
            newSpellData.Buff = (Buff)buff.Clone();
            SpellManager.Add(newSpellData.ID, newSpellData);
            SpellNames.Add(newSpellData.ID, newSpellData.Name);
            #endregion
            #region Divine Insight : SpellId = 102 / BuffId = 73
            //New Spell
            newSpellData = new SpellData();
            newSpellData.ID = 102;
            newSpellData.Range = 800;
            newSpellData.ActivationCost = 0;
            newSpellData.Name = "Divine Insight";
            newSpellData.SpellCastType = SpellCastType.Instant;
            newSpellData.BaseCastTime = 0;
            newSpellData.SpellTargetMode = SpellTargetMode.Self;
            newSpellData.SpellGlobal = SpellGlobalCooldownMode.NoCooldown;
            newSpellData.SpellCooldown = new Cooldown(0);
            newSpellData.IsHarmful = false;
            //Buff
            buff = new Buff(73, "Divine Insight", BuffType.Fixed, MagicType.Magic, AoeMode.None, 0, -1, 0, 0, false, false, false);
            //Enemy Single Target DoT Damage Aura
            buff.SpellTriggers.Add(new SpellTrigger("Mind Spike", 50, 103));
            //Add Spell
            newSpellData.Buff = (Buff)buff.Clone();
            SpellManager.Add(newSpellData.ID, newSpellData);
            SpellNames.Add(newSpellData.ID, newSpellData.Name);
            #endregion
            #region Divine Insight Shadow : SpellId = 103 / BuffId = 74
            //New Spell
            newSpellData = new SpellData();
            newSpellData.ID = 103;
            newSpellData.Range = 0;
            newSpellData.ActivationCost = 0;
            newSpellData.Name = "Divine Insight Shadow";
            newSpellData.SpellCastType = SpellCastType.Instant;
            newSpellData.BaseCastTime = 0;
            newSpellData.SpellTargetMode = SpellTargetMode.Self;
            newSpellData.SpellGlobal = SpellGlobalCooldownMode.NoCooldown;
            newSpellData.SpellCooldown = new Cooldown(0);
            newSpellData.IsHarmful = false;
            //Buff
            buff = new Buff(74, "Divine Insight Shadow", BuffType.Buff, MagicType.Magic, AoeMode.None, 0, 12, 2, 2, true, false, false, false);
            //Enemy Single Target DoT Damage Aura
            buff.SpellModifiers.Add(new SpellInfoModifierFromBuff(3, delegate(ITargetable caster, ITargetable target)
                { return true; }, SpellModifierType.InstantCast, 0, "Mind Blast"));
            //Add Spell
            newSpellData.Buff = (Buff)buff.Clone();
            SpellManager.Add(newSpellData.ID, newSpellData);
            SpellNames.Add(newSpellData.ID, newSpellData.Name);
            #endregion
            #region Shadow Word: Insanity : SpellId = 104 / BuffId = None
            //New Spell
            newSpellData = new SpellData();
            newSpellData.ID = 104;
            newSpellData.Range = 800;
            newSpellData.ActivationCost = 0;
            newSpellData.Name = "Shadow Word: Insanity";
            newSpellData.SpellCastType = SpellCastType.Instant;
            newSpellData.BaseCastTime = 0;
            newSpellData.SpellTargetMode = SpellTargetMode.Target;
            newSpellData.SpellGlobal = SpellGlobalCooldownMode.Normal;
            newSpellData.SpellCooldown = new Cooldown(0);
            newSpellData.IsHarmful = true;
            //Enemy Single Target DoT Damage Aura
            newSpellData.Effects.Add(new DamageEffect(DamageType.Shadow, 140, 160, 0));
            //Add Spell
            SpellManager.Add(newSpellData.ID, newSpellData);
            SpellNames.Add(newSpellData.ID, newSpellData.Name);
            #endregion
            #endregion
        }

        public void Update(TimeSpan elapsedGameTime, NetworkPlayerInterface playerUI)
        {
            Creatures.RemoveAll(CharacterDied);

            #region Creature Cell Allocation
            for (int i = 0; i < PartitionedCreatures.GetLength(0); i++)
                for (int j = 0; j < PartitionedCreatures.GetLength(1); j++)
                    PartitionedCreatures[i, j].Clear();
          
            foreach (Creature monster in Creatures)
                if (monster.Character.Entity.Health.CurrentValue != 0)
                    PartitionedCreatures[(int)monster.BoundRect.BottomCenter.Y / 200,
                        (int)monster.BoundRect.BottomCenter.X / 200].Add(monster);
            #endregion

            foreach (TargetingProjectile projectile in TargetingProjectiles)
                if (projectile.Update(elapsedGameTime, this))
                {
                    ApplySpell(projectile.Caster, projectile.Spell, projectile.Target, false, true);
                    projectile.Dispose();
                }
            foreach (StaticProjectile projectile in StaticProjectiles)
                if (projectile.Update(elapsedGameTime, this))
                    projectile.Dispose();

            TargetingProjectiles.RemoveAll(item => item.NeedsDispose == true);
            StaticProjectiles.RemoveAll(item => item.NeedsDispose == true);
            bool maybeBuffChanged;

            #region Players Update
            foreach (NetworkPlayer player in Characters)
            {
                maybeBuffChanged = false;
                foreach (Buff buff in player.Character.Entity.Buffs)
                {
                    if (buff.Update(elapsedGameTime))
                        maybeBuffChanged = true;

                    if (maybeBuffChanged)
                        foreach (BasicScript script in buff.Scripts)
                            if (script.Triggered)
                            {
                                SpellData spellData;
                                if (SpellManager.TryGetValue(script.Spell.SpellDataId, out spellData))
                                    ApplyConfirmedSpell(script.ScriptApplier, script.Spell, spellData, player, false, false, true);                
                            }
                }
                player.Character.Update(elapsedGameTime, maybeBuffChanged);

                #region Player Check Untargetable Target
                if (player.Character.SpellCast != null)
                {
                    if (player.Character.SpellCast.CheckInterrupt())
                    {
                        player.Character.SpellCast.Dispose();
                        player.Character.SpellCast = null;
                    }
                    else if (player.Character.SpellCast.CastTimeLeft == TimeSpan.Zero)
                    {
                        ApplySpell(player, player.Character.SpellCast.Spell,
                            player.Character.SpellCast.Target, true, false);
                        player.Character.SpellCast.Dispose();
                        player.Character.SpellCast = null;
                    }
                }
                if (player.Character.Target != null
                    && (player.Character.Target.NeedsDispose
                        || player.Character.Target.Character.Entity.IsNotTargetable))
                {
                    player.Character.Target = null;
                }
                #endregion
            }
            foreach (NetworkPlayer player in Characters)
                CharacterAnimationHandle(player);
            #endregion

            #region Monsters Update
            foreach (Creature monster in Creatures)
            {
                maybeBuffChanged = false;
                foreach (Buff buff in monster.Character.Entity.Buffs)
                {
                    if (buff.Update(elapsedGameTime))
                        maybeBuffChanged = true;

                    if (maybeBuffChanged)
                        foreach (BasicScript script in buff.Scripts)
                            if (script.Triggered)
                            {
                                SpellData spellData;
                                if (SpellManager.TryGetValue(script.Spell.SpellDataId, out spellData))
                                    ApplyConfirmedSpell(script.ScriptApplier, script.Spell, spellData, monster, false, false, true); 
                            }
                }
                monster.Character.Update(elapsedGameTime, maybeBuffChanged);
                monster.Update(elapsedGameTime, this);
                monster.ApplyStrategy(this);

                #region Creature Check Untargetable Target
                if (monster.Character.SpellCast != null)
                    if (monster.Character.SpellCast.CheckInterrupt())
                    {
                        monster.Character.SpellCast.Dispose();
                        monster.Character.SpellCast = null;
                    }
                    else if (monster.Character.SpellCast.CastTimeLeft == TimeSpan.Zero)
                    {
                        ApplySpell(monster, monster.Character.SpellCast.Spell,
                            monster.Character.SpellCast.Target, true, false);
                        monster.Character.SpellCast.Dispose();
                        monster.Character.SpellCast = null;
                    }
                if (monster.Character.Target != null
                    && (monster.Character.Target.NeedsDispose
                        || monster.Character.Target.Character.Entity.IsNotTargetable))
                {
                    monster.Character.Target = null;
                }
                #endregion
            }
            foreach (Creature monster in Creatures)
                CharacterAnimationHandle(monster);
            #endregion
        }
        public void Draw(GameTime gameTime, SpriteBatch spriteBatch, Camera camera)
        {
            MainMap.Draw(spriteBatch, camera);
            foreach (Creature monster in Creatures)
                monster.Draw(spriteBatch);
            foreach (NetworkPlayer player in Characters)
                player.Draw(spriteBatch);
            foreach (StaticProjectile projectile in StaticProjectiles)
                projectile.Draw(gameTime, spriteBatch);
            foreach (TargetingProjectile projectile in TargetingProjectiles)
                projectile.Draw(gameTime, spriteBatch);
        }

        public bool ApplySpell(ITargetable caster, Spell spell, ITargetable target,
            bool fromSpellCast, bool fromProjectile)
        {
            #region If spell not found or caster/target is disposed
            SpellData spellData;
            if (!SpellManager.TryGetValue(spell.SpellDataId, out spellData)
                || (target != null && target.NeedsDispose)
                || caster.NeedsDispose)
                return false;
            #endregion

            #region If spell comes from projectile or spellcast
            AnimatedSprite sprite;
            if (fromProjectile)
            {
                #region From Projectile
                string soundPath;

                if (OnHitSoundManager.TryGetValue(spell.Name, out soundPath))
                    GameRef.Content.Load<SoundEffect>(soundPath).Play(0.8f,0f,0f);
                else if (OnCastSoundManager.TryGetValue(spell.Name, out soundPath))
                    GameRef.Content.Load<SoundEffect>(soundPath).Play(0.8f,0f,0f);

                ApplyConfirmedSpell(caster, spell, spellData, target, false, true, false);
                return true;
                #endregion
            }
            else if (fromSpellCast)
            {
                #region From SpellCast
                if (spellData.SpellTargetMode == SpellTargetMode.Target)
                {
                    if (caster.Character.Entity.IsModelChanged && caster.Character.Entity.CurrentReplacedModel != null)
                    {
                        caster.Character.Entity.CurrentReplacedModel.PerformAnimation = true;
                        caster.Character.Entity.CurrentReplacedModel.CurrentAnimation = caster.Position.X > target.Position.X ?
                            AnimationKey.CastLeft : AnimationKey.CastRight;
                        caster.Character.Entity.CurrentReplacedModel.ResetCurrentAnimation();
                    }
                    else
                    {
                        caster.Character.Sprite.PerformAnimation = true;
                        caster.Character.Sprite.CurrentAnimation = caster.Position.X > target.Position.X ?
                            AnimationKey.CastLeft : AnimationKey.CastRight;
                        caster.Character.Sprite.ResetCurrentAnimation();
                    }
                }
                else
                {
                    if (caster.Character.Entity.IsModelChanged && caster.Character.Entity.CurrentReplacedModel != null)
                    {
                        caster.Character.Entity.CurrentReplacedModel.PerformAnimation = true;
                        caster.Character.Entity.CurrentReplacedModel.CurrentAnimation = caster.Character.IsDirectedRight ?
                                AnimationKey.CastRight : AnimationKey.CastLeft;
                        caster.Character.Entity.CurrentReplacedModel.ResetCurrentAnimation();
                    }
                    else
                    {
                        caster.Character.Sprite.PerformAnimation = true;
                        caster.Character.Sprite.CurrentAnimation = caster.Character.IsDirectedRight ?
                                AnimationKey.CastRight : AnimationKey.CastLeft;
                        caster.Character.Sprite.ResetCurrentAnimation();
                    }
                }
                if (SpriteSpellManager.TryGetValue(spell.Name, out sprite))
                    TargetingProjectiles.Add(new TargetingProjectile(sprite, spell, 15, caster, caster.Character.SpellCast.Target, GameRef));
                else
                    ApplyConfirmedSpell(caster, spell, spellData, caster.Character.SpellCast.Target, true, false, false);
                return true;
                #endregion
            }
            #endregion

            #region If caster cannot use skills
                if (caster.Character.Entity.IsPacified)
                    return false;
                #endregion

            #region If caster can't cast due to states, cooldown and false target
                if (caster.Character.Entity.IsSilenced && !spellData.CastableInSilence || caster.Character.Entity.IsNoControlable && !spellData.CastableInControl
                    || !CheckSpellCooldown(caster, spell, spellData) || !CheckSpellTargetMode(caster, spellData))
                    return false;
                #endregion

            #region If spell doesn't meet requirements
                SpellModificationInformation spellInfo = new SpellModificationInformation(spell.Name);
                foreach (SpellModifier modifier in spell.Modifiers)
                    if (modifier.CheckForFailure(caster, caster.Character.Target, spellInfo, spell))
                        return false;
                #endregion

            #region If spell is instant or casted
            bool IsCastApplied;
            if (!CheckSpellCastType(caster, spell, spellData, out IsCastApplied))
                return IsCastApplied;
            #endregion

            #region Handle animation
            if (spellData.SpellTargetMode == SpellTargetMode.Target)
            {
                if (caster.Character.Entity.IsModelChanged && caster.Character.Entity.CurrentReplacedModel != null)
                {
                    caster.Character.Entity.CurrentReplacedModel.PerformAnimation = true;
                    caster.Character.Entity.CurrentReplacedModel.CurrentAnimation = caster.Position.X > target.Position.X ?
                        AnimationKey.CastLeft : AnimationKey.CastRight;
                    caster.Character.Entity.CurrentReplacedModel.ResetCurrentAnimation();
                }
                else
                {
                    caster.Character.Sprite.PerformAnimation = true;
                    caster.Character.Sprite.CurrentAnimation = caster.Position.X > target.Position.X ?
                        AnimationKey.CastLeft : AnimationKey.CastRight;
                    caster.Character.Sprite.ResetCurrentAnimation();
                }
            }
            else
            {
                if (caster.Character.Entity.IsModelChanged && caster.Character.Entity.CurrentReplacedModel != null)
                {
                    caster.Character.Entity.CurrentReplacedModel.PerformAnimation = true;
                    caster.Character.Entity.CurrentReplacedModel.CurrentAnimation = caster.Character.IsDirectedRight ?
                            AnimationKey.CastRight : AnimationKey.CastLeft;
                    caster.Character.Entity.CurrentReplacedModel.ResetCurrentAnimation();
                }
                else
                {
                    caster.Character.Sprite.PerformAnimation = true;
                    caster.Character.Sprite.CurrentAnimation = caster.Character.IsDirectedRight ?
                            AnimationKey.CastRight : AnimationKey.CastLeft;
                    caster.Character.Sprite.ResetCurrentAnimation();
                }
            }
            #endregion

            #region If spell has projectile
            if (SpriteSpellManager.TryGetValue(spell.Name, out sprite))
            {
                string soundPath;
                if (OnCastSoundManager.TryGetValue(spell.Name, out soundPath))
                    GameRef.Content.Load<SoundEffect>(soundPath).Play(0.8f,0f,0f);
                if (spellData.SpellTargetMode == SpellTargetMode.Target)
                {
                    TargetingProjectiles.Add(new TargetingProjectile(sprite, spell, 15, caster, caster.Character.Target, GameRef));
                    spell.SpellCooldown.Apply(TimeSpan.Zero);
                    PlayerUiRef.SkillEvents.Add(new SkillEvent(spell.Name, 0.5f, caster, 50));
                    return true;
                }
                else
                    StaticProjectiles.Add(new StaticProjectile(sprite, caster, GameRef));
            }
            #endregion

            PlayerUiRef.SkillEvents.Add(new SkillEvent(spell.Name, 0.5f, caster, 50));
            ApplyConfirmedSpell(caster, spell, spellData, caster.Character.Target, false, false, false);
            return true;
        }
        private void ApplyConfirmedSpell(ITargetable caster, Spell spell, SpellData spellData,
            ITargetable target, bool fromSpellCast, bool fromProjectile, bool fromScript)
        {
            AnimatedSprite sprite;
            Buff newBuff;
            ProjectileBinding binding;
            SpellModificationInformation spellInfo;
            // Get spell modifiers from caster
            spellInfo = new SpellModificationInformation(spellData.Name);
            spellInfo.HasteRatingAddMod = caster.Character.Entity.HasteRating;
            spellInfo.CriticalDamageMultiplierAddMod = caster.Character.Entity.CritDamageMultiplier;
            spellInfo.CriticalChanceAddMod = caster.Character.Entity.CritChance;
            // Apply spell modifiers from spell
            foreach (SpellModifier modifier in spell.Modifiers)
                modifier.ModifySpell(caster, target, spellInfo, spell);
            spellInfo.IsCrit = Mechanics.Roll(0, 10000) < (int)spellInfo.CriticalChanceAddMod * 100;
            // Apply cooldown if not from projectile
            if (!fromProjectile)
            {
                spell.SpellCooldown.Apply(TimeSpan.Zero);
            }
            // Check triggers
            foreach (SpellTrigger trigger in spell.Triggers)
            {
                if (trigger.CheckTrigger(caster, caster.Character.Target, spellInfo, spell))
                {
                    SpellData newSpellData;
                    SpellManager.TryGetValue(trigger.TriggeredSpellId, out newSpellData);
                    ApplyConfirmedSpell(caster, new Spell(newSpellData.ID, newSpellData.Name, newSpellData.SpellCooldown), newSpellData, caster.Character.Target, false, false, true);
                }
            }
            spell.Triggers.RemoveAll(item => item.NeedRemoval);
            // Remove triggered modifiers and buffs
            spell.Modifiers.RemoveAll(item => item.NeedRemoval);
            caster.Character.Entity.Buffs.RemoveAll(item => item.NeedsRemoval);
            // Apply spell effects
            foreach (BaseEffect effect in spellData.Effects)
                if (effect.AoeMode == AoeMode.None)
                    if (spellData.SpellTargetMode == SpellTargetMode.Target)
                    {
                        if (!target.Character.Entity.IsInvulnerable)
                            effect.Apply(caster, target, TimeSpan.Zero, spellInfo, PlayerUiRef);
                    }
                    else
                    {
                        effect.Apply(caster, caster, TimeSpan.Zero, spellInfo, PlayerUiRef);
                    }
                else
                    ApplyAoeEffect(caster, target, effect as AoeEffect, spellInfo, spellData);
            // Get triggered scripts
            List<BasicScript> newScripts = new List<BasicScript>();
            foreach (Buff buff in caster.Character.Entity.Buffs)
            {
                buff.UpdateScriptTriggers(TimeSpan.Zero, null, spellData, caster);
                foreach (BasicScript script in buff.Scripts)
                    if (script.Triggered)
                        newScripts.Add(script);
            }
            // Apply triggered scripts
            foreach (BasicScript script in newScripts)
            {
                SpellData newSpellData;
                if (!SpellManager.TryGetValue(spell.SpellDataId, out newSpellData))
                    ApplyConfirmedSpell(caster, script.Spell, newSpellData, target, false, false, true);
            }
            // Remove triggered buffs
            caster.Character.Entity.Buffs.RemoveAll(item => item.NeedsRemoval == true);
            // Apply buff if exists
            if (spellData.Buff != null)
            {
                newBuff = (Buff)spellData.Buff.Clone();
                newBuff.CasterRef = caster;
                if (newBuff.AoeMode == AoeMode.None)
                {
                    if (spellData.SpellTargetMode == SpellTargetMode.Target)
                    {
                        if (!(newBuff.BuffType == BuffType.Debuff && target.Character.Entity.IsInvulnerable))
                        {
                            newBuff = newBuff.Apply(target);
                            if (SpriteSpellHitManager.TryGetValue(spellData.Name, out sprite))
                            {
                                if (ProjectileBindingManager.TryGetValue(newBuff.Name, out binding))
                                    StaticProjectiles.Add(new BuffStateProjectile(sprite,
                                        target, GameRef, newBuff, binding));
                                else
                                    StaticProjectiles.Add(new BuffStateProjectile(sprite, target,
                                        GameRef, newBuff, ProjectileBinding.CenterToCenter));
                            }
                        }
                    }
                    else
                    {
                        newBuff = newBuff.Apply(caster);
                        if (SpriteSpellHitManager.TryGetValue(spellData.Name, out sprite))
                            if (ProjectileBindingManager.TryGetValue(newBuff.Name, out binding))
                                StaticProjectiles.Add(new BuffStateProjectile(sprite,
                                    caster, GameRef, newBuff, binding));
                            else
                                StaticProjectiles.Add(new BuffStateProjectile(sprite, caster,
                                    GameRef, newBuff, ProjectileBinding.CenterToCenter));
                    }
                }
                else
                    ApplyAoeBuff(caster, newBuff);
            }
            // Play sound if exists
            string soundPath;
            if (fromSpellCast || fromScript)
            {
                if (OnHitSoundManager.TryGetValue(spellData.Name, out soundPath))
                    GameRef.Content.Load<SoundEffect>(soundPath).Play(0.8f,0f,0f);
            }
            else 
            {
                if (OnCastSoundManager.TryGetValue(spellData.Name, out soundPath))
                    GameRef.Content.Load<SoundEffect>(soundPath).Play(0.8f,0f,0f);
            }
        }
        
        private void ApplyAoeBuff(ITargetable caster, Buff buff)
        {
            Vector2 targetCenter;
            AnimatedSprite sprite;
            ProjectileBinding binding;
            Buff newBuff;

            switch (buff.AoeMode)
            {
                case AoeMode.Cone:
                    targetCenter = caster.BoundRect.BottomCenter;
                    if (caster.Character.IsDirectedRight)
                        targetCenter.X += buff.Radius/2;
                    else
                        targetCenter.X -= buff.Radius/2;
                    break;
                case AoeMode.Self:
                    targetCenter = caster.BoundRect.BottomCenter;
                    break;
                case AoeMode.Cleave:
                    if (caster.Character.Target != null)
                        targetCenter = caster.Character.Target.BoundRect.BottomCenter;
                    else
                        targetCenter = caster.BoundRect.BottomCenter;
                    break;
                default:
                    targetCenter = caster.BoundRect.BottomCenter;
                    break;
            }

            BoundRectangle targetZone = new BoundRectangle();
            targetZone.X = targetCenter.X - buff.Radius;
            targetZone.Y = targetCenter.Y - caster.BoundRect.Height;
            targetZone.Height = 2 * caster.BoundRect.Height;
            targetZone.Width = 2 * buff.Radius;

            foreach (NetworkPlayer player in Characters)
            {
                if (buff.BuffType == BuffType.Debuff && (player.Team == caster.Team || player.Character.Entity.IsInvulnerable))
                    continue;
                if (buff.BuffType != BuffType.Debuff && (player.Team != caster.Team))
                    continue;
                if (player.BoundRect.BottomCenter.X > targetZone.Left && player.BoundRect.BottomCenter.X < targetZone.Right && player.BoundRect.BottomCenter.Y > targetZone.Top && player.BoundRect.BottomCenter.Y < targetZone.Bottom)
                {
                    if (!(buff.BuffType == BuffType.Debuff && player.Character.Entity.IsInvulnerable))
                    {
                        newBuff = buff.Apply(player);
                        if (SpriteSpellHitManager.TryGetValue(buff.Name, out sprite))
                            if (ProjectileBindingManager.TryGetValue(newBuff.Name, out binding))
                                StaticProjectiles.Add(new BuffStateProjectile(sprite, player, GameRef, newBuff, binding));
                            else
                                StaticProjectiles.Add(new BuffStateProjectile(sprite, player, GameRef, newBuff, ProjectileBinding.CenterToCenter));
                    }
                }
            }

            for (int j = Math.Max(0, (int)(targetZone.Left / 200) - 1); j < Math.Min((int)(targetZone.Right / 200) + 1, PartitionedCreatures.GetLength(1)); j++)
                for (int i = Math.Max(0, (int)(targetZone.Top / 200) - 1); i < Math.Min((int)(targetZone.Bottom / 200) + 1, PartitionedCreatures.GetLength(0)); i++)
                    foreach (Creature monster in PartitionedCreatures[i, j])
                    {
                        if (buff.BuffType == BuffType.Debuff && (monster.Team == caster.Team || monster.Character.Entity.IsInvulnerable))
                            continue;
                        if (buff.BuffType != BuffType.Debuff && (monster.Team != caster.Team))
                            continue;
                        if (monster.BoundRect.BottomCenter.X > targetZone.Left && monster.BoundRect.BottomCenter.X < targetZone.Right
                            && monster.BoundRect.BottomCenter.Y > targetZone.Top && monster.BoundRect.BottomCenter.Y < targetZone.Bottom)
                        {
                            newBuff = buff.Apply(monster);
                            if (SpriteSpellHitManager.TryGetValue(buff.Name, out sprite))
                            {
                                if (ProjectileBindingManager.TryGetValue(newBuff.Name, out binding))
                                    StaticProjectiles.Add(new BuffStateProjectile(sprite, monster, GameRef, newBuff, binding));
                                else
                                    StaticProjectiles.Add(new BuffStateProjectile(sprite, monster, GameRef, newBuff, ProjectileBinding.CenterToCenter));
                            }
                        }
                    }
        }
        private void ApplyAoeEffect(ITargetable caster, ITargetable target, AoeEffect aoeEffect, SpellModificationInformation spellInfo, SpellData spellData)
        {
            Vector2 targetCenter;
            switch (aoeEffect.AoeMode)
            {
                case AoeMode.Cone:
                    targetCenter = caster.BoundRect.BottomCenter;
                    if (caster.Character.IsDirectedRight)
                        targetCenter.X += aoeEffect.Radius;
                    else
                        targetCenter.X -= aoeEffect.Radius;
                    break;
                case AoeMode.Self:
                    targetCenter = caster.BoundRect.BottomCenter;
                    break;
                case AoeMode.Cleave:
                    if (target != null)
                        targetCenter = target.BoundRect.BottomCenter;
                    else if (caster.Character.Target != null)
                        targetCenter = caster.Character.Target.BoundRect.BottomCenter;
                    else
                        targetCenter = caster.BoundRect.BottomCenter;
                    break;
                default:
                    targetCenter = Vector2.Zero;
                    break;
            }
            BoundRectangle targetZone = new BoundRectangle();
            targetZone.X = targetCenter.X - aoeEffect.Radius;
            targetZone.Y = targetCenter.Y - caster.BoundRect.Height;
            targetZone.Height = 2 * caster.BoundRect.Height;
            targetZone.Width = 2 * aoeEffect.Radius;

            foreach (NetworkPlayer player in Characters)
            {
                if (spellData.IsHarmful && (player.Team == caster.Team || player.Character.Entity.IsInvulnerable))
                    continue;
                if (!spellData.IsHarmful && (player.Team != caster.Team))
                    continue;
                if (player.BoundRect.BottomCenter.X > targetZone.Left && player.BoundRect.BottomCenter.X < targetZone.Right
                    && player.BoundRect.BottomCenter.Y > targetZone.Top && player.BoundRect.BottomCenter.Y < targetZone.Bottom)
                    aoeEffect.Effect.Apply(caster, player, TimeSpan.Zero, spellInfo, PlayerUiRef);
            }

            for (int j = Math.Max(0, (int)(targetZone.Left / 200) - 1); j < Math.Min((int)(targetZone.Right / 200) + 1, PartitionedCreatures.GetLength(1)); j++)
                for (int i = Math.Max(0, (int)(targetZone.Top / 200) - 1); i < Math.Min((int)(targetZone.Bottom / 200) + 1, PartitionedCreatures.GetLength(0)); i++)
                    foreach (Creature monster in PartitionedCreatures[i, j])
                    {
                        if (spellData.IsHarmful && (monster.Team == caster.Team || monster.Character.Entity.IsInvulnerable))
                            continue;
                        if (!spellData.IsHarmful && (monster.Team != caster.Team))
                            continue;
                        if (monster.BoundRect.BottomCenter.X > targetZone.Left && monster.BoundRect.BottomCenter.X < targetZone.Right &&
                            monster.BoundRect.BottomCenter.Y > targetZone.Top && monster.BoundRect.BottomCenter.Y < targetZone.Bottom)
                            aoeEffect.Effect.Apply(caster, monster, TimeSpan.Zero, spellInfo, PlayerUiRef);
                    }
        }
        private void ApplyGlobalCooldown(ITargetable caster, Spell spell, SpellData spellData, TimeSpan latency)
        {
            switch (spellData.SpellGlobal)
            {
                case SpellGlobalCooldownMode.Normal:
                    caster.Character.GlobalCooldown.ApplyModified(latency, caster.Character.Entity.HasteRating);
                    break;
                case SpellGlobalCooldownMode.NoCooldown:
                    break;
            }
        }
        private bool CheckSpellTargetMode(ITargetable caster, SpellData spellData)
        {
            switch (spellData.SpellTargetMode)
            {
                case SpellTargetMode.Target:
                    if (caster.Character.Target == null)
                        return false;
                    if (spellData.IsHarmful && caster.Character.Target.Team == caster.Team)
                        return false;
                    if (!spellData.IsHarmful && caster.Character.Target.Team != caster.Team)
                        return false;
                    if (Math.Pow(caster.Position.X - caster.Character.Target.Position.X, 2)
                        + Math.Pow(caster.Position.Y - caster.Character.Target.Position.Y, 2)
                        > Math.Pow(spellData.Range, 2))
                        return false;
                    break;
                case SpellTargetMode.Self:
                default:
                    break;
            }
            return true;
        }
        private bool CheckSpellCooldown(ITargetable caster, Spell spell, SpellData spellData)
        {
            if (!spell.SpellCooldown.NoCooldown)
                return false;

            if (!caster.Character.GlobalCooldown.NoCooldown &&
                spellData.SpellGlobal != SpellGlobalCooldownMode.NoCooldown)
                return false;

            return true;
        }
        private bool CheckSpellCastType(ITargetable caster, Spell spell, SpellData spellData, out bool IsCastApplied)
        {
            switch (spellData.SpellCastType)
            {
                case SpellCastType.Casted:
                    if (caster.Character.SpellCast != null)
                    {
                        IsCastApplied = false;
                        return false;
                    }
                    else
                    {
                        string soundPath;
                        SpellModificationInformation spellInfo = new SpellModificationInformation(spell.Name);
                        foreach (SpellModifier modifier in spell.Modifiers)
                            if (modifier.CheckForInstantCast(caster, caster.Character.Target, spellInfo, spell))
                            {
                                IsCastApplied = true;
                                if (OnHitSoundManager.TryGetValue(spell.Name, out soundPath))
                                    GameRef.Content.Load<SoundEffect>(soundPath).Play(0.8f,0f,0f);
                                return true;
                            }

                        foreach (SpellModifier modifier in spell.Modifiers)
                            modifier.ModifySpell(caster, caster.Character.Target, spellInfo, spell);

                        spell.Modifiers.RemoveAll(item => item.NeedRemoval);
                        caster.Character.Entity.Buffs.RemoveAll(item => item.NeedsRemoval);

                        caster.Character.SpellCast = new SpellCast(spell, spellData, caster, caster.Character.Target, spellInfo, TimeSpan.Zero);

                        ApplyGlobalCooldown(caster, spell, spellData, TimeSpan.Zero);

                        if (OnCastSoundManager.TryGetValue(spell.Name, out soundPath))
                            GameRef.Content.Load<SoundEffect>(soundPath).Play(0.8f,0f,0f);
                        IsCastApplied = true;
                        PlayerUiRef.SkillEvents.Add(new SkillEvent(spell.Name, 0.5f, caster, 50));
                        return false;
                    }
                case SpellCastType.Instant:
                    if (caster.Character.SpellCast != null)
                    {
                        IsCastApplied = false;
                        return false;
                    }
                    else
                    {
                        ApplyGlobalCooldown(caster, spell, spellData, TimeSpan.Zero);
                    }
                    break;
                default:
                    break;
            }
            IsCastApplied = true;
            return true;
        }
        private bool CharacterDied(Creature character)
        {
            if (character.Character.Entity.Health.CurrentValue == 0)
            {
                character.Dispose();
                return true;
            }
            return false;
        }
        private void CharacterAnimationHandle(ITargetable target)
        {
            AnimatedSprite sprite;
            if (target.Character.Entity.IsModelChanged && target.Character.Entity.CurrentReplacedModel != null)
                sprite = target.Character.Entity.CurrentReplacedModel;
            else
                sprite = target.Character.Sprite;

            if (!sprite.PerformAnimation)
            {
                if (target.Character.SpellCast != null)
                {
                    if (target.Velocity.X > 0)
                        sprite.CurrentAnimation = AnimationKey.RunCastingRight;
                    else if (target.Velocity.X < 0)
                        sprite.CurrentAnimation = AnimationKey.RunCastingLeft;
                    else
                    {
                        if (target.Velocity.Y == 0)
                        {
                            if (target.Character.IsDirectedRight)
                                sprite.CurrentAnimation = AnimationKey.CastingRight;
                            else
                                sprite.CurrentAnimation = AnimationKey.CastingLeft;
                        }
                        else
                        {
                            if (target.Character.IsDirectedRight)
                                sprite.CurrentAnimation = AnimationKey.RunCastingRight;
                            else
                                sprite.CurrentAnimation = AnimationKey.RunCastingLeft;
                        }
                    }
                }
                else
                {
                    if (target.Velocity.X > 0)
                        sprite.CurrentAnimation = AnimationKey.Right;
                    else if (target.Velocity.X < 0)
                        sprite.CurrentAnimation = AnimationKey.Left;
                    else
                    {
                        if (target.Velocity.Y == 0)
                        {
                            if (target.Character.IsDirectedRight)
                                sprite.CurrentAnimation = AnimationKey.StandRight;
                            else
                                sprite.CurrentAnimation = AnimationKey.StandLeft;
                        }
                        else
                        {
                            if (target.Character.IsDirectedRight)
                                sprite.CurrentAnimation = AnimationKey.Right;
                            else
                                sprite.CurrentAnimation = AnimationKey.Left;
                        }
                    }
                }
            }
            else
            {
                if (target.Velocity.X > 0)
                {
                    if (sprite.CurrentAnimation != AnimationKey.RunCastRight)
                    {
                        int aniframe = sprite.CurrentAnimationFrame;
                        sprite.CurrentAnimation = AnimationKey.RunCastRight;
                        sprite.CurrentAnimationFrame = aniframe;
                    }
                }
                else if (target.Velocity.X < 0)
                {
                    if (sprite.CurrentAnimation != AnimationKey.RunCastLeft)
                    {
                        int aniframe = sprite.CurrentAnimationFrame;
                        sprite.CurrentAnimation = AnimationKey.RunCastLeft;
                        sprite.CurrentAnimationFrame = aniframe;
                    }
                }
                else
                {
                    int aniframe = sprite.CurrentAnimationFrame;
                    sprite.CurrentAnimation = target.Character.IsDirectedRight ? AnimationKey.CastRight : AnimationKey.CastLeft;
                    sprite.CurrentAnimationFrame = aniframe;
                }
            }
        }
        public ITargetable GetTarget(int x, int y)
        {
            foreach (NetworkPlayer player in Characters)
                if (player.BoundRect.Left - 30 < x && player.BoundRect.Right + 30 > x && player.BoundRect.Bottom > y && player.BoundRect.Top < y && !player.Character.Entity.IsInvisible)
                    return player;

            for (int j = Math.Max(0, x / 200 - 1); j <= Math.Min(x / 200 + 1, PartitionedCreatures.GetLength(1) - 1); j++)
            {
                for (int i = Math.Max(0, y / 200 - 1); i <= Math.Min(y / 200 + 1, PartitionedCreatures.GetLength(0) - 1); i++)
                {
                    foreach (Creature monster in PartitionedCreatures[i, j])
                    {
                        if (monster.BoundRect.Left - 30 < x && monster.BoundRect.Right + 30 > x && monster.BoundRect.Bottom > y && monster.BoundRect.Top < y && !monster.Character.Entity.IsInvisible)
                            return monster;
                    }
                }
            }
            return null;
        }
        public ITargetable GetNearestEnemyTarget(ITargetable whoTargets, int x, int y, List<string> previousTargets)
        {
            int jx = (int)MathHelper.Clamp(x / 200, 0, PartitionedCreatures.GetLength(1));
            int iy = (int)MathHelper.Clamp(y / 200, 0, PartitionedCreatures.GetLength(0));

            if (previousTargets.Count > 20)
                previousTargets.Clear();

            for (int cellRadius = 0; cellRadius < 8; cellRadius++)
            {
                for (int j = Math.Max(0, jx - cellRadius); j <= Math.Min(jx + cellRadius, PartitionedCreatures.GetLength(1) - 1); j++)
                {
                    for (int i = Math.Max(0, iy - cellRadius); i <= Math.Min(iy + cellRadius, PartitionedCreatures.GetLength(0) - 1); i++)
                    {
                        foreach (Creature monster in PartitionedCreatures[i, j])
                        {
                            if (!previousTargets.Contains(monster.Character.Entity.EntityName))
                            {
                                if (monster == whoTargets || monster.Character.Entity.IsInvisible || monster.Team == whoTargets.Team)
                                    continue;
                                previousTargets.Add(monster.Character.Entity.EntityName);
                                return monster;
                            }
                        }
                    }
                }
            }

            foreach (NetworkPlayer character in Characters)
                if (!previousTargets.Contains(character.Character.Entity.EntityName))
                {
                    if (character == whoTargets || character.Character.Entity.IsInvisible || character.Team == whoTargets.Team)
                        continue;
                    previousTargets.Add(character.Character.Entity.EntityName);
                    return character;
                }
            previousTargets.Clear();

            return null;
        }
    }
}