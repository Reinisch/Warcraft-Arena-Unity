using Microsoft.Xna.Framework;

using BasicRpgEngine.Characters;
using BasicRpgEngine.Characters.AI;
using BasicRpgEngine.Spells;
using BasicRpgEngine.Graphics;

namespace BasicRpgEngine.Characters.AI
{
    public class MageAiVsWarrior : ICreatureStrategy
    {
        public void ApplyStrategy(Creature monster, World world)
        {
            if (monster.Character.Target == null || monster.Character.Target.Character.Entity.EntityClass != "Warrior")
            {
                monster.CombatStrategy = new MageAi();
                return;
            }
            float distance = Vector2.Distance(monster.Character.Target.Position, monster.Position);
            bool spellSuccess = false;
            Spell spell = null;
            int random = 0;

            if (distance > 1200)
            {
                monster.NeedsEscapeToLeft = false;
                monster.NeedsEscapeToRight = false;
                monster.PathStrategy = new RunToTargetAi();
                if (monster.Character.Entity.Spells.TryGetValue("Pyroblast", out spell))
                    if (spell.SpellCooldown.NoCooldown)
                        spellSuccess = world.ApplySpell(monster, spell, monster.Character.Target, false, false);
                if (monster.Character.Entity.Spells.TryGetValue("Frost Bolt", out spell))
                    if (spell.SpellCooldown.NoCooldown)
                        spellSuccess = world.ApplySpell(monster, spell, monster.Character.Target, false, false);
            }
            else if (distance > 950)
            {
                if (monster.Character.Entity.Spells.TryGetValue("Pyroblast", out spell))
                    if (spell.SpellCooldown.NoCooldown)
                        spellSuccess = world.ApplySpell(monster, spell, monster.Character.Target, false, false);
                random = Mechanics.Roll(0, 2);
                if (random == 0)
                if (monster.Character.Entity.Spells.TryGetValue("Frost Bolt", out spell))
                    if (spell.SpellCooldown.NoCooldown)
                        spellSuccess = world.ApplySpell(monster, spell, monster.Character.Target, false, false);
                if (random == 1)
                if (monster.Character.Entity.Spells.TryGetValue("Scorch", out spell))
                    if (spell.SpellCooldown.NoCooldown)
                        spellSuccess = world.ApplySpell(monster, spell, monster.Character.Target, false, false);
            }
            else if (distance > 800)
            {
                random = Mechanics.Roll(0, 4);
                if (random == 0)
                if (monster.Character.Entity.Spells.TryGetValue("Frost Bolt", out spell))
                    if (spell.SpellCooldown.NoCooldown)
                        spellSuccess = world.ApplySpell(monster, spell, monster.Character.Target, false, false);
                if (random == 1)
                if (monster.Character.Entity.Spells.TryGetValue("Scorch", out spell))
                    if (spell.SpellCooldown.NoCooldown)
                        spellSuccess = world.ApplySpell(monster, spell, monster.Character.Target, false, false);
                if (random == 2)
                if (monster.Character.Entity.Spells.TryGetValue("Pyroblast", out spell))
                    if (spell.SpellCooldown.NoCooldown)
                        spellSuccess = world.ApplySpell(monster, spell, monster.Character.Target, false, false);
                if (random == 3)
                if (monster.Character.Entity.Spells.TryGetValue("Frostjaw", out spell))
                    if (spell.SpellCooldown.NoCooldown)
                        spellSuccess = world.ApplySpell(monster, spell, monster.Character.Target, false, false);
                monster.PathStrategy = new NoMoveAi();
                monster.NeedsEscapeToLeft = false;
                monster.NeedsEscapeToRight = false;
            }
            else if (distance > 600)
            {
                random = Mechanics.Roll(0, 5);
                if (random == 0)
                    if (monster.Character.Entity.Spells.TryGetValue("Frost Bolt", out spell))
                        if (spell.SpellCooldown.NoCooldown)
                            spellSuccess = world.ApplySpell(monster, spell, monster.Character.Target, false, false);
                if (random == 1)
                    if (monster.Character.Entity.Spells.TryGetValue("Scorch", out spell))
                        if (spell.SpellCooldown.NoCooldown)
                            spellSuccess = world.ApplySpell(monster, spell, monster.Character.Target, false, false);
                if (random == 2)
                    if (monster.Character.Entity.Spells.TryGetValue("Pyroblast", out spell))
                        if (spell.SpellCooldown.NoCooldown)
                            spellSuccess = world.ApplySpell(monster, spell, monster.Character.Target, false, false);
                if (random == 3)
                    if (monster.Character.Entity.Spells.TryGetValue("Frostjaw", out spell))
                        if (spell.SpellCooldown.NoCooldown)
                            spellSuccess = world.ApplySpell(monster, spell, monster.Character.Target, false, false);
                if (random == 4)
                    if (monster.Character.Entity.Spells.TryGetValue("Fire Blast", out spell))
                        if (spell.SpellCooldown.NoCooldown)
                            spellSuccess = world.ApplySpell(monster, spell, monster.Character.Target, false, false);
            }
            else if (distance >= 280)
            {
                random = Mechanics.Roll(0, 5);
                if (random == 0)
                    if (monster.Character.Entity.Spells.TryGetValue("Frost Bolt", out spell))
                        if (spell.SpellCooldown.NoCooldown)
                            spellSuccess = world.ApplySpell(monster, spell, monster.Character.Target, false, false);
                if (random == 1)
                    if (monster.Character.Entity.Spells.TryGetValue("Scorch", out spell))
                        if (spell.SpellCooldown.NoCooldown)
                            spellSuccess = world.ApplySpell(monster, spell, monster.Character.Target, false, false);
                if (random == 2)
                    if (monster.Character.Entity.Spells.TryGetValue("Pyroblast", out spell))
                        if (spell.SpellCooldown.NoCooldown)
                            spellSuccess = world.ApplySpell(monster, spell, monster.Character.Target, false, false);
                if (random == 3)
                    if (monster.Character.Entity.Spells.TryGetValue("Frostjaw", out spell))
                        if (spell.SpellCooldown.NoCooldown)
                            spellSuccess = world.ApplySpell(monster, spell, monster.Character.Target, false, false);
                if (random == 4)
                    if (monster.Character.Entity.Spells.TryGetValue("Fire Blast", out spell))
                        if (spell.SpellCooldown.NoCooldown)
                            spellSuccess = world.ApplySpell(monster, spell, monster.Character.Target, false, false);
            }
            else
            {
                monster.PathStrategy = new RunFromTargetAi();
                if (monster.Character.Entity.Spells.TryGetValue("Blast Wave", out spell))
                    if (spell.SpellCooldown.NoCooldown)
                        spellSuccess = world.ApplySpell(monster, spell, monster.Character.Target, false, false);
                if (monster.Character.Entity.Spells.TryGetValue("Frost Nova", out spell))
                    if (spell.SpellCooldown.NoCooldown)
                        spellSuccess = world.ApplySpell(monster, spell, monster.Character.Target, false, false);
                if (monster.Character.Target.Character.Entity.Buffs.Find(item => item.Id == 32) != null)
                {
                    if (monster.Character.Entity.Spells.TryGetValue("Ice Block", out spell))
                        if (spell.SpellCooldown.NoCooldown)
                            spellSuccess = world.ApplySpell(monster, spell, monster.Character.Target, false, false);
                }
                if (monster.Character.Entity.Spells.TryGetValue("Invisibility", out spell))
                    if (spell.SpellCooldown.NoCooldown)
                        spellSuccess = world.ApplySpell(monster, spell, monster.Character.Target, false, false);
                if (!spellSuccess)
                {
                    if (monster.Character.Entity.Spells.TryGetValue("Blazing Speed", out spell))
                        if (spell.SpellCooldown.NoCooldown)
                            spellSuccess = world.ApplySpell(monster, spell, monster.Character.Target, false, false);
                    if (monster.Character.Entity.Spells.TryGetValue("Blink", out spell))
                        if (spell.SpellCooldown.NoCooldown)
                            spellSuccess = world.ApplySpell(monster, spell, monster.Character.Target, false, false);
                }
                if (!spellSuccess)
                {
                    if (monster.Character.Entity.Spells.TryGetValue("Cone of Cold", out spell))
                        if (spell.SpellCooldown.NoCooldown)
                        {
                            if (monster.Character.GlobalCooldown.NoCooldown && !monster.Character.Entity.IsNoControlable
                                && !monster.Character.Entity.IsPacified && !monster.Character.Entity.IsSilenced)
                            {
                                monster.Character.Sprite.CurrentAnimation = monster.Position.X < monster.Character.Target.Position.X ?
                                    AnimationKey.CastRight : AnimationKey.CastLeft;
                                spellSuccess = world.ApplySpell(monster, spell, monster.Character.Target, false, false);
                            }
                        }
                }
                if (!spellSuccess)
                {
                    random = Mechanics.Roll(0, 5);
                    if (random == 0)
                        if (monster.Character.Entity.Spells.TryGetValue("Frost Bolt", out spell))
                            if (spell.SpellCooldown.NoCooldown)
                                spellSuccess = world.ApplySpell(monster, spell, monster.Character.Target, false, false);
                    if (random == 1)
                        if (monster.Character.Entity.Spells.TryGetValue("Scorch", out spell))
                            if (spell.SpellCooldown.NoCooldown)
                                spellSuccess = world.ApplySpell(monster, spell, monster.Character.Target, false, false);
                    if (random == 2)
                        if (monster.Character.Entity.Spells.TryGetValue("Pyroblast", out spell))
                            if (spell.SpellCooldown.NoCooldown)
                                spellSuccess = world.ApplySpell(monster, spell, monster.Character.Target, false, false);
                    if (random == 3)
                        if (monster.Character.Entity.Spells.TryGetValue("Frostjaw", out spell))
                            if (spell.SpellCooldown.NoCooldown)
                                spellSuccess = world.ApplySpell(monster, spell, monster.Character.Target, false, false);
                    if (random == 4)
                        if (monster.Character.Entity.Spells.TryGetValue("Fire Blast", out spell))
                            if (spell.SpellCooldown.NoCooldown)
                                spellSuccess = world.ApplySpell(monster, spell, monster.Character.Target, false, false);
                }
            }        
        }
    }
}