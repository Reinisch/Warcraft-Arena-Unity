using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Net;

using BasicRpgEngine.Characters;
using BasicRpgEngine.Spells;

namespace BasicRpgEngine
{
    public class NetworkPlayerInterface
    {
        Texture2D playerFrame;
        Texture2D targetFrame;
        Texture2D playerPortrait;
        Texture2D barFill;
        Texture2D barFillNoTrans;
        Texture2D uiParts;
        Texture2D castingBarSpark;
        Texture2D castingBar;
        Texture2D statusBar;
        SpriteFont coolDownFont;
        SpriteFont eventFont;
        ActionBar actionBar1;
        ActionBar actionBar2;

        public SpriteFont Font { get; set; }
        public SpriteFont HotkeyFont { get; set; }

        public Dictionary<string, Texture2D> Icons { get; private set; }
        public NetworkPlayer PlayerRef { get; private set; }
        public List<SkillEvent> SkillEvents { get; private set; }
        public List<SkillDamageEvent> SkillDamageEvents { get; private set; }

        public NetworkPlayerInterface(Game game, NetworkPlayer playerRef, string className)
        {
            SkillEvents = new List<SkillEvent>();
            SkillDamageEvents = new List<SkillDamageEvent>();
            PlayerRef = playerRef;

            #region Icons
            Icons = new Dictionary<string, Texture2D>();
            //Mage
            Icons.Add("Generic", game.Content.Load<Texture2D>(@"PlayerInterface\Icons\Mage\Spell_Arcane_Arcane04"));
            Icons.Add("Time Warp", game.Content.Load<Texture2D>(@"PlayerInterface\Icons\Mage\Ability_Mage_TimeWarp"));
            Icons.Add("Invisibility", game.Content.Load<Texture2D>(@"PlayerInterface\Icons\Mage\ability_mage_GreaterInvisibility"));
            Icons.Add("Molten Armor", game.Content.Load<Texture2D>(@"PlayerInterface\Icons\Mage\ABILITY_MAGE_MOLTENARMOR"));
            Icons.Add("Arcane Missile Barrage", game.Content.Load<Texture2D>(@"PlayerInterface\Icons\Mage\Ability_Mage_MissileBarrage"));
            Icons.Add("Pyroblast", game.Content.Load<Texture2D>(@"PlayerInterface\Icons\Mage\Spell_Fire_Fireball02"));
            Icons.Add("Path of Fire", game.Content.Load<Texture2D>(@"PlayerInterface\Icons\Mage\Ability_Mage_HotStreak"));
            Icons.Add("Pyroblast!", game.Content.Load<Texture2D>(@"PlayerInterface\Icons\Mage\Ability_Mage_Burnout"));
            Icons.Add("Blast Wave", game.Content.Load<Texture2D>(@"PlayerInterface\Icons\Mage\spell_holy_excorcism_02"));
            Icons.Add("Living Bomb", game.Content.Load<Texture2D>(@"PlayerInterface\Icons\Mage\Ability_Mage_LivingBomb"));
            Icons.Add("Alter Time", game.Content.Load<Texture2D>(@"PlayerInterface\Icons\Mage\Spell_Mage_AlterTime"));
            Icons.Add("Arcane Barrage", game.Content.Load<Texture2D>(@"PlayerInterface\Icons\Mage\Ability_Mage_ArcaneBarrage"));
            Icons.Add("Greater Invisibility", game.Content.Load<Texture2D>(@"PlayerInterface\Icons\Mage\ability_mage_GreaterInvisibility"));
            Icons.Add("Deep Freeze", game.Content.Load<Texture2D>(@"PlayerInterface\Icons\Mage\Ability_Mage_DeepFreeze"));
            Icons.Add("Fiery Payback", game.Content.Load<Texture2D>(@"PlayerInterface\Icons\Mage\Ability_Mage_FieryPayback"));
            Icons.Add("Mass Polymorph", game.Content.Load<Texture2D>(@"PlayerInterface\Icons\Mage\spell_nature_polymorph"));
            Icons.Add("Frost Fire Bolt", game.Content.Load<Texture2D>(@"PlayerInterface\Icons\Mage\Ability_Mage_FrostFireBolt"));
            Icons.Add("Frostjaw", game.Content.Load<Texture2D>(@"PlayerInterface\Icons\Mage\Ability_Mage_FrostJaw"));
            Icons.Add("Hot Streak", game.Content.Load<Texture2D>(@"PlayerInterface\Icons\Mage\Ability_Mage_HotStreak"));
            Icons.Add("Incanters Absorbtion", game.Content.Load<Texture2D>(@"PlayerInterface\Icons\Mage\Ability_Mage_IncantersAbsorbtion"));
            Icons.Add("Polymorph", game.Content.Load<Texture2D>(@"PlayerInterface\Icons\Mage\Ability_Mage_ImprovedPolymorph"));
            Icons.Add("Blazing Speed", game.Content.Load<Texture2D>(@"PlayerInterface\Icons\Mage\Spell_Fire_BurningSpeed"));
            Icons.Add("Fire Blast", game.Content.Load<Texture2D>(@"PlayerInterface\Icons\Mage\Spell_Fire_Fireball"));
            Icons.Add("Scorch", game.Content.Load<Texture2D>(@"PlayerInterface\Icons\Mage\Spell_Fire_SoulBurn"));
            Icons.Add("Ice Block", game.Content.Load<Texture2D>(@"PlayerInterface\Icons\Mage\Spell_Frost_Frost"));
            Icons.Add("Icy Veins", game.Content.Load<Texture2D>(@"PlayerInterface\Icons\Mage\Spell_Frost_ColdHearted"));
            Icons.Add("Ice Barrier", game.Content.Load<Texture2D>(@"PlayerInterface\Icons\Mage\spell_ice_lament"));
            Icons.Add("Ice Lance", game.Content.Load<Texture2D>(@"PlayerInterface\Icons\Mage\Spell_Frost_FrostBlast"));
            Icons.Add("Frost Nova", game.Content.Load<Texture2D>(@"PlayerInterface\Icons\Mage\Spell_Frost_FrostNova"));
            Icons.Add("Combustion", game.Content.Load<Texture2D>(@"PlayerInterface\Icons\Mage\Spell_Fire_SelfDestruct"));
            Icons.Add("Cone of Cold", game.Content.Load<Texture2D>(@"PlayerInterface\Icons\Mage\Spell_Frost_Glacier"));
            Icons.Add("Counter Spell", game.Content.Load<Texture2D>(@"PlayerInterface\Icons\Mage\Spell_Frost_IceShock"));
            Icons.Add("Blink", game.Content.Load<Texture2D>(@"PlayerInterface\Icons\Mage\Spell_Arcane_Blink"));
            Icons.Add("Wizard Mark", game.Content.Load<Texture2D>(@"PlayerInterface\Icons\Mage\Spell_Frost_WizardMark"));
            Icons.Add("Mage Armor", game.Content.Load<Texture2D>(@"PlayerInterface\Icons\Mage\Spell_Magic_MageArmor"));
            Icons.Add("Inferno Blast", game.Content.Load<Texture2D>(@"PlayerInterface\Icons\Mage\Spell_Mage_InfernoBlast"));
            Icons.Add("Frost Bolt", game.Content.Load<Texture2D>(@"PlayerInterface\Icons\Mage\Spell_Frost_FrostBolt02"));
            Icons.Add("Heating Up!", game.Content.Load<Texture2D>(@"PlayerInterface\Icons\Mage\Ability_Mage_FieryPayback"));
            //Warrior
            Icons.Add("Charge", game.Content.Load<Texture2D>(@"PlayerInterface\Icons\Warrior\Ability_Warrior_Charge"));
            Icons.Add("Throwdown", game.Content.Load<Texture2D>(@"PlayerInterface\Icons\Warrior\Ability_Warrior_Throwdown"));
            Icons.Add("Shockwave", game.Content.Load<Texture2D>(@"PlayerInterface\Icons\Warrior\Ability_Warrior_Shockwave"));
            Icons.Add("Bleed", game.Content.Load<Texture2D>(@"PlayerInterface\Icons\Warrior\Ability_Warrior_BloodBath"));
            Icons.Add("Battle Shout", game.Content.Load<Texture2D>(@"PlayerInterface\Icons\Warrior\Ability_Warrior_RallyingCry"));
            Icons.Add("Thunder Clap", game.Content.Load<Texture2D>(@"PlayerInterface\Icons\Warrior\Warrior_talent_icon_BloodandThunder"));
            Icons.Add("Hamstring", game.Content.Load<Texture2D>(@"PlayerInterface\Icons\Warrior\Ability_ShockWave"));
            Icons.Add("Mortal Strike", game.Content.Load<Texture2D>(@"PlayerInterface\Icons\Warrior\Ability_Warrior_SavageBlow"));
            Icons.Add("Shield Wall", game.Content.Load<Texture2D>(@"PlayerInterface\Icons\Warrior\Ability_Warrior_ShieldWall"));
            Icons.Add("Avatar", game.Content.Load<Texture2D>(@"PlayerInterface\Icons\Warrior\Warrior_talent_icon_Avatar"));
            Icons.Add("Cleave", game.Content.Load<Texture2D>(@"PlayerInterface\Icons\Warrior\Ability_Warrior_Cleave"));           
            //Priest
            Icons.Add("Shadow Form", game.Content.Load<Texture2D>(@"PlayerInterface\Icons\Priest\Ability_Priest_Darkness"));
            Icons.Add("Psychic Scream", game.Content.Load<Texture2D>(@"PlayerInterface\Icons\Priest\Spell_Shadow_PsychicScream"));
            Icons.Add("Silence", game.Content.Load<Texture2D>(@"PlayerInterface\Icons\Priest\Ability_Priest_Silence"));
            Icons.Add("Mind Spike", game.Content.Load<Texture2D>(@"PlayerInterface\Icons\Priest\Spell_Priest_Mindspike"));
            Icons.Add("Pain Suppression", game.Content.Load<Texture2D>(@"PlayerInterface\Icons\Priest\Spell_Holy_PainSupression"));
            Icons.Add("Greater Heal", game.Content.Load<Texture2D>(@"PlayerInterface\Icons\Priest\Spell_Holy_GreaterHeal"));
            Icons.Add("Phantasm", game.Content.Load<Texture2D>(@"PlayerInterface\Icons\Priest\Ability_Priest_Phantasm"));
            Icons.Add("Vampiric Touch", game.Content.Load<Texture2D>(@"PlayerInterface\Icons\Priest\Spell_Holy_Stoicism"));
            Icons.Add("Renew", game.Content.Load<Texture2D>(@"PlayerInterface\Icons\Priest\Spell_Holy_Renew"));
            Icons.Add("Mind Blast", game.Content.Load<Texture2D>(@"PlayerInterface\Icons\Priest\Spell_Shadow_UnholyFrenzy"));
            Icons.Add("Shadow Word: Pain", game.Content.Load<Texture2D>(@"PlayerInterface\Icons\Priest\Spell_Shadow_ShadowWordPain"));
            Icons.Add("Divine Insight", game.Content.Load<Texture2D>(@"PlayerInterface\Icons\Priest\Spell_Priest_BurningWill"));
            Icons.Add("Divine Insight Shadow", game.Content.Load<Texture2D>(@"PlayerInterface\Icons\Priest\Spell_Shadow_Skull"));
            Icons.Add("Shadow Word: Insanity", game.Content.Load<Texture2D>(@"PlayerInterface\Icons\Priest\Spell_Shadow_MindFlay"));
            #endregion

            if (className == "Mage")
            {
                #region Action Bar 1
                actionBar1 = new ActionBar(game, this, new Point(340, 665));
                Spell spell;

                PlayerRef.Character.Entity.Spells.TryGetValue("Polymorph", out spell);
                actionBar1.Buttons[0] = new ActionBarButton(spell, Keys.C, Keys.None);

                PlayerRef.Character.Entity.Spells.TryGetValue("Counter Spell", out spell);
                actionBar1.Buttons[1] = new ActionBarButton(spell, Keys.X, Keys.None);

                PlayerRef.Character.Entity.Spells.TryGetValue("Frost Nova", out spell);
                actionBar1.Buttons[2] = new ActionBarButton(spell, Keys.D4, Keys.None);

                PlayerRef.Character.Entity.Spells.TryGetValue("Ice Barrier", out spell);
                actionBar1.Buttons[3] = new ActionBarButton(spell, Keys.F, Keys.LeftControl);

                PlayerRef.Character.Entity.Spells.TryGetValue("Frost Bolt", out spell);
                actionBar1.Buttons[4] = new ActionBarButton(spell, Keys.D1, Keys.None);

                PlayerRef.Character.Entity.Spells.TryGetValue("Deep Freeze", out spell);
                actionBar1.Buttons[5] = new ActionBarButton(spell, Keys.F, Keys.None);

                PlayerRef.Character.Entity.Spells.TryGetValue("Frostjaw", out spell);
                actionBar1.Buttons[6] = new ActionBarButton(spell, Keys.D2, Keys.LeftControl);

                PlayerRef.Character.Entity.Spells.TryGetValue("Ice Lance", out spell);
                actionBar1.Buttons[7] = new ActionBarButton(spell, Keys.R, Keys.LeftControl);

                PlayerRef.Character.Entity.Spells.TryGetValue("Frost Fire Bolt", out spell);
                actionBar1.Buttons[8] = new ActionBarButton(spell, Keys.E, Keys.LeftControl);

                PlayerRef.Character.Entity.Spells.TryGetValue("Combustion", out spell);
                actionBar1.Buttons[9] = new ActionBarButton(spell, Keys.D4, Keys.LeftControl);

                PlayerRef.Character.Entity.Spells.TryGetValue("Scorch", out spell);
                actionBar1.Buttons[10] = new ActionBarButton(spell, Keys.D2, Keys.None);

                PlayerRef.Character.Entity.Spells.TryGetValue("Pyroblast", out spell);
                actionBar1.Buttons[11] = new ActionBarButton(spell, Keys.E, Keys.None);

                PlayerRef.Character.Entity.Spells.TryGetValue("Blast Wave", out spell);
                actionBar1.Buttons[12] = new ActionBarButton(spell, Keys.D3, Keys.LeftControl);

                PlayerRef.Character.Entity.Spells.TryGetValue("Blazing Speed", out spell);
                actionBar1.Buttons[13] = new ActionBarButton(spell, Keys.X, Keys.LeftControl);
                #endregion

                #region Action Bar 2
                actionBar2 = new ActionBar(game, this, new Point(340, 715));

                PlayerRef.Character.Entity.Spells.TryGetValue("Cone of Cold", out spell);
                actionBar2.Buttons[0] = new ActionBarButton(spell, Keys.D3, Keys.None);

                PlayerRef.Character.Entity.Spells.TryGetValue("Mass Polymorph", out spell);
                actionBar2.Buttons[1] = new ActionBarButton(spell, Keys.C, Keys.LeftShift);

                PlayerRef.Character.Entity.Spells.TryGetValue("Living Bomb", out spell);
                actionBar2.Buttons[2] = new ActionBarButton(spell, Keys.R, Keys.None);

                PlayerRef.Character.Entity.Spells.TryGetValue("Fire Blast", out spell);
                actionBar2.Buttons[3] = new ActionBarButton(spell, Keys.D5, Keys.None);

                PlayerRef.Character.Entity.Spells.TryGetValue("Icy Veins", out spell);
                actionBar2.Buttons[4] = new ActionBarButton(spell, Keys.D4, Keys.LeftShift);

                PlayerRef.Character.Entity.Spells.TryGetValue("Blink", out spell);
                actionBar2.Buttons[5] = new ActionBarButton(spell, Keys.Q, Keys.None);

                PlayerRef.Character.Entity.Spells.TryGetValue("Path of Fire", out spell);
                actionBar2.Buttons[6] = new ActionBarButton(spell, Keys.D2, Keys.LeftShift);

                PlayerRef.Character.Entity.Spells.TryGetValue("Combustion", out spell);
                actionBar2.Buttons[7] = new ActionBarButton(spell, Keys.D4, Keys.LeftControl);

                PlayerRef.Character.Entity.Spells.TryGetValue("Scorch", out spell);
                actionBar2.Buttons[8] = new ActionBarButton(spell, Keys.D2, Keys.None);

                PlayerRef.Character.Entity.Spells.TryGetValue("Pyroblast", out spell);
                actionBar2.Buttons[9] = new ActionBarButton(spell, Keys.E, Keys.None);

                PlayerRef.Character.Entity.Spells.TryGetValue("Invisibility", out spell);
                actionBar2.Buttons[10] = new ActionBarButton(spell, Keys.F, Keys.LeftShift);

                PlayerRef.Character.Entity.Spells.TryGetValue("Ice Block", out spell);
                actionBar2.Buttons[11] = new ActionBarButton(spell, Keys.D5, Keys.LeftShift);

                PlayerRef.Character.Entity.Spells.TryGetValue("Frostjaw", out spell);
                actionBar2.Buttons[12] = new ActionBarButton(spell, Keys.D2, Keys.LeftControl);

                PlayerRef.Character.Entity.Spells.TryGetValue("Ice Lance", out spell);
                actionBar2.Buttons[13] = new ActionBarButton(spell, Keys.R, Keys.LeftControl);
                #endregion
            }
            else if (className == "Warrior")
            {
                #region Action Bar 1
                actionBar1 = new ActionBar(game, this, new Point(340, 665));
                Spell spell;

                PlayerRef.Character.Entity.Spells.TryGetValue("Polymorph", out spell);
                actionBar1.Buttons[0] = new ActionBarButton(spell, Keys.C, Keys.None);

                PlayerRef.Character.Entity.Spells.TryGetValue("Counter Spell", out spell);
                actionBar1.Buttons[1] = new ActionBarButton(spell, Keys.X, Keys.None);

                PlayerRef.Character.Entity.Spells.TryGetValue("Frost Nova", out spell);
                actionBar1.Buttons[2] = new ActionBarButton(spell, Keys.D4, Keys.None);

                PlayerRef.Character.Entity.Spells.TryGetValue("Ice Barrier", out spell);
                actionBar1.Buttons[3] = new ActionBarButton(spell, Keys.F, Keys.LeftControl);

                PlayerRef.Character.Entity.Spells.TryGetValue("Frost Bolt", out spell);
                actionBar1.Buttons[4] = new ActionBarButton(spell, Keys.D1, Keys.None);

                PlayerRef.Character.Entity.Spells.TryGetValue("Deep Freeze", out spell);
                actionBar1.Buttons[5] = new ActionBarButton(spell, Keys.F, Keys.None);

                PlayerRef.Character.Entity.Spells.TryGetValue("Frostjaw", out spell);
                actionBar1.Buttons[6] = new ActionBarButton(spell, Keys.D2, Keys.LeftControl);

                PlayerRef.Character.Entity.Spells.TryGetValue("Ice Lance", out spell);
                actionBar1.Buttons[7] = new ActionBarButton(spell, Keys.R, Keys.LeftControl);

                PlayerRef.Character.Entity.Spells.TryGetValue("Avatar", out spell);
                actionBar1.Buttons[8] = new ActionBarButton(spell, Keys.D4, Keys.LeftControl);

                PlayerRef.Character.Entity.Spells.TryGetValue("Shield Wall", out spell);
                actionBar1.Buttons[9] = new ActionBarButton(spell, Keys.D5, Keys.LeftShift);

                PlayerRef.Character.Entity.Spells.TryGetValue("Scorch", out spell);
                actionBar1.Buttons[10] = new ActionBarButton(spell, Keys.D2, Keys.None);

                PlayerRef.Character.Entity.Spells.TryGetValue("Pyroblast", out spell);
                actionBar1.Buttons[11] = new ActionBarButton(spell, Keys.E, Keys.None);

                PlayerRef.Character.Entity.Spells.TryGetValue("Blast Wave", out spell);
                actionBar1.Buttons[12] = new ActionBarButton(spell, Keys.D3, Keys.LeftControl);

                PlayerRef.Character.Entity.Spells.TryGetValue("Charge", out spell);
                actionBar1.Buttons[13] = new ActionBarButton(spell, Keys.X, Keys.LeftControl);
                #endregion

                #region Action Bar 2
                actionBar2 = new ActionBar(game, this, new Point(340, 715));

                PlayerRef.Character.Entity.Spells.TryGetValue("Cone of Cold", out spell);
                actionBar2.Buttons[0] = new ActionBarButton(spell, Keys.D3, Keys.None);

                PlayerRef.Character.Entity.Spells.TryGetValue("Cleave", out spell);
                actionBar2.Buttons[1] = new ActionBarButton(spell, Keys.C, Keys.LeftShift);

                PlayerRef.Character.Entity.Spells.TryGetValue("Living Bomb", out spell);
                actionBar2.Buttons[2] = new ActionBarButton(spell, Keys.R, Keys.None);

                PlayerRef.Character.Entity.Spells.TryGetValue("Fire Blast", out spell);
                actionBar2.Buttons[3] = new ActionBarButton(spell, Keys.D5, Keys.None);

                PlayerRef.Character.Entity.Spells.TryGetValue("Icy Veins", out spell);
                actionBar2.Buttons[4] = new ActionBarButton(spell, Keys.D4, Keys.LeftShift);

                PlayerRef.Character.Entity.Spells.TryGetValue("Blink", out spell);
                actionBar2.Buttons[5] = new ActionBarButton(spell, Keys.Q, Keys.None);

                PlayerRef.Character.Entity.Spells.TryGetValue("Path of Fire", out spell);
                actionBar2.Buttons[6] = new ActionBarButton(spell, Keys.D2, Keys.LeftShift);

                PlayerRef.Character.Entity.Spells.TryGetValue("Thunder Clap", out spell);
                actionBar2.Buttons[7] = new ActionBarButton(spell, Keys.Z, Keys.LeftShift);

                PlayerRef.Character.Entity.Spells.TryGetValue("Throwdown", out spell);
                actionBar2.Buttons[8] = new ActionBarButton(spell, Keys.Z, Keys.None);

                PlayerRef.Character.Entity.Spells.TryGetValue("Hamstring", out spell);
                actionBar2.Buttons[9] = new ActionBarButton(spell, Keys.Q, Keys.LeftControl);

                PlayerRef.Character.Entity.Spells.TryGetValue("Battle Shout", out spell);
                actionBar2.Buttons[10] = new ActionBarButton(spell, Keys.T, Keys.LeftShift);

                PlayerRef.Character.Entity.Spells.TryGetValue("Shockwave", out spell);
                actionBar2.Buttons[11] = new ActionBarButton(spell, Keys.F, Keys.LeftShift);

                PlayerRef.Character.Entity.Spells.TryGetValue("Bleed", out spell);
                actionBar2.Buttons[12] = new ActionBarButton(spell, Keys.G, Keys.LeftShift);

                PlayerRef.Character.Entity.Spells.TryGetValue("Mortal Strike", out spell);
                actionBar2.Buttons[13] = new ActionBarButton(spell, Keys.E, Keys.LeftShift);
                #endregion
            }
            else if (className == "Priest")
            {
                #region Action Bar 1
                actionBar1 = new ActionBar(game, this, new Point(340, 665));
                Spell spell;

                PlayerRef.Character.Entity.Spells.TryGetValue("Shadow Word: Insanity", out spell);
                actionBar1.Buttons[0] = new ActionBarButton(spell, Keys.C, Keys.None);

                PlayerRef.Character.Entity.Spells.TryGetValue("Silence", out spell);
                actionBar1.Buttons[1] = new ActionBarButton(spell, Keys.X, Keys.None);

                PlayerRef.Character.Entity.Spells.TryGetValue("Frost Nova", out spell);
                actionBar1.Buttons[2] = new ActionBarButton(spell, Keys.D4, Keys.None);

                PlayerRef.Character.Entity.Spells.TryGetValue("Ice Barrier", out spell);
                actionBar1.Buttons[3] = new ActionBarButton(spell, Keys.F, Keys.LeftControl);

                PlayerRef.Character.Entity.Spells.TryGetValue("Frost Bolt", out spell);
                actionBar1.Buttons[4] = new ActionBarButton(spell, Keys.D1, Keys.None);

                PlayerRef.Character.Entity.Spells.TryGetValue("Deep Freeze", out spell);
                actionBar1.Buttons[5] = new ActionBarButton(spell, Keys.F, Keys.None);

                PlayerRef.Character.Entity.Spells.TryGetValue("Frostjaw", out spell);
                actionBar1.Buttons[6] = new ActionBarButton(spell, Keys.D2, Keys.LeftControl);

                PlayerRef.Character.Entity.Spells.TryGetValue("Ice Lance", out spell);
                actionBar1.Buttons[7] = new ActionBarButton(spell, Keys.R, Keys.LeftShift);

                PlayerRef.Character.Entity.Spells.TryGetValue("Mind Spike", out spell);
                actionBar1.Buttons[8] = new ActionBarButton(spell, Keys.E, Keys.LeftControl);

                PlayerRef.Character.Entity.Spells.TryGetValue("Mind Blast", out spell);
                actionBar1.Buttons[9] = new ActionBarButton(spell, Keys.E, Keys.LeftShift);

                PlayerRef.Character.Entity.Spells.TryGetValue("Scorch", out spell);
                actionBar1.Buttons[10] = new ActionBarButton(spell, Keys.D2, Keys.None);

                PlayerRef.Character.Entity.Spells.TryGetValue("Pyroblast", out spell);
                actionBar1.Buttons[11] = new ActionBarButton(spell, Keys.E, Keys.None);

                PlayerRef.Character.Entity.Spells.TryGetValue("Blast Wave", out spell);
                actionBar1.Buttons[12] = new ActionBarButton(spell, Keys.D3, Keys.LeftControl);

                PlayerRef.Character.Entity.Spells.TryGetValue("Blazing Speed", out spell);
                actionBar1.Buttons[13] = new ActionBarButton(spell, Keys.X, Keys.LeftControl);
                #endregion

                #region Action Bar 2
                actionBar2 = new ActionBar(game, this, new Point(340, 715));

                PlayerRef.Character.Entity.Spells.TryGetValue("Cone of Cold", out spell);
                actionBar2.Buttons[0] = new ActionBarButton(spell, Keys.D3, Keys.None);

                PlayerRef.Character.Entity.Spells.TryGetValue("Divine Insight", out spell);
                actionBar2.Buttons[1] = new ActionBarButton(spell, Keys.C, Keys.LeftShift);

                PlayerRef.Character.Entity.Spells.TryGetValue("Shadow Word: Pain", out spell);
                actionBar2.Buttons[2] = new ActionBarButton(spell, Keys.R, Keys.None);

                PlayerRef.Character.Entity.Spells.TryGetValue("Fire Blast", out spell);
                actionBar2.Buttons[3] = new ActionBarButton(spell, Keys.D5, Keys.None);

                PlayerRef.Character.Entity.Spells.TryGetValue("Icy Veins", out spell);
                actionBar2.Buttons[4] = new ActionBarButton(spell, Keys.D4, Keys.LeftShift);

                PlayerRef.Character.Entity.Spells.TryGetValue("Blink", out spell);
                actionBar2.Buttons[5] = new ActionBarButton(spell, Keys.Q, Keys.None);

                PlayerRef.Character.Entity.Spells.TryGetValue("Phantasm", out spell);
                actionBar2.Buttons[6] = new ActionBarButton(spell, Keys.D2, Keys.LeftShift);

                PlayerRef.Character.Entity.Spells.TryGetValue("Renew", out spell);
                actionBar2.Buttons[7] = new ActionBarButton(spell, Keys.Z, Keys.None);

                PlayerRef.Character.Entity.Spells.TryGetValue("Pain Suppression", out spell);
                actionBar2.Buttons[8] = new ActionBarButton(spell, Keys.G, Keys.LeftShift);

                PlayerRef.Character.Entity.Spells.TryGetValue("Psychic Scream", out spell);
                actionBar2.Buttons[9] = new ActionBarButton(spell, Keys.Q, Keys.LeftControl);

                PlayerRef.Character.Entity.Spells.TryGetValue("Shadow Form", out spell);
                actionBar2.Buttons[10] = new ActionBarButton(spell, Keys.H, Keys.LeftShift);

                PlayerRef.Character.Entity.Spells.TryGetValue("Ice Block", out spell);
                actionBar2.Buttons[11] = new ActionBarButton(spell, Keys.D5, Keys.LeftShift);

                PlayerRef.Character.Entity.Spells.TryGetValue("Greater Heal", out spell);
                actionBar2.Buttons[12] = new ActionBarButton(spell, Keys.T, Keys.LeftShift);

                PlayerRef.Character.Entity.Spells.TryGetValue("Vampiric Touch", out spell);
                actionBar2.Buttons[13] = new ActionBarButton(spell, Keys.R, Keys.LeftControl);
                #endregion
            }

            #region Frames
            eventFont = game.Content.Load<SpriteFont>(@"Fonts\EventFont");
            Font = game.Content.Load<SpriteFont>(@"Fonts\UiFont");
            HotkeyFont = game.Content.Load<SpriteFont>(@"Fonts\HotkeyFont");
            coolDownFont = game.Content.Load<SpriteFont>(@"Fonts\SmallCooldown");
            playerFrame = game.Content.Load<Texture2D>(@"PlayerInterface\CharacterFrames\UI-Player-Portrait");
            targetFrame = game.Content.Load<Texture2D>(@"PlayerInterface\TargetingFrames\ui-targetingframe");
            playerPortrait = game.Content.Load<Texture2D>(@"PlayerInterface\CharacterFrames\Human");
            uiParts = game.Content.Load<Texture2D>(@"PlayerInterface\CharacterFrames\UI-Parts");
            barFillNoTrans = game.Content.Load<Texture2D>(@"PlayerInterface\CharacterFrames\BarFill-NoTrans");
            barFill = game.Content.Load<Texture2D>(@"PlayerInterface\CharacterFrames\BarFill");
            castingBarSpark = game.Content.Load<Texture2D>(@"PlayerInterface\CastingBar\ui-castingbar-spark");
            castingBar = game.Content.Load<Texture2D>(@"PlayerInterface\CastingBar\ui-frostbar");
            statusBar = game.Content.Load<Texture2D>(@"PlayerInterface\CastingBar\ui-statusbar");
            #endregion
        }

