using Common;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.AI;

namespace Core
{
    [UsedImplicitly]
    public class SpellCastInFrontBehaviour: UnitStateMachineBehaviour
    {
        [SerializeField, UsedImplicitly] private SpellInfo spellInfo;
        [SerializeField, UsedImplicitly] private int castIntervalMin;
        [SerializeField, UsedImplicitly] private int castIntervalMax;
        [SerializeField, UsedImplicitly] private bool once;
        [SerializeField, UsedImplicitly] private float range;
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
            Vector3 target = Unit.Position + Unit.transform.forward * range;
            if (!NavMesh.SamplePosition(target, out NavMeshHit hit, MovementUtils.MaxNavMeshSampleRange, MovementUtils.WalkableAreaMask))
                return;
            target = hit.position;

            if (trigger)
            {
                Unit.Spells.TriggerSpell(
                    spellInfo,
                    target);
            }
            else
            {
                Unit.Spells.CastSpell(
                    spellInfo,
                    new SpellCastingOptions(
                        new SpellExplicitTargets
                        {
                            Destination = target
                        }));
            }
        }
    }
}