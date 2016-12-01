using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public delegate int AbsorbEffect(Unit unit, ArenaManager world, int damage, out Buff absorbBuff);
public delegate void ScriptEffect(SpellScriptInvokeArgs spellArgs);

public interface IAura
{
    void Apply(Unit unit);
    void Reverse(Unit unit);

    void Dispose();
    IAura Clone(Buff buff);
}
