using System.Collections.Generic;
using Client.Sound;
using Client.UI;
using Core;
using JetBrains.Annotations;
using TMPro;
using UnityEngine;
using Zenject;

namespace Client
{
    public class ArenaResultPanel : UIPanel<BattleScreen>
    {
        [Inject] private ArenaResultPresenter presenter;
        [Inject] private PlayerManager playerManager;

        [Header("Scoreboard")]
        [SerializeField, UsedImplicitly] private TMP_Text titleLabel;
        [SerializeField, UsedImplicitly] private ArenaResultRow rowPrototype;
        [SerializeField, UsedImplicitly] private Transform rowContainer;
        [SerializeField, UsedImplicitly] private Color teamAColor = new Color(0.4f, 0.6f, 1f);
        [SerializeField, UsedImplicitly] private Color teamBColor = new Color(1f, 0.45f, 0.4f);

        [Header("Announcement text")]
        [SerializeField, UsedImplicitly] private int countdownAnnounceThreshold = 5;
        [SerializeField, UsedImplicitly] private int[] countdownMilestones = { 30, 20, 15, 10 };
        [SerializeField, UsedImplicitly] private string countdownFormat = "Arena begins in {0}...";
        [SerializeField, UsedImplicitly] private string battleStartText = "The arena battle has begun!";
        [SerializeField, UsedImplicitly] private string victoryText = "Victory!";
        [SerializeField, UsedImplicitly] private string defeatText = "Defeat!";
        [SerializeField, UsedImplicitly] private string drawText = "Draw!";

        [Header("Sounds")]
        [SerializeField, UsedImplicitly] private SoundEntry countdownTickSound;
        [SerializeField, UsedImplicitly] private SoundEntry battleStartSound;
        [SerializeField, UsedImplicitly] private SoundEntry allianceWinSound;
        [SerializeField, UsedImplicitly] private SoundEntry hordeWinSound;

        private readonly List<ArenaResultRow> rows = new();

        public int CountdownAnnounceThreshold => countdownAnnounceThreshold;
        public IReadOnlyList<int> CountdownMilestones => countdownMilestones;
        public string CountdownFormat => countdownFormat;
        public string BattleStartText => battleStartText;
        public string VictoryText => victoryText;
        public string DefeatText => defeatText;
        public string DrawText => drawText;

        protected override void PanelInitialized()
        {
            base.PanelInitialized();

            if (rowPrototype != null)
                rowPrototype.gameObject.SetActive(false);

            presenter.Initialize(this);
        }

        protected override void PanelDeinitialized()
        {
            presenter.Deinitialize();

            base.PanelDeinitialized();
        }

        public void ShowResult(string title, IReadOnlyList<ArenaParticipantInfo> participants)
        {
            if (titleLabel != null)
                titleLabel.text = title;

            foreach (ArenaResultRow row in rows)
                if (row != null)
                    Destroy(row.gameObject);
            rows.Clear();

            if (rowPrototype != null && rowContainer != null && participants != null)
                foreach (ArenaParticipantInfo participant in participants)
                {
                    ArenaResultRow row = Instantiate(rowPrototype, rowContainer);
                    row.gameObject.SetActive(true);
                    row.Set(participant, participant.TeamA ? teamAColor : teamBColor);
                    rows.Add(row);
                }

            Show();
        }

        public void PlayCountdownTick() => Play(countdownTickSound);
        public void PlayBattleStart() => Play(battleStartSound);
        public void PlayAllianceWin() => Play(allianceWinSound);
        public void PlayHordeWin() => Play(hordeWinSound);

        private void Play(SoundEntry entry)
        {
            if (entry == null)
                return;

            Vector3 at = playerManager != null && playerManager.Player != null
                ? playerManager.Player.Position
                : transform.position;

            entry.Play(at);
        }
    }
}