        public void Update(GameTime gameTime, World WorldRef)
        {
            foreach (SkillEvent skillEvent in SkillEvents)
                skillEvent.Update(gameTime);
            foreach (SkillDamageEvent skillDamageEvent in SkillDamageEvents)
                skillDamageEvent.Update(gameTime);

            SkillEvents.RemoveAll(item => item.FloatingTime == TimeSpan.Zero);
            SkillDamageEvents.RemoveAll(item => item.FloatingTime == TimeSpan.Zero);
            actionBar1.Update(gameTime, WorldRef);
            actionBar2.Update(gameTime, WorldRef);
        }
        public bool UpdateNetwork(GameTime gameTime, out PacketWriter packetWriter)
        {
            foreach (SkillEvent skillEvent in SkillEvents)
                skillEvent.Update(gameTime);
            foreach (SkillDamageEvent skillDamageEvent in SkillDamageEvents)
                skillDamageEvent.Update(gameTime);

            SkillEvents.RemoveAll(item => item.FloatingTime == TimeSpan.Zero);
            SkillDamageEvents.RemoveAll(item => item.FloatingTime == TimeSpan.Zero);

            packetWriter = new PacketWriter();
            packetWriter.Write('S');
            bool result = false;
            result = actionBar1.UpdateNetwork(gameTime, ref packetWriter);
            if (actionBar2.UpdateNetwork(gameTime, ref packetWriter))
                result = true;
            return result;
        }
        public void UpdateHost(GameTime gameTime, World WorldRef, NetworkSession networkSession)
        {
            foreach (SkillEvent skillEvent in SkillEvents)
                skillEvent.Update(gameTime);
            foreach (SkillDamageEvent skillDamageEvent in SkillDamageEvents)
                skillDamageEvent.Update(gameTime);

            SkillEvents.RemoveAll(item => item.FloatingTime == TimeSpan.Zero);
            SkillDamageEvents.RemoveAll(item => item.FloatingTime == TimeSpan.Zero);

            actionBar1.UpdateHost(gameTime, WorldRef, networkSession);
            actionBar2.UpdateHost(gameTime, WorldRef, networkSession);
        }
        public void DrawEvents(GameTime gameTime, SpriteBatch spriteBatch)
        {
            int iconSize = 30;
            Texture2D icon;

            foreach (SkillEvent skillEvent in SkillEvents)
            {
                if (Icons.TryGetValue(skillEvent.SkillName, out icon))
                {
                    spriteBatch.Draw(icon, new Rectangle((int)(skillEvent.Target.Position.X - iconSize / 4), (int)(skillEvent.Target.Position.Y - iconSize / 4 - 20 - skillEvent.Offset), iconSize, iconSize), Color.White);
                    spriteBatch.DrawString(eventFont, skillEvent.SkillName, skillEvent.Target.Position + new Vector2(iconSize / 2, -30 + 1 - skillEvent.Offset), Color.Black);
                    spriteBatch.DrawString(eventFont, skillEvent.SkillName, skillEvent.Target.Position + new Vector2(iconSize / 2, -30 - 1 - skillEvent.Offset), Color.Black);
                    spriteBatch.DrawString(eventFont, skillEvent.SkillName, skillEvent.Target.Position + new Vector2(iconSize / 2 + 1, -30 - skillEvent.Offset), Color.Black);
                    spriteBatch.DrawString(eventFont, skillEvent.SkillName, skillEvent.Target.Position + new Vector2(iconSize / 2 - 1, -30 - skillEvent.Offset), Color.Black);
                    spriteBatch.DrawString(eventFont, skillEvent.SkillName, skillEvent.Target.Position + new Vector2(iconSize / 2, -30 - skillEvent.Offset), Color.Yellow);
                }
            }

            float scale;
            foreach (SkillDamageEvent skillEvent in SkillDamageEvents)
            {
                if (Icons.TryGetValue(skillEvent.SkillName, out icon))
                {
                    Vector2 Position = Vector2.Zero;
                    if (!skillEvent.IsBinded)
                        Position = skillEvent.Position + new Vector2(iconSize / 2, -10 - skillEvent.Offset);
                    else
                        Position = skillEvent.Target.Position + new Vector2(iconSize / 2, -10 - skillEvent.Offset);

                    if (skillEvent.IsCritical)
                        scale = 1.2f + 1.3f*(float)(skillEvent.FloatingTime.TotalSeconds/skillEvent.FullDuration.TotalSeconds);
                    else
                        scale = 1.2f;

                    spriteBatch.DrawString(eventFont, skillEvent.Damage.ToString(), Position + new Vector2(0, 1), Color.Black, 0, Vector2.Zero, scale, SpriteEffects.None, 0);
                    spriteBatch.DrawString(eventFont, skillEvent.Damage.ToString(), Position + new Vector2(0, -1), Color.Black, 0, Vector2.Zero, scale, SpriteEffects.None, 0);
                    spriteBatch.DrawString(eventFont, skillEvent.Damage.ToString(), Position + new Vector2(1, 0), Color.Black, 0, Vector2.Zero, scale, SpriteEffects.None, 0);
                    spriteBatch.DrawString(eventFont, skillEvent.Damage.ToString(), Position + new Vector2(-1, 0), Color.Black, 0, Vector2.Zero, scale, SpriteEffects.None, 0);
                    spriteBatch.DrawString(eventFont, skillEvent.Damage.ToString(), Position, Color.Yellow, 0, Vector2.Zero, scale, SpriteEffects.None, 0);
                }
            }
        }
        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            int i = 0;
            int j = 0;
            int k = 0;
            int z = 0;
            string strCooldown;
            Point origin = new Point(410, 590);
            Point PositionBuff = new Point(origin.X - 120, origin.Y - 10);
            int iconSize = 40;
            Texture2D icon;

