using System.Collections.Generic;
using Client.UI;
using Common;
using Core;
using UnityEngine;
using Zenject;

namespace Client
{
    public class ArenaResultPresenter : Presenter<ArenaResultPanel>
    {
        [Inject] private ClientArenaController arena;
        [Inject] private EventBus eventBus;

        private ArenaPhase lastPhase;
        private int lastAnnouncedSecond;

        public override void Initialize(ArenaResultPanel view)
        {
            base.Initialize(view);

            lastPhase = arena.Phase;
            lastAnnouncedSecond = -1;
            arena.StateChanged += OnStateChanged;
            arena.MatchEnded += OnMatchEnded;
        }

        public override void Deinitialize()
        {
            arena.StateChanged -= OnStateChanged;
            arena.MatchEnded -= OnMatchEnded;

            base.Deinitialize();
        }

        private void OnStateChanged()
        {
            if (arena.Phase == ArenaPhase.Warmup)
                AnnounceCountdown();

            if (arena.Phase == lastPhase)
                return;

            if (arena.Phase == ArenaPhase.InProgress)
            {
                Announce(View.BattleStartText);
                View.PlayBattleStart();
            }

            lastPhase = arena.Phase;
        }

        private void AnnounceCountdown()
        {
            int second = Mathf.CeilToInt(Mathf.Max(0f, arena.Countdown));
            if (second == lastAnnouncedSecond || second < 1)
                return;

            if (second > View.CountdownAnnounceThreshold && !IsMilestone(second))
                return;

            lastAnnouncedSecond = second;
            Announce(string.Format(View.CountdownFormat, second));
            View.PlayCountdownTick();
        }

        private bool IsMilestone(int second)
        {
            IReadOnlyList<int> milestones = View.CountdownMilestones;
            if (milestones != null)
                for (int i = 0; i < milestones.Count; i++)
                    if (milestones[i] == second)
                        return true;

            return false;
        }

        private void OnMatchEnded(ArenaMatchResult result)
        {
            string title = arena.LocalOutcome switch
            {
                ArenaLocalOutcome.Won => View.VictoryText,
                ArenaLocalOutcome.Lost => View.DefeatText,
                _ => View.DrawText
            };

            switch (result)
            {
                case ArenaMatchResult.TeamA:
                    View.PlayAllianceWin();
                    break;
                case ArenaMatchResult.TeamB:
                    View.PlayHordeWin();
                    break;
            }

            Announce(title);
            View.ShowResult(title, arena.Participants);
        }

        private void Announce(string text)
        {
            if (!string.IsNullOrEmpty(text))
                eventBus.ExecuteEvent(GameEvents.SystemMessage, text);
        }
    }
}
