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

        private TimeTracker castTimeTracker = new TimeTracker();

        protected override void OnStart()
        {
            base.OnStart();

            castTimeTracker.Reset(RandomUtils.Next(castIntervalMin, castIntervalMax));
        }

        protected override void OnActiveUpdate(int deltaTime)
        {
            base.OnActiveUpdate(deltaTime);

            castTimeTracker.Update(deltaTime);
            if (castTimeTracker.Passed)
            {
                Unit.Spells.CastSpell(spellInfo, new SpellCastingOptions());
                castTimeTracker.Reset(RandomUtils.Next(castIntervalMin, castIntervalMax));
            }
        }
    }
}
