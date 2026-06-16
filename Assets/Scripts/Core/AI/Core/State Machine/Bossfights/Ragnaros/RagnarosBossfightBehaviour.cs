using JetBrains.Annotations;
using UnityEngine;

namespace Core
{
    [UsedImplicitly]
    public class RagnarosBossfightBehaviour: UnitStateMachineBehaviour
    {
        [SerializeField, UsedImplicitly] private string healthRatioName;
        [SerializeField, UsedImplicitly] private string introPhaseName;
        [SerializeField, UsedImplicitly] private string firstPhaseName;
        [SerializeField, UsedImplicitly] private string secondPhaseName;
        [SerializeField, UsedImplicitly] private float firstPhaseHp;
        [SerializeField, UsedImplicitly] private float secondPhaseHp;
        [SerializeField, UsedImplicitly] private string deadName;

        private int healthRatioHash;
        private int introPhaseHash;
        private int firstPhaseHash;
        private int secondPhaseHash;
        private int deadHash;

        protected override void OnRegister()
        {
            base.OnRegister();

            healthRatioHash = Animator.StringToHash(healthRatioName);
            introPhaseHash = Animator.StringToHash(introPhaseName);
            firstPhaseHash = Animator.StringToHash(firstPhaseName);
            secondPhaseHash = Animator.StringToHash(secondPhaseName);
            deadHash = Animator.StringToHash(deadName);
        }

        protected override void OnExit()
        {
            StateAnimator.SetBool(introPhaseHash, false);
            StateAnimator.SetBool(firstPhaseHash, false);
            StateAnimator.SetBool(secondPhaseHash, false);

            base.OnExit();
        }

        protected override void OnActiveUpdate(int deltaTime)
        {
            base.OnActiveUpdate(deltaTime);

            StateAnimator.SetFloat(healthRatioHash, Unit.HealthRatio);
            StateAnimator.SetBool(deadHash, Unit.IsDead);

            if (Unit.IsDead)
            {
                StateAnimator.SetBool(introPhaseHash, false);
                StateAnimator.SetBool(firstPhaseHash, false);
                StateAnimator.SetBool(secondPhaseHash, false);
            }
            else
            {
                bool isSecondPhaseThreshold = Unit.HealthRatio < secondPhaseHp;
                bool isFirstPhaseThreshold = Unit.HealthRatio < firstPhaseHp;
                bool isIntroPhaseThreshold = !isFirstPhaseThreshold;
                StateAnimator.SetBool(introPhaseHash, isIntroPhaseThreshold);
                StateAnimator.SetBool(firstPhaseHash, isFirstPhaseThreshold && !isSecondPhaseThreshold);
                StateAnimator.SetBool(secondPhaseHash, isSecondPhaseThreshold);
            }
        }
    }
}