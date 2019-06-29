using Bolt;
using Common;
using Core;
using JetBrains.Annotations;
using UnityEngine;

namespace Client
{
    public class UnitRenderer : EntityEventListener<IUnitState>
    {
        [SerializeField, UsedImplicitly] private TagContainer tagContainer;
        [SerializeField, UsedImplicitly] private Animator animator;

        private Unit Unit { get; set; }
        private IUnitState UnitState { get; set; }

        public TagContainer TagContainer => tagContainer;
        public Animator Animator => animator;

        public void Initialize(Unit unit)
        {
            Unit = unit;

            Unit.BoltEntity.AddEventListener(this);
            UnitState = entity.GetState<IUnitState>();
            UnitState.AddCallback(nameof(UnitState.DeathState), OnDeathStateChanged);
            UnitState.AddCallback(nameof(UnitState.SpellCast), OnSpellCastChanged);

            if (Unit.IsDead)
            {
                animator.SetBool("IsDead", true);
                animator.Play("Death", 0, 1.0f);
                animator.Play("Death", 1, 1.0f);
            }
        }

        public void Deinitialize()
        {
            Unit.BoltEntity.RemoveEventListener(this);
            UnitState.RemoveAllCallbacks();

            Unit = null;
            UnitState = null;

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
                EventHandler.ExecuteEvent(EventHandler.GlobalDispatcher, GameEvents.SpellLaunched, Unit, launchEvent.SpellId, token);
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
            animator.SetBool("Casting", UnitState.SpellCast.Id != 0);
        }

        [UsedImplicitly]
        private void TriggerInstantCast()
        {
            // Switch leg animation for casting
            if (animator.GetBool("Grounded"))
                animator.Play(animator.GetFloat("Speed") > 0.1f ? "Run" : "Cast", 1);
            else
                animator.Play("Air", 1);
        }
    }
}