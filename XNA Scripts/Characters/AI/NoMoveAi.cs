using Microsoft.Xna.Framework;

using BasicRpgEngine.Characters;
using BasicRpgEngine.Characters.AI;

namespace BasicRpgEngine.Characters.AI
{
    public class NoMoveAi : ICreatureStrategy
    {
        public void ApplyStrategy(Creature monster, World world)
        {
            if (monster.Character.Target == null)
            {
                monster.PathStrategy = new RandomPathAi();
                return;
            }
            if (!monster.Character.Entity.IsNoControlable)
            {
                if (monster.Position.X < monster.Character.Target.Position.X)
                    monster.playerInput = Vector2.Zero;
            }
        }
    }
}