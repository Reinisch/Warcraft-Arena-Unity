using UnityEngine;

namespace Core
{
    public abstract class ConditionalModiferValue : ScriptableObject
    {
        public abstract void Modify(Unit caster, Unit target, ref float value);
    }
}
