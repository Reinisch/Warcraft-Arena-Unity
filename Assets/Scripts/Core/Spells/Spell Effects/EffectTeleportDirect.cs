using System;
using JetBrains.Annotations;
using UnityEngine;
using Common;

using EventHandler = Common.EventHandler;

namespace Core
{
    [UsedImplicitly, CreateAssetMenu(fileName = "Effect Teleport Direct", menuName = "Game Data/Spells/Effects/Teleport Direct", order = 5)]
    public class EffectTeleportDirect : SpellEffectInfo
    {
        [SerializeField, UsedImplicitly] private float horizontalDistance;

        public float HorizontalDistance => horizontalDistance;

        public override float Value => HorizontalDistance;
        public override SpellEffectType EffectType => SpellEffectType.TeleportDirect;

        internal override void Handle(Spell spell, int effectIndex, Unit target, SpellEffectHandleMode mode)
        {
            spell.EffectTeleportDirect(this, effectIndex, target, mode);
        }
    }

    public partial class Spell
    {
        internal void EffectTeleportDirect(EffectTeleportDirect effect, int effectIndex, Unit target, SpellEffectHandleMode mode)
        {
            if (mode != SpellEffectHandleMode.HitFinal)
                return;

            if (target == null || !target.IsAlive)
                return;

            float topCheck = Math.Abs(target.UnitCollider.bounds.max.y);
            float safeExtentsY = Math.Abs(target.UnitCollider.bounds.extents.y);
            float safeExtentsX = Math.Abs(target.UnitCollider.bounds.extents.x);
            float distance = effect.HorizontalDistance;

            Vector3 targetTop = target.UnitCollider.bounds.center + Vector3.up * topCheck;
            Vector3 targetPosition;

            Drawing.DrawLine(target.UnitCollider.bounds.center, target.UnitCollider.bounds.center + Vector3.up * topCheck, Color.red, 3f);

            if (Physics.Raycast(target.UnitCollider.bounds.center, Vector3.up, out RaycastHit hitInfo, topCheck, PhysicsReference.Mask.Ground))
                targetPosition = hitInfo.point - Vector3.up * safeExtentsY;
            else
                targetPosition = targetTop;

            Drawing.DrawLine(targetPosition, targetPosition + target.transform.forward * distance, Color.red, 3f);

            if (Physics.Raycast(targetPosition, target.transform.forward, out hitInfo, distance, PhysicsReference.Mask.Ground))
                targetPosition = hitInfo.point - target.transform.forward * safeExtentsX;
            else
                targetPosition = targetPosition + target.transform.forward * distance;

            Drawing.DrawLine(targetPosition, targetPosition - Vector3.up * topCheck * 1.5f, Color.red, 3f);

            if (Physics.Raycast(targetPosition, -Vector3.up, out hitInfo, topCheck * 2f, PhysicsReference.Mask.Ground))
                targetPosition = hitInfo.point;
            else
                targetPosition = targetPosition - Vector3.up * topCheck * 2f;

            if (target is Player player && !player.IsController && player.Motion.HasMovementControl)
                EventHandler.ExecuteEvent(GameEvents.ServerPlayerTeleport, player, targetPosition);
            else
            {
                target.SetMovementFlag(MovementFlags.Ascending, false);
                target.transform.position = targetPosition;
            }
        }
    }
}
