using System;

using BasicRpgEngine.Characters;
using BasicRpgEngine.Characters.AI;


namespace BasicRpgEngine.Characters.AI
{
    public class RandomPathAi : ICreatureStrategy
    {
        public void ApplyStrategy(Creature monster, World world)
        {
            if (!monster.Character.Entity.IsNoControlable)
            {
                if (monster.MoveTimer.TotalSeconds < 0)
                {
                    monster.MoveTimer = new TimeSpan(0, 0, 0, 0, 600);
                    if (monster.Position.X < monster.GameMapRef.MapWidth / 2)
                        monster.playerInput.X = Mechanics.Roll(-1, 3) > 0 ? monster.PlayerSpeed : -monster.PlayerSpeed;
                    else
                        monster.playerInput.X = Mechanics.Roll(-1, 3) > 0 ? -monster.PlayerSpeed : monster.PlayerSpeed;
                }

                monster.playerInput.Y = Mechanics.Roll(1, 201) == 200 ? 7f : 0;
                monster.playerInput.Y = Mechanics.Roll(1, 201) == 200 ? -7f : 0;
            }
        }
    }
}