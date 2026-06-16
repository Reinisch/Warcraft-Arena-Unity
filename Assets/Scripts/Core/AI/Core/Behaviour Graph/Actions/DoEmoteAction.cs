using System;
using Unity.Behavior;
using Unity.Properties;
using UnityEngine;

namespace Core.BehaviorGraph
{
    [Serializable, GeneratePropertyBag]
    [NodeDescription(
        name: "Do Emote",
        description: "Triggers an emote on the unit, optionally cancelling any current spell cast first.",
        story: "[Unit] performs emote [EmoteType]",
        category: "Action/Unit",
        id: "a1b2c3d4e5f6a7b8c9d0e1f2a3b4c5d7")]
    public class DoEmoteAction : BehaviourGraphAction
    {
        [SerializeReference] public BlackboardVariable<Unit> Unit;
        [SerializeReference] public BlackboardVariable<EmoteType> EmoteType;
        [SerializeReference] public BlackboardVariable<bool> CancelCasting;

        protected override Status OnStart()
        {
            if (CancelCasting.Value)
                Unit.Value.SpellCast?.Cancel();

            Unit.Value.ModifyEmoteState(EmoteType.Value);

            return Status.Success;
        }
    }
}
