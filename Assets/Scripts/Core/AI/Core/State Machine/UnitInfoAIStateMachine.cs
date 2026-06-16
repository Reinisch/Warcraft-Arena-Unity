using JetBrains.Annotations;
using UnityEngine;

namespace Core
{
    [UsedImplicitly, CreateAssetMenu(fileName = "Unit AI - State Machine", menuName = "Game Data/AI/State Machine", order = 1)]
    public sealed class UnitInfoAIStateMachine : UnitInfoAISettings
    {
        [SerializeField, UsedImplicitly] private Animator prototype;

        public Animator Prototype => prototype;

        public override IUnitAIModel CreateAI() => new UnitStateMachine(this);
    }
}
