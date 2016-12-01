using System.Collections.Generic;

using BasicRpgEngine.Characters;
using BasicRpgEngine.Characters.AI;

namespace BasicRpgEngine.Characters.AI
{
    public class WarriorAi : ICreatureStrategy
    {
        public void ApplyStrategy(Creature monster, World world)
        {
            monster.Character.Target = world.GetNearestEnemyTarget(monster, (int)monster.BoundRect.BottomCenter.X, (int)monster.BoundRect.BottomCenter.Y, new List<string>());
            if (monster.Character.Target != null && monster.Team != monster.Character.Target.Team)
            {
                monster.PathStrategy = new RunToTargetAi();
                monster.CombatStrategy = new WarriorAiOffensive();
                return;
            }
            else
            {
                monster.PathStrategy = new RandomPathAi();
            }
        }
    }
}