            #region Player Frame
            spriteBatch.Draw(uiParts, new Rectangle(origin.X, origin.Y + 17, 118, 17), new Rectangle(2, 50, 166, 14), Color.LightGray);
            spriteBatch.Draw(barFill, new Rectangle(origin.X, origin.Y + 36, 118, 10), new Rectangle(0, 0, 64, 64), Color.Gray);
            spriteBatch.Draw(barFillNoTrans, new Rectangle(origin.X, origin.Y + 47, 118 , 10), new Rectangle(0, 0, 64, 64), Color.Blue);
            spriteBatch.Draw(barFillNoTrans, new Rectangle(origin.X, origin.Y + 36, 118 * PlayerRef.Character.Entity.Health.CurrentValue / PlayerRef.Character.Entity.Health.MaximumValue, 10), new Rectangle(0, 0, 64, 64), Color.Green);
            spriteBatch.DrawString(Font, PlayerRef.Character.Entity.EntityName, new Vector2(origin.X + 20, origin.Y + 18), Color.Black, 0, Vector2.Zero, 0.70f, SpriteEffects.None, 0);
            spriteBatch.DrawString(Font, PlayerRef.Character.Entity.EntityName, new Vector2(origin.X + 24, origin.Y + 18), Color.Black, 0, Vector2.Zero, 0.70f, SpriteEffects.None, 0);
            spriteBatch.DrawString(Font, PlayerRef.Character.Entity.EntityName, new Vector2(origin.X + 22, origin.Y + 16), Color.SkyBlue, 0, Vector2.Zero, 0.70f, SpriteEffects.None, 0);
            spriteBatch.Draw(playerPortrait, new Rectangle(origin.X - 65, origin.Y + 5, 64, 64), new Rectangle(0, 0, 64, 64), Color.White);
            spriteBatch.Draw(playerFrame, new Rectangle(origin.X - 70, origin.Y, 256, 76), new Rectangle(0, 0, 256, 76), Color.White);

