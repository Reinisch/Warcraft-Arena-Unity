using System;
using JetBrains.Annotations;
using UnityEngine;

namespace Core
{
    public class EffectTeleportDirect : SpellEffectInfo
    {
        [SerializeField, UsedImplicitly] private float horizontalDistance;

        public float HorizontalDistance => horizontalDistance;

        public override SpellEffectType EffectType => SpellEffectType.TeleportDirect;
        public override SpellTargetEntities TargetEntityType => SpellTargetEntities.Unit;

        internal override void Handle(Spell spell, int effectIndex, Unit target, SpellEffectHandleMode mode)
        {
            spell.EffectTeleportDirect(this, effectIndex, target, mode);
        }
    }

    public partial class Spell
    {
        internal void EffectTeleportDirect(EffectTeleportDirect effect, int effectIndex, Unit target, SpellEffectHandleMode mode)
        {
            if (mode != SpellEffectHandleMode.LaunchTarget)
                return;

            if (target == null || !target.IsAlive)
                return;

            float topCheck = Math.Abs(target.UnitCollider.bounds.max.y);
            float safeExtentsY = Math.Abs(target.UnitCollider.bounds.extents.y);
            float safeExtentsX = Math.Abs(target.UnitCollider.bounds.extents.x);
            float distance = effect.HorizontalDistance;

            Vector3 targetTop = target.UnitCollider.bounds.center + Vector3.up * topCheck;
            Vector3 targetPosition;

#if UNITY_EDITOR
            Debug.DrawLine(target.UnitCollider.bounds.center, target.UnitCollider.bounds.center + Vector3.up * topCheck, Color.red, 3f);
            #endif

            if (Physics.Raycast(target.UnitCollider.bounds.center, Vector3.up, out RaycastHit hitInfo, topCheck, PhysicsReference.Mask.Ground))
                targetPosition = hitInfo.point - Vector3.up * safeExtentsY;
            else
                targetPosition = targetTop;

            #if UNITY_EDITOR
            Debug.DrawLine(targetPosition, targetPosition + target.transform.forward * distance, Color.red, 3f);
            #endif

            if (Physics.Raycast(targetPosition, target.transform.forward, out hitInfo, distance, PhysicsReference.Mask.Ground))
                targetPosition = hitInfo.point - target.transform.forward * safeExtentsX;
            else
                targetPosition = targetPosition + target.transform.forward * distance;

            #if UNITY_EDITOR
            Debug.DrawLine(targetPosition, targetPosition - Vector3.up * topCheck * 1.5f, Color.red, 3f);
            #endif

            if (Physics.Raycast(targetPosition, -Vector3.up, out hitInfo, topCheck * 1.5f, PhysicsReference.Mask.Ground))
                targetPosition = hitInfo.point;
            else
                targetPosition = targetPosition - Vector3.up * topCheck * 1.5f;

            target.transform.position = targetPosition;
        }
    }
}
