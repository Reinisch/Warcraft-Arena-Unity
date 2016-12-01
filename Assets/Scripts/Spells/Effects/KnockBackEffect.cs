using System;
using UnityEngine;

public class KnockBackEffect : IEffect
{
    int horizontalValue;
    int verticalValue;

    public AoeMode AoeMode { get; private set; }

    public KnockBackEffect(int horizontalValue, int verticalValue)
    {
        this.horizontalValue = horizontalValue;
        this.verticalValue = verticalValue;
        AoeMode = AoeMode.None;
    }

    public void Apply(Unit caster, Unit target, Spell spell, ArenaManager world)
    {
        var direction = Vector3.Cross(caster.transform.up, (target.transform.position - caster.transform.position).normalized);

        target.GetComponent<Rigidbody>().MovePosition(target.transform.position + 
            new Vector3(direction.x * horizontalValue, direction.y * verticalValue, direction.z * horizontalValue));
    }
}