using System;
using UnityEngine;

public class TeleportEffect : IEffect
{
    float horizontalValue;

    public AoeMode AoeMode { get; private set; }

    public TeleportEffect(float newHorizontalValue)
    {
        horizontalValue = newHorizontalValue;
    }

    public void Apply(Unit caster, Unit target, Spell spell, ArenaManager world)
    {
        float topCheck = Math.Abs(target.UnitCollider.bounds.max.y);
        float safeExtentsY = Math.Abs(target.UnitCollider.bounds.extents.y);
        float safeExtentsX = Math.Abs(target.UnitCollider.bounds.extents.x);

        RaycastHit hitInfo;

        var targetTop = target.UnitCollider.bounds.center + Vector3.up * topCheck;
        var targetPosition = targetTop;

#if UNITY_EDITOR
        Debug.DrawLine(target.UnitCollider.bounds.center, target.UnitCollider.bounds.center + Vector3.up * topCheck, Color.red, 3f);
#endif

        if (Physics.Raycast(target.UnitCollider.bounds.center, Vector3.up, out hitInfo, topCheck, 1 << LayerMask.NameToLayer("Ground")))
            targetPosition = hitInfo.point - Vector3.up * safeExtentsY;
        else
            targetPosition = targetTop;

#if UNITY_EDITOR
        Debug.DrawLine(targetPosition, targetPosition + target.transform.forward * horizontalValue, Color.red, 3f);
#endif

        if (Physics.Raycast(targetPosition, target.transform.forward, out hitInfo, horizontalValue, 1 << LayerMask.NameToLayer("Ground")))
            targetPosition = hitInfo.point - target.transform.forward * safeExtentsX;
        else
            targetPosition = targetPosition + target.transform.forward * horizontalValue;

#if UNITY_EDITOR
        Debug.DrawLine(targetPosition, targetPosition - Vector3.up * topCheck * 1.5f, Color.red, 3f);
#endif

        if (Physics.Raycast(targetPosition, -Vector3.up, out hitInfo, topCheck * 1.5f, 1 << LayerMask.NameToLayer("Ground")))
            targetPosition = hitInfo.point;
        else
            targetPosition = targetPosition - Vector3.up * topCheck * 1.5f;

        target.transform.position = targetPosition;
    }
}