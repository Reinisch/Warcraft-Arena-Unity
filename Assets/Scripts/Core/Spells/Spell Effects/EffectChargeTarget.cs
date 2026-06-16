using System;
using Common;
using JetBrains.Annotations;
using UnityEngine;

namespace Core
{
    [UsedImplicitly, CreateAssetMenu(fileName = "Effect Charge Target", menuName = "Game Data/Spells/Effects/Charge Target", order = 1)]
    public class EffectChargeTarget : SpellEffectInfo
    {
        public enum Kind
        {
            Charge,
            Pounce
        }

        [Header("Charge Target")]
        [SerializeField, UsedImplicitly]
        private float chargeSpeed;
        [SerializeField, UsedImplicitly]
        private Kind kind = Kind.Charge;
        [Tooltip("Pounce only: how far behind the target to land (the caster leaps over it and turns to face it).")]
        [SerializeField, UsedImplicitly]
        private float pounceBehindDistance = 2.5f;

        public float ChargeSpeed => chargeSpeed;
        public Kind ChargeKind => kind;
        public float PounceBehindDistance => pounceBehindDistance;

        public override bool IgnoresSpellImmunity => true;
        public override float Value => 1.0f;
        public override SpellEffectType EffectType => SpellEffectType.Charge;

        internal override void Handle(Spell spell, int effectIndex, Unit target, SpellEffectHandleMode mode)
        {
            spell.EffectChargeTarget(this, target, mode);
        }
    }

    public partial class Spell
    {
        internal void EffectChargeTarget(EffectChargeTarget effect, Unit target, SpellEffectHandleMode mode)
        {
            if (mode != SpellEffectHandleMode.HitFinal || target == null || OriginalCaster == null)
                return;

            Vector3 chargePoint;
            switch (SpellInfo.ExplicitTargetType)
            {
                case SpellExplicitTargetType.Target when ExplicitTargets.Target != null:
                    chargePoint = ExplicitTargets.Target.Position;
                    break;
                case SpellExplicitTargetType.Destination when ExplicitTargets.Destination.HasValue: 
                    chargePoint = ExplicitTargets.Destination.Value;
                    break;
                default:
                    Assert.Fail($"Unexpected explicit targeting for charging: {SpellInfo.ExplicitTargetType} in spell: {SpellInfo.name}");
                    return;
            }

            switch (effect.ChargeKind)
            {
                case Core.EffectChargeTarget.Kind.Charge:
                    Caster.Motion.StartChargingMovement(chargePoint, effect.ChargeSpeed);
                    break;
                case Core.EffectChargeTarget.Kind.Pounce:
                    // Leap to a point BEHIND the target (continuing the caster→target approach) and face the
                    // target on landing. Clamp the landing spot to the navmesh so we don't end up in geometry.
                    Vector3 approach = Vector3.ProjectOnPlane(chargePoint - Caster.Position, Vector3.up);
                    Vector3 approachDir = approach.sqrMagnitude > 0.0001f ? approach.normalized : Caster.Rotation * Vector3.forward;
                    Vector3 behindPoint = chargePoint + approachDir * effect.PounceBehindDistance;
                    if (UnityEngine.AI.NavMesh.SamplePosition(behindPoint, out UnityEngine.AI.NavMeshHit hit,
                            MovementUtils.MaxChargeSampleRange, MovementUtils.WalkableAreaMask))
                        behindPoint = hit.position;
                    Caster.Motion.StartPounceMovement(behindPoint, chargePoint, effect.ChargeSpeed);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}