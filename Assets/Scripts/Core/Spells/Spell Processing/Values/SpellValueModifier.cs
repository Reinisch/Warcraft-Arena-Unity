using UnityEngine;

namespace Core
{
    public abstract class SpellValueModifier : ScriptableObject
    {
        internal abstract void Modify(ref SpellValue spellValue);
    }
}
