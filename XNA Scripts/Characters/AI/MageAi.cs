using System.Collections.Generic;
using System.Linq;
using System;

using BasicRpgEngine.Characters;
using BasicRpgEngine.Characters.AI;
using BasicRpgEngine.Spells;

namespace BasicRpgEngine.Characters.AI
{
    public class MageAi : ICreatureStrategy
    {
        public void ApplyStrategy(Creature monster, World world)
        {
            monster.Character.Target = world.GetNearestEnemyTarget(monster, (int)monster.BoundRect.BottomCenter.X, (int)monster.BoundRect.BottomCenter.Y, new List<string>());
            if (monster.Character.Target != null && monster.Character.Target.Character.Entity.EntityClass == "Warrior")
            {
                monster.CombatStrategy = new MageAiVsWarrior();
                return;
            }
            else
            {
                if (monster.CastTimer.TotalSeconds < 2)
                {
                    int spellNumber = Mechanics.Roll(0, 21);
                    Spell spell = monster.Character.Entity.Spells.ElementAt(Mechanics.Roll(0, monster.Character.Entity.Spells.Count)).Value;
                    monster.CastTimer = TimeSpan.FromSeconds(Mechanics.Roll(6, 12));

                    world.ApplySpell(monster, spell, monster.Character.Target, false, false);
                }
                else
                {
                    Spell spell;
                    if (monster.Character.Entity.Spells.TryGetValue("Frost Bolt", out spell))
                        world.ApplySpell(monster, spell, monster.Character.Target, false, false);
                }
            }
        }
    }
}