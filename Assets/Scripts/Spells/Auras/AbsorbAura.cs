using System;
using UnityEngine;

public class AbsorbAura : IAura
{
    public int absorbAmount;
    public int absorbLeft;

    public Buff Buff { get; set; }

    public AbsorbAura(int newAbsorbAmount, Buff newBuff)
    {
        Buff = newBuff;
        absorbAmount = newAbsorbAmount;
        absorbLeft = absorbAmount;
    }

    public void Apply(Unit unit)
    {
        unit.Character.AbsorbEffects.Add(AbsorbDamage);
    }
    public void Reverse(Unit unit)
    {
        unit.Character.AbsorbEffects.Remove(AbsorbDamage);
    }

    public int AbsorbDamage(Unit unit, ArenaManager world, int damageAmount, out Buff absorbBuff)
    {
        if (absorbLeft > damageAmount)
        {
            absorbLeft -= damageAmount;
            absorbBuff = Buff;
            return 0;
        }
        else
        {
            damageAmount -= absorbLeft;
            absorbLeft = 0;
            absorbBuff = Buff;
            return damageAmount;
        }
    }

    public IAura Clone(Buff newBuff)
    {
        return new AbsorbAura(absorbAmount, newBuff);
    }
    public void Dispose()
    {
        Buff = null;
    }
}