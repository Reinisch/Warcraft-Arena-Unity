using System;
using Client.Sound;
using Core;
using Unity.Behavior;
using Unity.Properties;
using UnityEngine;
using Zenject;

namespace Client.BehaviorGraph
{
    [Serializable, GeneratePropertyBag]
    [NodeDescription(
        name: "Play Unit Sound",
        description: "Plays a one-shot sound on the renderer of the specified unit.",
        story: "Play [SoundEntry] on [Unit]",
        category: "Action/Unit",
        id: "e5f6a7b8c9d0e1f2a3b4c5d6e7f8a9b0")]
    public class PlayUnitSound : BehaviourGraphAction
    {
        [SerializeReference] public BlackboardVariable<Unit> Unit;
        [SerializeReference] public BlackboardVariable<SoundEntry> SoundEntry;

        [Inject]
        private RenderingReference rendering;

        protected override Status OnStart()
        {
            if (!rendering.UnitRenderers.TryFind(Unit.Value, out UnitRenderer unitRenderer))
                return Status.Success;

            unitRenderer.PlayOneShot(SoundEntry.Value);
            return Status.Success;
        }
    }
}
