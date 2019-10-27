using Core;
using JetBrains.Annotations;
using UnityEngine;

namespace Client
{
    public sealed class UnitModel : MonoBehaviour
    {
        [SerializeField, UsedImplicitly] private TagContainer tagContainer;
        [SerializeField, UsedImplicitly] private Animator animator;
        [SerializeField, UsedImplicitly] private float strafeSpeed = 1.0f;

        public TagContainer TagContainer => tagContainer;
        public Animator Animator => animator;
        public UnitRenderer Renderer { get; private set; }
        public UnitModelSettings Settings { get; private set; }

        public void Initialize(UnitRenderer unitRenderer, UnitModelSettings modelSettings)
        {
            transform.SetParent(unitRenderer.transform, false);
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;
            transform.localScale = Vector3.one;

            Renderer = unitRenderer;
            Settings = modelSettings;

            if (Renderer.Unit.IsDead)
            {
                animator.SetBool("IsDead", true);
                animator.Play("Death");
            }
        }

        public void Deinitialize()
        {
            Animator.WriteDefaultValues();

            Renderer = null;
        }

        public void DoUpdate(float deltaTime)
        {
            UpdateAnimations(deltaTime);
        }

        public void TriggerInstantCast()
        {
            if (Animator.GetBool(AnimatorUtils.ResurrectingAnimationParam) || Animator.GetBool(AnimatorUtils.DyingAnimationParam))
                return;

            Animator.Play(AnimatorUtils.SpellCastAnimationState, 0, 0.1f);
            Animator.ResetTrigger(AnimatorUtils.SpellCastAnimationTrigger);

            // Switch leg animation for casting
            if (!animator.GetBool("Grounded"))
                animator.Play("Air", 1);
            else if (animator.GetFloat("Speed") > 0.1f)
                animator.Play("Run", 1);
            else
                animator.Play("Cast", 1, 0.1f);
        }

        private void UpdateAnimations(float deltaTime)
        {
            if (!Renderer.Unit.IsAlive)
            {
                animator.SetBool("IsDead", true);
                return;
            }

            if (!Renderer.Unit.HasMovementFlag(MovementFlags.Flying))
            {
                Animator.SetBool("Grounded", true);

                float currentStrafe = Animator.GetFloat("Strafe");
                float strafeTarget = Renderer.Unit.HasMovementFlag(MovementFlags.StrafeLeft) ? 0 :
                    Renderer.Unit.HasMovementFlag(MovementFlags.StrafeRight) ? 1 : 0.5f;

                float strafeDelta = 2 * Mathf.Sign(strafeTarget - currentStrafe) * deltaTime * strafeSpeed;
                float resultStrafe = Mathf.Clamp(currentStrafe + strafeDelta, 0.0f, 1.0f);

                if (Mathf.Abs(strafeTarget - currentStrafe) > Mathf.Abs(strafeDelta))
                    Animator.SetFloat("Strafe", resultStrafe);

                if (Renderer.Unit.HasMovementFlag(MovementFlags.Forward | MovementFlags.StrafeRight | MovementFlags.StrafeLeft))
                    Animator.SetFloat("Speed", 1);
                else
                    Animator.SetFloat("Speed", Mathf.Clamp(Animator.GetFloat("Speed") - 10 * deltaTime, 0.0f, 1.0f));
            }
            else
                Animator.SetBool("Grounded", false);
        }
    }
}