            #region PlayerBuffs
            foreach (Buff buff in PlayerRef.Character.Entity.Buffs)
            {
                if (Icons.TryGetValue(buff.Name, out icon))
                {
                    if (buff.Name == "Avatar" || buff.Name == "Shield Wall" || buff.Name == "Battle Shout")
                    {
                        PositionBuff.X -= 100;
                        iconSize = 37;
                        #region Important Buff
                        spriteBatch.Draw(icon, new Rectangle(PositionBuff.X + k, PositionBuff.Y - 550, iconSize * 2, iconSize * 2), new Rectangle(0, 0, 64, 64), Color.White);
                        if (buff.Seconds != -1)
                            if (buff.TimeLeft.TotalSeconds < 1)
                            {
                                strCooldown = "0." + (buff.TimeLeft.Milliseconds / 100).ToString();
                                spriteBatch.DrawString(coolDownFont, strCooldown, new Vector2(PositionBuff.X + 2 - 5 + k, PositionBuff.Y - 570 + 3 + z * iconSize * 2), Color.Black);
                                spriteBatch.DrawString(coolDownFont, strCooldown, new Vector2(PositionBuff.X - 2 - 5 + k, PositionBuff.Y - 570 + 1 + z * iconSize * 2), Color.Black);
                                spriteBatch.DrawString(coolDownFont, strCooldown, new Vector2(PositionBuff.X - 5 + k, PositionBuff.Y - 570 + 1 + z * iconSize * 2), Color.Yellow);
                            }
                            else
                            {
                                strCooldown = (buff.TimeLeft.Seconds).ToString();
                                spriteBatch.DrawString(coolDownFont, strCooldown, new Vector2(PositionBuff.X - 5 + 2 + k, PositionBuff.Y - 570 + 3 + z * iconSize * 2), Color.Black);
                                spriteBatch.DrawString(coolDownFont, strCooldown, new Vector2(PositionBuff.X - 5 - 2 + k, PositionBuff.Y - 570 + 1 + z * iconSize * 2), Color.Black);
                                spriteBatch.DrawString(coolDownFont, strCooldown, new Vector2(PositionBuff.X - 5 + k, PositionBuff.Y - 570 + 1 + z * iconSize * 2), Color.Yellow);
                            }
                        if (buff.HasStucks)
                        {
                            strCooldown = (buff.CurrentStucks).ToString();
                            spriteBatch.DrawString(HotkeyFont, strCooldown, new Vector2(PositionBuff.X + 5 + iconSize * 8 / 5 + k, PositionBuff.Y - 550 + 20 + 3 + iconSize * 4 / 5 + z * iconSize * 2) - HotkeyFont.MeasureString(strCooldown) / 2, Color.Black);
                            spriteBatch.DrawString(HotkeyFont, strCooldown, new Vector2(PositionBuff.X + 1 + iconSize * 8 / 5 + k, PositionBuff.Y - 550 + 20 + 1 + iconSize * 4 / 5 + z * iconSize * 2) - HotkeyFont.MeasureString(strCooldown) / 2, Color.Black);
                            spriteBatch.DrawString(HotkeyFont, strCooldown, new Vector2(PositionBuff.X + 3 + iconSize * 8 / 5 + k, PositionBuff.Y - 550 + 20 + 1 + iconSize * 4 / 5 + z * iconSize * 2) - HotkeyFont.MeasureString(strCooldown) / 2, Color.White);
                        }
                        strCooldown = buff.Name;
                        spriteBatch.DrawString(eventFont, strCooldown, new Vector2(PositionBuff.X + 2 + 20 + 2 * iconSize + k, PositionBuff.Y - 600 + 20 + 3 + iconSize + z * iconSize * 2) - new Vector2(20, 5), Color.Black, 0, Vector2.Zero, 1.5f, SpriteEffects.None, 0);
                        spriteBatch.DrawString(eventFont, strCooldown, new Vector2(PositionBuff.X - 2 + 20 + 2 * iconSize + k, PositionBuff.Y - 600 + 20 + 1 + iconSize + z * iconSize * 2) - new Vector2(20, 5), Color.Black, 0, Vector2.Zero, 1.5f, SpriteEffects.None, 0);
                        spriteBatch.DrawString(eventFont, strCooldown, new Vector2(PositionBuff.X + 20 + 2 * iconSize + k, PositionBuff.Y - 600 + 20 + 1 + iconSize + z * iconSize * 2) - new Vector2(20, 5), Color.Yellow, 0, Vector2.Zero, 1.5f, SpriteEffects.None, 0);
                        k += 10 + iconSize * 2 + (int)(eventFont.MeasureString(buff.Name).X * 1.5f);
                        if (k > 1000)
                        {
                            k = 0;
                            z++;
                            if (z == 2)
                                break;
                        }
                        #endregion
                        iconSize = 40;
                        PositionBuff.X += 100;
                    }
                    else
                    {
                        #region Normal Buff
                        spriteBatch.Draw(icon, new Rectangle(PositionBuff.X - i * (iconSize + 1), PositionBuff.Y, iconSize, iconSize), new Rectangle(0, 0, 64, 64), Color.White);
                        if (buff.Seconds != -1)
                            if (buff.TimeLeft.TotalSeconds < 1)
                            {
                                strCooldown = "0." + (buff.TimeLeft.Milliseconds / 100).ToString();
                                spriteBatch.DrawString(coolDownFont, strCooldown, new Vector2(PositionBuff.X + 2 + iconSize / 2 - i * (iconSize + 1), PositionBuff.Y + 2 + iconSize / 2) - coolDownFont.MeasureString(strCooldown) / 2, Color.Black);
                                spriteBatch.DrawString(coolDownFont, strCooldown, new Vector2(PositionBuff.X - 2 + iconSize / 2 - i * (iconSize + 1), PositionBuff.Y + iconSize / 2) - coolDownFont.MeasureString(strCooldown) / 2, Color.Black);
                                spriteBatch.DrawString(coolDownFont, strCooldown, new Vector2(PositionBuff.X + iconSize / 2 - i * (iconSize + 1), PositionBuff.Y + iconSize / 2) - coolDownFont.MeasureString(strCooldown) / 2, Color.Yellow);
                            }
                            else
                            {
                                strCooldown = (buff.TimeLeft.Seconds).ToString();
                                spriteBatch.DrawString(coolDownFont, strCooldown, new Vector2(PositionBuff.X + 2 + iconSize / 2 - i * (iconSize + 1), PositionBuff.Y + 2 + iconSize / 2) - coolDownFont.MeasureString(strCooldown) / 2, Color.Black);
                                spriteBatch.DrawString(coolDownFont, strCooldown, new Vector2(PositionBuff.X - 2 + iconSize / 2 - i * (iconSize + 1), PositionBuff.Y + iconSize / 2) - coolDownFont.MeasureString(strCooldown) / 2, Color.Black);
                                spriteBatch.DrawString(coolDownFont, strCooldown, new Vector2(PositionBuff.X + iconSize / 2 - i * (iconSize + 1), PositionBuff.Y + iconSize / 2) - coolDownFont.MeasureString(strCooldown) / 2, Color.Yellow);
                            }
                        if (buff.HasStucks)
                        {
                            strCooldown = (buff.CurrentStucks).ToString();
                            spriteBatch.DrawString(HotkeyFont, strCooldown, new Vector2(PositionBuff.X + 5 + iconSize * 4 / 5 - i * (iconSize + 1), PositionBuff.Y + 3 + iconSize * 4 / 5) - HotkeyFont.MeasureString(strCooldown) / 2, Color.Black);
                            spriteBatch.DrawString(HotkeyFont, strCooldown, new Vector2(PositionBuff.X + 1 + iconSize * 4 / 5 - i * (iconSize + 1), PositionBuff.Y + 1 + iconSize * 4 / 5) - HotkeyFont.MeasureString(strCooldown) / 2, Color.Black);
                            spriteBatch.DrawString(HotkeyFont, strCooldown, new Vector2(PositionBuff.X + 3 + iconSize * 4 / 5 - i * (iconSize + 1), PositionBuff.Y + 1 + iconSize * 4 / 5) - HotkeyFont.MeasureString(strCooldown) / 2, Color.White);
                        }
                        i++;
                        if (i * iconSize > 290)
                        {
                            i = 0;
                            j++;
                            if (j == 2)
                                break;
                            PositionBuff.Y += iconSize + 1;
                        }
                        #endregion
                    }
                }
                else if (Icons.TryGetValue("Generic", out icon))
                {
                    spriteBatch.Draw(icon, new Rectangle(PositionBuff.X - i * (iconSize + 1), PositionBuff.Y, iconSize, iconSize), new Rectangle(0, 0, 64, 64), Color.White);
                    i++;
                    if (i * iconSize > 290)
                    {
                        i = 0;
                        j++;
                        if (j == 2)
                            break;
                        PositionBuff.Y += iconSize + 1;
                    }
                }
            }
            #endregion
            #endregion

