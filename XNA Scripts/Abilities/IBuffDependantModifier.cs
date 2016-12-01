using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BasicRpgEngine.Spells
{
    public interface IBuffDependantModifier
    {
        string SpellName { get; }
        Buff BuffRef { get; }
    }
}