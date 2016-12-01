using Microsoft.Xna.Framework;

using BasicRpgEngine.Characters;
using BasicRpgEngine.Characters.AI;

namespace BasicRpgEngine.Characters.AI
{
    public class RunFromTargetAi : ICreatureStrategy
    {
        public void ApplyStrategy(Creature monster, World world)
        {
            if (monster.Character.Target == null)
            {
                monster.PathStrategy = new RandomPathAi();
                return;
            }
            float distance = Vector2.Distance(monster.Character.Target.Position, monster.Position);
            if (!monster.Character.Entity.IsNoControlable)
            {
                if (monster.NeedsEscapeToRight)
                {
                    if (monster.Position.X > 600)
                        monster.NeedsEscapeToRight = false;
                    monster.playerInput.X = monster.PlayerSpeed;
                    return;
                }
                else if (monster.NeedsEscapeToLeft)
                {
                    if (monster.Position.X < monster.GameMapRef.MapWidth - 700)
                        monster.NeedsEscapeToLeft = false;
                    monster.playerInput.X = -monster.PlayerSpeed;
                    return;
                }
                if (monster.Position.X <= monster.Character.Target.Position.X)
                {
                    if (distance < 200 && monster.Position.X < 150)
                    {
                        monster.playerInput.X = monster.PlayerSpeed;
                        monster.NeedsEscapeToRight = true;
                        return;
                    }
                    monster.playerInput.X = -monster.PlayerSpeed;       
                }
                else
                {
                    if (distance < 200 && monster.Position.X > monster.GameMapRef.MapWidth - 250)
                    {
                        monster.playerInput.X = monster.PlayerSpeed;
                        monster.NeedsEscapeToLeft = true;
                        return;
                    }
                    monster.playerInput.X = monster.PlayerSpeed;
                }

                monster.playerInput.Y = Mechanics.Roll(1, 101) == 100 ? 7f : 0;
            }
        }
    }
}