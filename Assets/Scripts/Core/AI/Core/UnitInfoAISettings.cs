using UnityEngine;

namespace Core
{
    public abstract class UnitInfoAISettings : ScriptableObject
    {
        public abstract IUnitAIModel CreateAI();
    }
}
