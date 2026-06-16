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
        [SerializeField, UsedImplicitly] private string continueName;
        [SerializeField, UsedImplicitly] private int emoteCooldown;
        [SerializeField, UsedImplicitly] private int emoteIntervalMin;
        [SerializeField, UsedImplicitly] private int emoteIntervalMax;
        [SerializeField, UsedImplicitly] private bool oneTime;
        [SerializeField, UsedImplicitly] private bool dontRepeat;

        private TimeTracker emoteTimeTracker = new();
        private TimeTracker cooldownTimeTracker = new();
        private bool doneSingle;
        private EmoteType previousEmote;
        private int continueHash;

        protected override void OnRegister()
        {
            base.OnRegister();

            continueHash = Animator.StringToHash(continueName);
        }

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

            cooldownTimeTracker.Update(deltaTime);
            if (oneTime && doneSingle || !string.IsNullOrEmpty(continueName) && StateAnimator.GetBool(continueHash))
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
                cooldownTimeTracker.Reset(emoteCooldown);
            }
        }
    }
}