            #region Target Frame
            if (PlayerRef.Character.Target != null)
            {
                spriteBatch.Draw(uiParts, new Rectangle(origin.X + 430, origin.Y + 17, 120, 17), new Rectangle(2, 50, 166, 14), Color.LightGray);
                spriteBatch.Draw(barFill, new Rectangle(origin.X + 430, origin.Y + 36, 120, 10), new Rectangle(0, 0, 64, 64), Color.Gray);
                spriteBatch.Draw(barFillNoTrans, new Rectangle(origin.X + 430, origin.Y + 47, 120, 10), new Rectangle(0, 0, 64, 64), Color.Blue);
                spriteBatch.Draw(barFillNoTrans, new Rectangle(origin.X + 430, origin.Y + 36, 120 * PlayerRef.Character.Target.Character.Entity.Health.CurrentValue / PlayerRef.Character.Target.Character.Entity.Health.MaximumValue, 10), new Rectangle(0, 0, 64, 64), Color.Green);
                spriteBatch.DrawString(Font, PlayerRef.Character.Target.Character.Entity.EntityName, new Vector2(origin.X + 452, origin.Y + 16), Color.SkyBlue, 0, Vector2.Zero, 0.70f, SpriteEffects.None, 0);
                spriteBatch.Draw(playerPortrait, new Rectangle(origin.X + 550, origin.Y + 5, 64, 64), new Rectangle(0, 0, 64, 64), Color.White);
                spriteBatch.Draw(targetFrame, new Rectangle(origin.X + 400, origin.Y, 256, 128), new Rectangle(0, 8, 256, 120), Color.White);
                PositionBuff = new Point(origin.X + 625, origin.Y - 10);

                #region Target Casting Frame
                if (PlayerRef.Character.Target.Character.SpellCast != null)
                {
                    int x = 270;
                    int y = -40;
                    int width = 145;
                    int height = 30;
                    int iconTargetSize = 30;
                    Point position = new Point(origin.X + 126 + x, origin.Y + y);

                    double castBarMultiplier = (PlayerRef.Character.Target.Character.SpellCast.CastTime.TotalMilliseconds - PlayerRef.Character.Target.Character.SpellCast.CastTimeLeft.TotalMilliseconds) / PlayerRef.Character.Target.Character.SpellCast.CastTime.TotalMilliseconds;
                    spriteBatch.Draw(uiParts, new Rectangle(position.X + 1, position.Y + 2, width + 6, height + 4), new Rectangle(2, 50, 166, 12), Color.White);
                    spriteBatch.Draw(playerFrame, new Rectangle(position.X + 2, position.Y + 2, width + 4, 10), new Rectangle(78, 14, 106, 10), Color.White);
                    spriteBatch.Draw(playerFrame, new Rectangle(position.X + 2, position.Y + height - 4, width + 4, 10), new Rectangle(78, 25, 106, 10), Color.White);
                    spriteBatch.Draw(playerFrame, new Rectangle(position.X, position.Y + 6, 6, height - 2), new Rectangle(186, 19, 6, 11), Color.White, 0, Vector2.Zero, SpriteEffects.FlipHorizontally, 0);
                    spriteBatch.Draw(playerFrame, new Rectangle(position.X + width + 4, position.Y + 6, 6, height - 2), new Rectangle(186, 19, 6, 11), Color.White);
                    spriteBatch.Draw(playerFrame, new Rectangle(position.X, position.Y + 2, 6, 8), new Rectangle(186, 13, 6, 8), Color.White, 0, Vector2.Zero, SpriteEffects.FlipHorizontally, 0);
                    spriteBatch.Draw(playerFrame, new Rectangle(position.X + width + 4, position.Y + 2, 6, 8), new Rectangle(186, 13, 6, 8), Color.White);
                    spriteBatch.Draw(playerFrame, new Rectangle(position.X, position.Y + height + 1, 6, 5), new Rectangle(186, 30, 6, 5), Color.White, 0, Vector2.Zero, SpriteEffects.FlipHorizontally, 0);
                    spriteBatch.Draw(playerFrame, new Rectangle(position.X + width + 4, position.Y + height + 1, 6, 5), new Rectangle(186, 30, 6, 5), Color.White);
                    spriteBatch.Draw(castingBar, new Rectangle(position.X + 4, position.Y + 5, (int)(width * castBarMultiplier), height - 2), new Rectangle(0, 0, (int)(290 * castBarMultiplier), 38), Color.Orange);
                    spriteBatch.Draw(castingBarSpark, new Rectangle(position.X - 12 + (int)(width * castBarMultiplier), position.Y - 8, 32, height + 20), new Rectangle(0, 0, 32, 32), Color.White);
                    spriteBatch.DrawString(Font, PlayerRef.Character.Target.Character.SpellCast.Spell.Name, new Vector2(position.X + width/2 + 10, position.Y + height - 6) - Font.MeasureString(PlayerRef.Character.Target.Character.SpellCast.Spell.Name) / 2, Color.Black, 0, Vector2.Zero, 0.8f, SpriteEffects.None, 0);
                    spriteBatch.DrawString(Font, PlayerRef.Character.Target.Character.SpellCast.Spell.Name, new Vector2(position.X + width/2 + 10, position.Y + height - 6) - Font.MeasureString(PlayerRef.Character.Target.Character.SpellCast.Spell.Name) / 2, Color.White, 0, Vector2.Zero, 0.8f, SpriteEffects.None, 0);

                    if (Icons.TryGetValue(PlayerRef.Character.Target.Character.SpellCast.Spell.Name, out icon))
                        spriteBatch.Draw(icon, new Rectangle(position.X - 4, position.Y - 5, iconTargetSize, iconTargetSize), new Rectangle(0, 0, 64, 64), Color.White);
                    else if (Icons.TryGetValue("Frost Fire Bolt", out icon))
                        spriteBatch.Draw(icon, new Rectangle(position.X - 4, position.Y - 5, iconTargetSize, iconTargetSize), new Rectangle(0, 0, 64, 64), Color.White);
                }
                #endregion

                #region Target Buffs
                i = 0; j = 0;
                foreach (Buff buff in PlayerRef.Character.Target.Character.Entity.Buffs)
                {
                    if (Icons.TryGetValue(buff.Name, out icon))
                    {
                        spriteBatch.Draw(icon, new Rectangle(PositionBuff.X + i * (iconSize + 1), PositionBuff.Y, iconSize, iconSize), new Rectangle(0, 0, 64, 64), Color.White);
                        if (buff.Seconds != -1)
                        if (buff.TimeLeft.TotalSeconds < 1)
                        {
                            strCooldown = "0." + (buff.TimeLeft.Milliseconds / 100).ToString();
                            spriteBatch.DrawString(coolDownFont, strCooldown, new Vector2(PositionBuff.X + 2 + iconSize / 2 + i * (iconSize + 1), PositionBuff.Y + 2 + iconSize / 2) - coolDownFont.MeasureString(strCooldown) / 2, Color.Black);
                            spriteBatch.DrawString(coolDownFont, strCooldown, new Vector2(PositionBuff.X - 2 + iconSize / 2 + i * (iconSize + 1), PositionBuff.Y + iconSize / 2) - coolDownFont.MeasureString(strCooldown) / 2, Color.Black);
                            spriteBatch.DrawString(coolDownFont, strCooldown, new Vector2(PositionBuff.X + iconSize / 2 + i * (iconSize + 1), PositionBuff.Y + iconSize / 2) - coolDownFont.MeasureString(strCooldown) / 2, Color.Yellow);
                        }
                        else
                        {
                            strCooldown = (buff.TimeLeft.Seconds).ToString();
                            spriteBatch.DrawString(coolDownFont, strCooldown, new Vector2(PositionBuff.X + 2 + iconSize / 2 + i * (iconSize + 1), PositionBuff.Y + 2 + iconSize / 2) - coolDownFont.MeasureString(strCooldown) / 2, Color.Black);
                            spriteBatch.DrawString(coolDownFont, strCooldown, new Vector2(PositionBuff.X - 2 + iconSize / 2 + i * (iconSize + 1), PositionBuff.Y + iconSize / 2) - coolDownFont.MeasureString(strCooldown) / 2, Color.Black);
                            spriteBatch.DrawString(coolDownFont, strCooldown, new Vector2(PositionBuff.X + iconSize / 2 + i * (iconSize + 1), PositionBuff.Y + iconSize / 2) - coolDownFont.MeasureString(strCooldown) / 2, Color.Yellow);
                        }
                        if (buff.HasStucks)
                        {
                            strCooldown = (buff.CurrentStucks).ToString();
                            spriteBatch.DrawString(HotkeyFont, strCooldown, new Vector2(PositionBuff.X + 5 + iconSize * 4 / 5 - i * (iconSize + 1), PositionBuff.Y + 3 + iconSize * 4 / 5) - HotkeyFont.MeasureString(strCooldown) / 2, Color.Black);
                            spriteBatch.DrawString(HotkeyFont, strCooldown, new Vector2(PositionBuff.X + 1 + iconSize * 4 / 5 - i * (iconSize + 1), PositionBuff.Y + 1 + iconSize * 4 / 5) - HotkeyFont.MeasureString(strCooldown) / 2, Color.Black);
                            spriteBatch.DrawString(HotkeyFont, strCooldown, new Vector2(PositionBuff.X + 3 + iconSize * 4 / 5 - i * (iconSize + 1), PositionBuff.Y + 1 + iconSize * 4 / 5) - HotkeyFont.MeasureString(strCooldown) / 2, Color.White);
                        }
                        i++;
                        if (i * iconSize > 290)
                        {
                            i = 0;
                            j++;
                            if (j == 2)
                                break;
                            PositionBuff.Y += iconSize + 1;
                        }
                    }
                    else if (Icons.TryGetValue("Generic", out icon))
                    {
                        spriteBatch.Draw(icon, new Rectangle(PositionBuff.X + i * (iconSize + 1), PositionBuff.Y, iconSize, iconSize), new Rectangle(0, 0, 64, 64), Color.White);
                        i++;
                        if (i * iconSize > 290)
                        {
                            i = 0;
                            j++;
                            if (j == 2)
                                break;
                            PositionBuff.Y += iconSize + 1;
                        }
                    }
                }
                #endregion
            }
            #endregion

