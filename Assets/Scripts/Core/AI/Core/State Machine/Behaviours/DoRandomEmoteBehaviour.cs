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
        [SerializeField, UsedImplicitly] private bool oneTime;

        private TimeTracker emoteTimeTracker = new TimeTracker();
        private bool doneSingle;

        protected override void OnStart()
        {
            base.OnStart();

            emoteTimeTracker.Reset(RandomUtils.Next(emoteIntervalMin, emoteIntervalMax));
            doneSingle = false;
        }

        protected override void OnActiveUpdate(int deltaTime)
        {
            base.OnActiveUpdate(deltaTime);

            if (oneTime && doneSingle)
                return;

            emoteTimeTracker.Update(deltaTime);
            if (emoteTimeTracker.Passed && randomEmotes.Count > 0)
            {
                Unit.ModifyEmoteState(RandomUtils.GetRandomElement(randomEmotes));
                doneSingle = true;
                emoteTimeTracker.Reset(RandomUtils.Next(emoteIntervalMin, emoteIntervalMax));
            }
        }
    }
}
