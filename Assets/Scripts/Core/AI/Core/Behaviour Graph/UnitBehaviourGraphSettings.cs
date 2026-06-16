using JetBrains.Annotations;
using Unity.Behavior;
using UnityEngine;

namespace Core
{
    [UsedImplicitly, CreateAssetMenu(fileName = "Unit AI - Behaviour Graph", menuName = "Game Data/AI/Behaviour Graph", order = 2)]
    public sealed class UnitBehaviourGraphSettings : UnitInfoAISettings
    {
        [field: SerializeField]
        public BehaviorGraphAgent GraphPrefab { get; private set; }

        public override IUnitAIModel CreateAI() => new UnitBehaviourGraph(this);
    }
}
