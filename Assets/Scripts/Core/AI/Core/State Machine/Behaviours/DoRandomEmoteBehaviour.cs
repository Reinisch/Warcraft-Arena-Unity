using System.Collections.Generic;
using Common;
using JetBrains.Annotations;
using UnityEngine;

namespace Core
{
    [UsedImplicitly]
    public class DoRandomEmoteBehaviour : UnitStateMachineBehaviour
    {
        [SerializeField, UsedImplicitly] private List<EmoteType> randomEmotes;
        [SerializeField, UsedImplicitly] private int emoteIntervalMin;
        [SerializeField, UsedImplicitly] private int emoteIntervalMax;

        private TimeTracker emoteTimeTracker = new TimeTracker();

        protected override void OnStart()
        {
            base.OnStart();

            emoteTimeTracker.Reset(RandomUtils.Next(emoteIntervalMin, emoteIntervalMax));
        }

        protected override void OnActiveUpdate(int deltaTime)
        {
            base.OnActiveUpdate(deltaTime);

            emoteTimeTracker.Update(deltaTime);
            if (emoteTimeTracker.Passed && randomEmotes.Count > 0)
            {
                Unit.ModifyEmoteState(RandomUtils.GetRandomElement(randomEmotes));
                emoteTimeTracker.Reset(RandomUtils.Next(emoteIntervalMin, emoteIntervalMax));
            }
        }
    }
}
