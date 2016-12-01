using Microsoft.Xna.Framework;

using BasicRpgEngine.Characters;
using BasicRpgEngine.Characters.AI;
using BasicRpgEngine.Spells;
using BasicRpgEngine.Graphics;

namespace BasicRpgEngine.Characters.AI
{
    public class WarriorAiOffensive : ICreatureStrategy
    {
        public void ApplyStrategy(Creature monster, World world)
        {
            if (monster.Character.Target == null)
            {
                monster.CombatStrategy = new WarriorAi();
                return;
            }
            float distance = Vector2.Distance(monster.Character.Target.BoundRect.BottomCenter, monster.BoundRect.BottomCenter);
            bool spellSuccess = false;
            Spell spell = null;
            int random = 0;

            if (distance > 1200)
            {
                monster.PathStrategy = new RunToTargetAi();
                if (monster.Character.Entity.Spells.TryGetValue("Charge", out spell))
                    if (spell.SpellCooldown.NoCooldown)
                        spellSuccess = world.ApplySpell(monster, spell, monster.Character.Target, false, false);
            }
            else if (distance > 950)
            {
                monster.PathStrategy = new RunToTargetAi();
                if (monster.Character.Entity.Spells.TryGetValue("Charge", out spell))
                    if (spell.SpellCooldown.NoCooldown)
                        spellSuccess = world.ApplySpell(monster, spell, monster.Character.Target, false, false);
                if (monster.Character.Entity.Spells.TryGetValue("Battle Shout", out spell))
                    if (spell.SpellCooldown.NoCooldown)
                        spellSuccess = world.ApplySpell(monster, spell, monster.Character.Target, false, false);
            }
            else if (distance > 600)
            {
                monster.PathStrategy = new RunToTargetAi();
                if (monster.Character.Entity.Spells.TryGetValue("Charge", out spell))
                    if (spell.SpellCooldown.NoCooldown)
                        spellSuccess = world.ApplySpell(monster, spell, monster.Character.Target, false, false);
                if (monster.Character.Entity.Spells.TryGetValue("Battle Shout", out spell))
                    if (spell.SpellCooldown.NoCooldown)
                        spellSuccess = world.ApplySpell(monster, spell, monster.Character.Target, false, false);
            }
            else if (distance >= 280)
            {
                monster.PathStrategy = new RunToTargetAi();
                if (monster.Character.Entity.Spells.TryGetValue("Charge", out spell))
                    if (spell.SpellCooldown.NoCooldown)
                        spellSuccess = world.ApplySpell(monster, spell, monster.Character.Target, false, false);
                if (monster.Character.Entity.Spells.TryGetValue("Battle Shout", out spell))
                    if (spell.SpellCooldown.NoCooldown)
                        spellSuccess = world.ApplySpell(monster, spell, monster.Character.Target, false, false);
                if (monster.Character.Entity.Spells.TryGetValue("Shockwave", out spell))
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
                if (monster.Character.Entity.Spells.TryGetValue("Thunder Clap", out spell))
                    if (spell.SpellCooldown.NoCooldown)
                        spellSuccess = world.ApplySpell(monster, spell, monster.Character.Target, false, false);
            }
            else 
            {
                if (distance > 180)
                    monster.PathStrategy = new RunToTargetAi();

                if (monster.Character.Entity.Spells.TryGetValue("Avatar", out spell))
                    if (spell.SpellCooldown.NoCooldown)
                        spellSuccess = world.ApplySpell(monster, spell, monster.Character.Target, false, false);
                if (monster.Character.Entity.Spells.TryGetValue("Shield Wall", out spell))
                    if (spell.SpellCooldown.NoCooldown)
                        spellSuccess = world.ApplySpell(monster, spell, monster.Character.Target, false, false);
                if (monster.Character.Target.Character.Entity.Buffs.Find(item => item.Id == 30) == null)
                {
                    if (monster.Character.Entity.Spells.TryGetValue("Hamstring", out spell))
                        if (spell.SpellCooldown.NoCooldown)
                            spellSuccess = world.ApplySpell(monster, spell, monster.Character.Target, false, false);
                }
                if (monster.Character.Entity.Spells.TryGetValue("Throwdown", out spell))
                    if (spell.SpellCooldown.NoCooldown)
                        spellSuccess = world.ApplySpell(monster, spell, monster.Character.Target, false, false);
                if (!spellSuccess)
                {
                    if (monster.Character.Entity.Spells.TryGetValue("Bleed", out spell))
                        if (spell.SpellCooldown.NoCooldown)
                            spellSuccess = world.ApplySpell(monster, spell, monster.Character.Target, false, false);
                    if (monster.Character.Entity.Spells.TryGetValue("Mortal Strike", out spell))
                        if (spell.SpellCooldown.NoCooldown)
                            spellSuccess = world.ApplySpell(monster, spell, monster.Character.Target, false, false);
                }
                if (!spellSuccess)
                {
                    random = Mechanics.Roll(0, 4);
                    if (random == 0)
                        if (monster.Character.Entity.Spells.TryGetValue("Cleave", out spell))
                            if (spell.SpellCooldown.NoCooldown)
                                spellSuccess = world.ApplySpell(monster, spell, monster.Character.Target, false, false);
                    if (random == 1)
                        if (monster.Character.Entity.Spells.TryGetValue("Thunder Clap", out spell))
                            if (spell.SpellCooldown.NoCooldown)
                                spellSuccess = world.ApplySpell(monster, spell, monster.Character.Target, false, false);
                    if (random == 2)
                        if (monster.Character.Entity.Spells.TryGetValue("Battle Shout", out spell))
                            if (spell.SpellCooldown.NoCooldown)
                                spellSuccess = world.ApplySpell(monster, spell, monster.Character.Target, false, false);
                    if (random == 3)
                        if (monster.Character.Entity.Spells.TryGetValue("Shockwave", out spell))
                            if (spell.SpellCooldown.NoCooldown)
                            {
                                if (monster.Character.GlobalCooldown.NoCooldown && !monster.Character.Entity.IsNoControlable
                                    && !monster.Character.Entity.IsPacified && !monster.Character.Entity.IsSilenced)
                                {
                                    if (spellSuccess = world.ApplySpell(monster, spell, monster.Character.Target, false, false))
                                        monster.Character.Sprite.CurrentAnimation = monster.Position.X < monster.Character.Target.Position.X ?
                                                                                AnimationKey.CastRight : AnimationKey.CastLeft;
                                }
                            }
                }
            }
        }
    }
}
