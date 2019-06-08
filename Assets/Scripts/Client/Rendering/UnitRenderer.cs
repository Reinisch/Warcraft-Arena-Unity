using Bolt;
using Client;
using Common;
using Core;
using JetBrains.Annotations;
using UnityEngine;

public class UnitRenderer : EntityEventListener<IUnitState>
{
    [SerializeField, UsedImplicitly] private EffectTagPositioner effectTagPositioner;
    [SerializeField, UsedImplicitly] private Animator animator;

    private Animator Animator => animator;
    private Unit Unit { get; set; }

    public EffectTagPositioner EffectTagPositioner => effectTagPositioner;

    public void Initialize(Unit unit)
    {
        Unit = unit;

        Unit.BoltEntity.AddEventListener(this);
    }

    public void Deinitialize()
    {
        Unit.BoltEntity.RemoveEventListener(this);

        Unit = null;
    }

    public void DoUpdate(int deltaTime)
    {
        UpdateAnimations(deltaTime);
    }

    public override void OnEvent(UnitSpellCastEvent spellCastEvent)
    {
        base.OnEvent(spellCastEvent);

        EventHandler.ExecuteEvent(EventHandler.GlobalDispatcher, GameEvents.SpellCasted, Unit, spellCastEvent.SpellId);
    }

    private void UpdateAnimations(int deltaTime)
    {
        if (Unit.DeathState != DeathState.Alive)
            animator.SetTrigger("Death");

        if (!Unit.MovementInfo.HasMovementFlag(MovementFlags.Flying))
        {
            Animator.SetBool("Grounded", true);

            float currentStrafe = Animator.GetFloat("Strafe");
            float strafeTarget = Unit.MovementInfo.HasMovementFlag(MovementFlags.StrafeLeft) ? 0 :
                 Unit.MovementInfo.HasMovementFlag(MovementFlags.StrafeRight) ? 1 : 0.5f;

            float deltaSeconds = (float)deltaTime / 1000;
            float strafeDelta = 2 * Mathf.Sign(strafeTarget - currentStrafe) * deltaSeconds;
            float resultStrafe = Mathf.Clamp(currentStrafe + strafeDelta, 0.0f, 1.0f);

            if (Mathf.Abs(strafeTarget - currentStrafe) > Mathf.Abs(strafeDelta))
                Animator.SetFloat("Strafe", resultStrafe);

            if (Unit.MovementInfo.HasMovementFlag(MovementFlags.Forward | MovementFlags.StrafeRight | MovementFlags.StrafeLeft))
                Animator.SetFloat("Speed", 1);
            else
                Animator.SetFloat("Speed", Mathf.Clamp(Animator.GetFloat("Speed") - 10 * deltaSeconds, 0.0f, 1.0f));
        }
        else
            Animator.SetBool("Grounded", false);
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