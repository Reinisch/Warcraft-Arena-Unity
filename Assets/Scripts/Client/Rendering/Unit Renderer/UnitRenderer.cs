using Bolt;
using Common;
using Core;
using JetBrains.Annotations;
using UnityEngine;

namespace Client
{
    public sealed partial class UnitRenderer : EntityEventListener<IUnitState>
    {
        [SerializeField, UsedImplicitly] private RenderingReference rendering;
        [SerializeField, UsedImplicitly] private TagContainer tagContainer;
        [SerializeField, UsedImplicitly] private Animator animator;

        private readonly AuraEffectController auraEffectController = new AuraEffectController();

        public TagContainer TagContainer => tagContainer;
        public Animator Animator => animator;
        public Unit Unit { get; private set; }

        public void Initialize(Unit unit)
        {
            Unit = unit;

            Unit.BoltEntity.AddEventListener(this);
            Unit.AddCallback(nameof(IUnitState.DeathState), OnDeathStateChanged);
            Unit.AddCallback(nameof(IUnitState.SpellCast), OnSpellCastChanged);

            if (Unit.IsDead)
            {
                animator.SetBool("IsDead", true);
                animator.Play("Death", 0, 1.0f);
                animator.Play("Death", 1, 1.0f);
            }

            auraEffectController.HandleAttach(this);
        }

        public void Deinitialize()
        {
            auraEffectController.HandleDetach();

            Unit.BoltEntity.RemoveEventListener(this);
            Unit.RemoveCallback(nameof(IUnitState.DeathState), OnDeathStateChanged);
            Unit.RemoveCallback(nameof(IUnitState.SpellCast), OnSpellCastChanged);

            Unit = null;

            Animator.WriteDefaultValues();
        }

        public void DoUpdate(float deltaTime)
        {
            UpdateAnimations(deltaTime);
        }

        public override void OnEvent(UnitSpellLaunchEvent launchEvent)
        {
            base.OnEvent(launchEvent);

            if (!Unit.IsController)
            {
                var token = launchEvent.ProcessingEntries as SpellProcessingToken;
                EventHandler.ExecuteEvent(EventHandler.GlobalDispatcher, GameEvents.SpellLaunched, Unit, launchEvent.SpellId, token, launchEvent.Source);
            }
        }

        public override void OnEvent(UnitSpellDamageEvent spellDamageEvent)
        {
            base.OnEvent(spellDamageEvent);

            Animator.SetBool("WoundedCrit", spellDamageEvent.IsCrit);
            Animator.SetTrigger("Wound");
        }

        public override void OnEvent(UnitSpellHitEvent spellHitEvent)
        {
            base.OnEvent(spellHitEvent);

            EventHandler.ExecuteEvent(EventHandler.GlobalDispatcher, GameEvents.SpellHit, Unit, spellHitEvent.SpellId);
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
            if (!Unit.IsAlive)
            {
                animator.SetBool("IsDead", true);
                return;
            }

            if (!Unit.MovementInfo.HasMovementFlag(MovementFlags.Flying))
            {
                Animator.SetBool("Grounded", true);

                float currentStrafe = Animator.GetFloat("Strafe");
                float strafeTarget = Unit.MovementInfo.HasMovementFlag(MovementFlags.StrafeLeft) ? 0 :
                    Unit.MovementInfo.HasMovementFlag(MovementFlags.StrafeRight) ? 1 : 0.5f;

                float strafeDelta = 2 * Mathf.Sign(strafeTarget - currentStrafe) * deltaTime;
                float resultStrafe = Mathf.Clamp(currentStrafe + strafeDelta, 0.0f, 1.0f);

                if (Mathf.Abs(strafeTarget - currentStrafe) > Mathf.Abs(strafeDelta))
                    Animator.SetFloat("Strafe", resultStrafe);

                if (Unit.MovementInfo.HasMovementFlag(MovementFlags.Forward | MovementFlags.StrafeRight | MovementFlags.StrafeLeft))
                    Animator.SetFloat("Speed", 1);
                else
                    Animator.SetFloat("Speed", Mathf.Clamp(Animator.GetFloat("Speed") - 10 * deltaTime, 0.0f, 1.0f));
            }
            else
                Animator.SetBool("Grounded", false);
        }

        private void OnDeathStateChanged()
        {
            animator.SetBool("IsDead", Unit.IsDead);
        }

        private void OnSpellCastChanged()
        {
            animator.SetBool("Casting", Unit.SpellCast.State.Id != 0);
        }
    }
}