            #region Casting Frame
            if (!PlayerRef.Character.GlobalCooldown.NoCooldown)
                spriteBatch.Draw(castingBarSpark, new Rectangle(origin.X + 400 - 277 * (int)PlayerRef.Character.GlobalCooldown.TimeLeft.TotalMilliseconds / (int)PlayerRef.Character.GlobalCooldown.Duration.TotalMilliseconds, origin.Y + 40, 32, 32), new Rectangle(0, 0, 32, 32), Color.White);
            
            if (PlayerRef.Character.SpellCast != null)
            {
                int y = 10;
                double castBarMultiplier = (PlayerRef.Character.SpellCast.CastTime.TotalMilliseconds - PlayerRef.Character.SpellCast.CastTimeLeft.TotalMilliseconds)/PlayerRef.Character.SpellCast.CastTime.TotalMilliseconds;
                spriteBatch.Draw(uiParts, new Rectangle(origin.X + 127, origin.Y + 2 + y, 296, 44), new Rectangle(2, 50, 166, 12), Color.White);
                spriteBatch.Draw(playerFrame, new Rectangle(origin.X + 128, origin.Y + 2 + y, 294, 10), new Rectangle(78, 14, 106, 10), Color.White);
                spriteBatch.Draw(playerFrame, new Rectangle(origin.X + 128, origin.Y + 36 + y, 294, 10), new Rectangle(78, 25, 106, 10), Color.White);
                spriteBatch.Draw(playerFrame, new Rectangle(origin.X + 126, origin.Y + 6 + y, 6, 38), new Rectangle(186, 19, 6, 11), Color.White, 0, Vector2.Zero, SpriteEffects.FlipHorizontally, 0);
                spriteBatch.Draw(playerFrame, new Rectangle(origin.X + 420, origin.Y + 6 + y, 6, 38), new Rectangle(186, 19, 6, 11), Color.White);
                spriteBatch.Draw(playerFrame, new Rectangle(origin.X + 126, origin.Y + 2 + y, 6, 8), new Rectangle(186, 13, 6, 8), Color.White, 0, Vector2.Zero, SpriteEffects.FlipHorizontally, 0);
                spriteBatch.Draw(playerFrame, new Rectangle(origin.X + 420, origin.Y + 2 + y, 6, 8), new Rectangle(186, 13, 6, 8), Color.White);
                spriteBatch.Draw(playerFrame, new Rectangle(origin.X + 126, origin.Y + 41 + y, 6, 5), new Rectangle(186, 30, 6, 5), Color.White, 0, Vector2.Zero, SpriteEffects.FlipHorizontally, 0);
                spriteBatch.Draw(playerFrame, new Rectangle(origin.X + 420, origin.Y + 41 + y, 6, 5), new Rectangle(186, 30, 6, 5), Color.White);
                spriteBatch.Draw(castingBar, new Vector2(origin.X + 130, origin.Y + 5 + y), new Rectangle(0, 0, (int)(290 * castBarMultiplier), 38), Color.Orange);
                spriteBatch.Draw(castingBarSpark, new Rectangle(origin.X + 114 + (int)(290 * castBarMultiplier), origin.Y - 8 + y, 32, 64), new Rectangle(0, 0, 32, 32), Color.White);
                spriteBatch.DrawString(Font, PlayerRef.Character.SpellCast.Spell.Name, new Vector2(origin.X + 278, origin.Y + 26 + y) - Font.MeasureString(PlayerRef.Character.SpellCast.Spell.Name) / 2, Color.Black, 0, Vector2.Zero, 1, SpriteEffects.None, 0);
                spriteBatch.DrawString(Font, PlayerRef.Character.SpellCast.Spell.Name, new Vector2(origin.X + 275, origin.Y + 24 + y) - Font.MeasureString(PlayerRef.Character.SpellCast.Spell.Name) / 2, Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0);
                if (Icons.TryGetValue(PlayerRef.Character.SpellCast.Spell.Name, out icon))
                    spriteBatch.Draw(icon, new Rectangle(origin.X + 122, origin.Y - 5 + y, 40, 40), new Rectangle(0, 0, 64, 64), Color.White);
                else if (Icons.TryGetValue("Frost Fire Bolt", out icon))
                    spriteBatch.Draw(icon, new Rectangle(origin.X + 122, origin.Y - 5 + y, 40, 40), new Rectangle(0, 0, 64, 64), Color.White);
            }
            #endregion

            actionBar1.Draw(gameTime, spriteBatch);
            actionBar2.Draw(gameTime, spriteBatch);
        }
    }
}