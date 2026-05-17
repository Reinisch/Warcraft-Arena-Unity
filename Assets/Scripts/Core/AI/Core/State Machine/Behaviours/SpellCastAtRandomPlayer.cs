using System.Collections.Generic;
using Common;
using JetBrains.Annotations;
using UnityEngine;

namespace Core
{
    [UsedImplicitly]
    public class SpellCastAtRandomPlayer : UnitStateMachineBehaviour
    {
        [SerializeField, UsedImplicitly] private SpellInfo spellInfo;
        [SerializeField, UsedImplicitly] private int castIntervalMin;
        [SerializeField, UsedImplicitly] private int castIntervalMax;
        [SerializeField, UsedImplicitly] private bool once;
        [SerializeField, UsedImplicitly] private bool aliveTargets;
        [SerializeField, UsedImplicitly] private bool nearbyTargets;
        [SerializeField, UsedImplicitly] private bool trigger;

        private TimeTracker castTimeTracker;

        protected override void OnStart()
        {
            base.OnStart();

            if (once)
                Cast();
            else
                castTimeTracker.Reset(RandomUtils.Next(castIntervalMin, castIntervalMax));
        }

        protected override void OnActiveUpdate(int deltaTime)
        {
            base.OnActiveUpdate(deltaTime);

            if (once)
                return;

            castTimeTracker.Update(deltaTime);
            if (castTimeTracker.Passed)
            {
                Cast();
                castTimeTracker.Reset(RandomUtils.Next(castIntervalMin, castIntervalMax));
            }
        }

        private void Cast()
        {
            Player player = nearbyTargets
                ? Unit.World.UnitManager.FindNearby<Player>(Unit.Position, TargetPredicate)
                : Unit.World.UnitManager.FindRandom<Player>(TargetPredicate);
            if (player == null)
                return;

            if (trigger)
            {
                Unit.Spells.TriggerSpell(
                    spellInfo,
                    player);
            }
            else
            {
                Unit.Spells.CastSpell(
                    spellInfo,
                    new SpellCastingOptions(
                        new SpellExplicitTargets
                        {
                            Target = player
                        }));
            }
        }

        private bool TargetPredicate(Player player)
        {
            if (aliveTargets && player.IsDead)
                return false;

            return true;
        }
    }
}