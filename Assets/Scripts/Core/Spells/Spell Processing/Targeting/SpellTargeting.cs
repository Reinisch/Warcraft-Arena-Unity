using UnityEngine;

namespace Core
{
    public abstract class SpellTargeting : ScriptableObject
    {
        internal abstract void SelectTargets(Spell spell, int effectMask);
    }
}
