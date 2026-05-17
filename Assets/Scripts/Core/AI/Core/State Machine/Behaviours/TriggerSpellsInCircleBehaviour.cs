using Common;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.AI;

namespace Core
{
    [UsedImplicitly]
    public class TriggerSpellsInCircleBehaviour: UnitStateMachineBehaviour
    {
        [SerializeField, UsedImplicitly] private SpellInfo spellInfo;
        [SerializeField, UsedImplicitly] private string continueName;
        [SerializeField, UsedImplicitly] private float minRadius;
        [SerializeField, UsedImplicitly] private float maxRadius;
        [SerializeField, UsedImplicitly] private int attemptsPerTrigger = 5;
        [SerializeField, UsedImplicitly] private int castIntervalMin;
        [SerializeField, UsedImplicitly] private int castIntervalMax;
        [SerializeField, UsedImplicitly] private int totalTime;
        [SerializeField, UsedImplicitly] private SpellCastFlags extraFlags;

        private TimeTracker castTimeTracker = new TimeTracker();
        private TimeTracker totalTimeTracker = new TimeTracker();
        private int continueHash;
        private bool done;

        protected override void OnRegister()
        {
            base.OnRegister();

            continueHash = Animator.StringToHash(continueName);
        }

        protected override void OnStart()
        {
            base.OnStart();

            done = false;
            castTimeTracker.Reset(RandomUtils.Next(castIntervalMin, castIntervalMax));
            totalTimeTracker.Reset(totalTime);
        }

        protected override void OnExit()
        {
            StateAnimator.SetBool(continueHash, false);

            base.OnExit();
        }

        protected override void OnActiveUpdate(int deltaTime)
        {
            base.OnActiveUpdate(deltaTime);

            if (!done)
            {
                castTimeTracker.Update(deltaTime);
                totalTimeTracker.Update(deltaTime);

                if (totalTimeTracker.Passed)
                {
                    StateAnimator.SetBool(continueHash, true);
                    done = true;
                }

                if (castTimeTracker.Passed)
                {
                    AttemptToTriggerSpell();
                    castTimeTracker.Reset(RandomUtils.Next(castIntervalMin, castIntervalMax));
                }
            }
        }

        private void AttemptToTriggerSpell()
        {
            for (int i = 0; i < attemptsPerTrigger; i++)
            {
                float randomOrientation = RandomUtils.Next(0.0f, 360.0f);
                float randomRadius = RandomUtils.Next(minRadius, maxRadius);
                Vector3 target = Unit.Position + Unit.Rotation * 
                    Quaternion.Euler(0, randomOrientation, 0) * Vector3.forward * randomRadius;

                if (!NavMesh.SamplePosition(target, out NavMeshHit hit, MovementUtils.MaxNavMeshSampleRange, MovementUtils.WalkableAreaMask))
                    continue;

                Unit.Spells.TriggerSpell(spellInfo, hit.position, extraFlags);
                break;
            }
        }
    }
}