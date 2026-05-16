using Common;
using JetBrains.Annotations;
using UnityEngine;

namespace Core
{
    [UsedImplicitly]
    public class SpellCastBehaviour : UnitStateMachineBehaviour
    {
        [SerializeField, UsedImplicitly] private SpellInfo spellInfo;
        [SerializeField, UsedImplicitly] private int castIntervalMin;
        [SerializeField, UsedImplicitly] private int castIntervalMax;
        [SerializeField, UsedImplicitly] private bool once;

        private TimeTracker castTimeTracker = new TimeTracker();

        protected override void OnStart()
        {
            base.OnStart();

            if (once)
                Unit.Spells.CastSpell(spellInfo, new SpellCastingOptions());
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
                Unit.Spells.CastSpell(spellInfo, new SpellCastingOptions());
                castTimeTracker.Reset(RandomUtils.Next(castIntervalMin, castIntervalMax));
            }
        }
    }
}
