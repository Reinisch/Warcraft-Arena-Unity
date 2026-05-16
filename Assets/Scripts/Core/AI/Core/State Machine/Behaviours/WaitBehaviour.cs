using Common;
using JetBrains.Annotations;
using UnityEngine;

namespace Core
{
    [UsedImplicitly]
    public class WaitBehaviour: UnitStateMachineBehaviour
    {
        [SerializeField, UsedImplicitly] private string continueName;
        [SerializeField, UsedImplicitly] private int min;
        [SerializeField, UsedImplicitly] private int max;

        private TimeTracker timeTracker;
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
            timeTracker.Reset(RandomUtils.Next(min, max));
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
                timeTracker.Update(deltaTime);

                if (timeTracker.Passed)
                {
                    StateAnimator.SetBool(continueHash, true);
                    done = true;
                }
            }
        }
    }
}