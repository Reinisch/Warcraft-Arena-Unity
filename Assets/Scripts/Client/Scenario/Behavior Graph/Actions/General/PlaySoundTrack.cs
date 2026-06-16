using Core;
using System;
using Client.Sound;
using Unity.Behavior;
using Unity.Properties;
using UnityEngine;

namespace Client.BehaviorGraph
{
    [Serializable, GeneratePropertyBag]
    [NodeDescription(
        name: "Play Sound Track",
        description: "Plays an audio source as a looping soundtrack between startLoopTime and endLoopTime. Runs until the node is interrupted.",
        story: "Play soundtrack [SoundTrackSource] looping [StartLoopTime] to [EndLoopTime]",
        category: "Action/Map",
        id: "e1f2a3b4c5d6e7f8a911c1d2e3f4a5b6")]
    public class PlaySoundTrack : BehaviourGraphAction
    {
        [SerializeReference] public BlackboardVariable<GameObject> Owner;
        [SerializeReference] public BlackboardVariable<SoundEntry> Soundtrack;
        [SerializeReference] public BlackboardVariable<float> StartLoopTime = new(30f);
        [SerializeReference] public BlackboardVariable<float> EndLoopTime = new(120f);

        private SoundPlayHandle soundtrackPlayHandle;

        protected override Status OnStart()
        {
            soundtrackPlayHandle = Soundtrack.Value.Play(Owner.Value.transform.position, Owner.Value.transform);
               
            return Status.Running;
        }

        protected override void OnEnd()
        {
            soundtrackPlayHandle.Release();
        }

        protected override Status OnUpdate()
        {
            soundtrackPlayHandle.Loop(StartLoopTime.Value, EndLoopTime.Value);

            return Status.Running;
        }
    }
}
