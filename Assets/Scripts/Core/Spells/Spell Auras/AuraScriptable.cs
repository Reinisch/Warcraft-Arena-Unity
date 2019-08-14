using UnityEngine;

namespace Core
{
    internal abstract class AuraScriptable : ScriptableObject
    {
        public virtual void AuraApplicationApplied(AuraApplication application)
        {
        }

        public virtual void AuraApplicationRemoved(AuraApplication application)
        {
        }
    }
}
