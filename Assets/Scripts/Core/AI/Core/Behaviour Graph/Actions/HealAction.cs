using System;
using Unity.Behavior;
using Unity.Properties;
using UnityEngine;

namespace Core.BehaviorGraph
{
    [Serializable, GeneratePropertyBag]
    [NodeDescription(
        name: "Heal",
        description: "Heals the unit by a random amount within the given flat and ratio ranges.",
        story: "[Unit] heals by ratio [MaxRatio]",
        category: "Action/Unit",
        id: "c3d4e5f6a7b8c9d0e1f2a3b4c5d6e7f9")]
    public class HealAction : BehaviourGraphAction
    {
        [SerializeReference] public BlackboardVariable<Unit> Unit;
        [SerializeReference] public BlackboardVariable<float> MaxRatio;

        protected override Status OnStart()
        {
            int healing = (int) (MaxRatio.Value * Unit.Value.MaxHealth);
            Unit.Value.DealHeal(Unit.Value, healing);

            return Status.Success;
        }
    }
}
