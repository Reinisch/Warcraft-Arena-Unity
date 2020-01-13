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

        public float ChargeSpeed => chargeSpeed;
        public Kind ChargeKind => kind;

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
                    Caster.Motion.StartPounceMovement(chargePoint, effect.ChargeSpeed);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}