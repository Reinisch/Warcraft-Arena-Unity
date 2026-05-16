using Common;
using JetBrains.Annotations;
using UnityEngine;

namespace Core
{
    [UsedImplicitly]
    public class RagnarosBossfightBehaviour: UnitStateMachineBehaviour
    {
        [SerializeField, UsedImplicitly] private string healthRatioName;
        [SerializeField, UsedImplicitly] private string deadName;

        private int healthRatioHash;
        private int deadHash;

        protected override void OnRegister()
        {
            base.OnRegister();

            healthRatioHash = Animator.StringToHash(healthRatioName);
            deadHash = Animator.StringToHash(deadName);
        }

        protected override void OnActiveUpdate(int deltaTime)
        {
            base.OnActiveUpdate(deltaTime);

            StateAnimator.SetFloat(healthRatioHash, Unit.HealthRatio);
            StateAnimator.SetBool(deadHash, Unit.IsDead);
        }
    }
}