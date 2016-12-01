using Microsoft.Xna.Framework;

using BasicRpgEngine.Characters;
using BasicRpgEngine.Characters.AI;

namespace BasicRpgEngine.Characters.AI
{
    public class RunToTargetAi : ICreatureStrategy
    {
        public void ApplyStrategy(Creature monster, World world)
        {
            float temp;
            if (monster.Character.Target == null)
            {
                monster.PathStrategy = new RandomPathAi();
                monster.NeedsElevation = false;
                return;
            }
            if (monster.NeedsElevation)
            {
                if (monster.BoundRect.BottomCenter.Y <= monster.Character.Target.BoundRect.BottomCenter.Y || Vector2.Distance(monster.ElevationDestination, monster.BoundRect.BottomCenter) < 10f)
                {
                    monster.NeedsElevation = false;
                }
                else
                {
                    temp = monster.BoundRect.BottomCenter.X - monster.ElevationDestination.X;
                    if (temp > 30)
                        monster.playerInput.X = -monster.PlayerSpeed;
                    else if (temp < -30)
                        monster.playerInput.X = monster.PlayerSpeed;
                    else
                        monster.playerInput.X = 0;
                    monster.playerInput.Y = -7f;
                }
                return;
            }
            if (!monster.Character.Entity.IsNoControlable)
            {
                temp = monster.BoundRect.BottomCenter.X - monster.Character.Target.BoundRect.BottomCenter.X;
                if (temp > 40)
                {
                    if (temp > 80)
                        monster.playerInput.X = -monster.PlayerSpeed;
                }
                else if (temp < -40)
                {
                    if (temp < -80)
                        monster.playerInput.X = monster.PlayerSpeed;
                }
                else
                    monster.playerInput.X = 0;

                temp = monster.BoundRect.BottomCenter.Y - monster.Character.Target.BoundRect.BottomCenter.Y;
                if (temp > 40)
                {
                    if (temp > 240)
                    {
                        Vector2 destination;
                        if (monster.GameMapRef.GetNearestElevetion(monster.BoundRect.BottomCenter, out destination))
                        {
                            monster.ElevationDestination = destination;
                            monster.NeedsElevation = true;
                            return;
                        }
                    }
                    else if (temp > 60)
                        monster.playerInput.Y = -7f;
                }
                else if (temp < -40)
                {
                    if (temp < -60)
                        monster.playerInput.Y = 7f;
                }
                else
                    monster.playerInput.Y = 0;
            }
        }
    }
}