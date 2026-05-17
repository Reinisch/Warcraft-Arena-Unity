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
        [SerializeField, UsedImplicitly] private bool dontRepeat;

        private TimeTracker emoteTimeTracker = new TimeTracker();
        private bool doneSingle;
        private EmoteType previousEmote;

        protected override void OnStart()
        {
            base.OnStart();

            previousEmote = EmoteType.None;
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
                Unit.SpellCast?.Cancel();

                var potentialEmotes = new List<EmoteType>(randomEmotes);
                if (dontRepeat && potentialEmotes.Count > 1)
                    potentialEmotes.Remove(previousEmote);
                EmoteType nextEmote = RandomUtils.GetRandomElement(potentialEmotes);

                Unit.ModifyEmoteState(nextEmote);
                previousEmote = nextEmote;
                doneSingle = true;
                emoteTimeTracker.Reset(RandomUtils.Next(emoteIntervalMin, emoteIntervalMax));
            }
        }
    }
}
