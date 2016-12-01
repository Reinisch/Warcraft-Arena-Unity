using System;
using System.Collections.Generic;
using UnityEngine;

public class DamageEffect : IEffect
{
    //DamageType damageType;
    int minValue;
    int maxValue;

    public AoeMode AoeMode { get; private set; }
    public List<SpellModifier> Modifiers { get; private set; }

    public DamageEffect(DamageType newDamageType, int MinValue, int MaxValue)
    {
        //damageType = newDamageType;
        minValue = MinValue;
        maxValue = MaxValue;
        AoeMode = AoeMode.None;
        Modifiers = new List<SpellModifier>();
    }

    public void Apply(Unit caster, Unit target, Spell spell, ArenaManager world)
    {
        int amount = 0;

        amount += RandomSolver.Next(minValue, maxValue);

        if (amount < 1)
            amount = 1;

        int initialDamage = amount;

        caster.Character.parameters.Copy(world.SpellInfo.parameters);
        for (int i = 0; i < Modifiers.Count; i++)
            Modifiers[i].ModifySpell(caster, target, world.SpellInfo, spell, world);

        initialDamage = (int)(initialDamage * world.SpellInfo.parameters[ParameterType.DamageDealing].FinalValue);

        bool isCrit = RandomSolver.CheckSuccess(world.SpellInfo.parameters[ParameterType.CritChance].FinalValue);
        if(isCrit)
            initialDamage = (int)(initialDamage * caster.Character.parameters[ParameterType.CritDamageMultiplier].FinalValue);

        amount = initialDamage;
        Buff absorbBuff;
        for(int i = target.character.AbsorbEffects.Count - 1; i >= 0; i--)
        {
            amount = target.character.AbsorbEffects[i](target, world, amount, out absorbBuff);
            if (amount != 0)
            {
                target.character.Buffs.Remove(absorbBuff);
            }
            else
                return;
        }

        target.Character.health.Decrease((ushort)amount);

        if (caster.id == world.PlayerUnit.id)
        {
            GameObject damageEvent = MonoBehaviour.Instantiate(Resources.Load("Prefabs/UI/DamageEvent")) as GameObject;
            damageEvent.GetComponent<UnitDamageUIEvent>().Initialize(initialDamage, target, isCrit, ArenaManager.PlayerInterface);
        }
    }
}