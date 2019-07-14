using Client.Spells;
using JetBrains.Annotations;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Client
{
    public class BuffSlot : MonoBehaviour
    {
        [SerializeField, UsedImplicitly] private RenderingReference rendering;
        [SerializeField, UsedImplicitly] private Image contentImage;
        [SerializeField, UsedImplicitly] private Image cooldownImage;
        [SerializeField, UsedImplicitly] private TextMeshProUGUI cooldownText;
        [SerializeField, UsedImplicitly] private CanvasGroup canvasGroup;
        [SerializeField, UsedImplicitly] private bool displayTimer;

        private readonly char[] timerText = { ' ', ' ', ' ' };
        private readonly char[] emptyTimerText = { ' ', ' ', ' ' };

        private int auraInfoId;
        private int serverRefreshFrame;
        private int refreshDuration;
        private int maxDuration;

        private int durationLeft;

        public void UpdateState(VisibleAuraState auraState)
        {
            if (auraState == null || auraState.AuraId == 0)
            {
                auraInfoId = 0;
                canvasGroup.alpha = 0.0f;
                return;
            }

            if (!displayTimer)
                cooldownText.SetCharArray(emptyTimerText, 0, 0);

            canvasGroup.alpha = 1.0f;
            auraInfoId = auraState.AuraId;
            serverRefreshFrame = auraState.RefreshFrame;
            refreshDuration = auraState.Duration;
            maxDuration = auraState.MaxDuration;

            int expectedCooldownFrames = (int)(refreshDuration / BoltNetwork.FrameDeltaTime / 1000.0f);
            int framesPassed = BoltNetwork.ServerFrame - serverRefreshFrame;
            if (framesPassed > expectedCooldownFrames || expectedCooldownFrames < 1)
                durationLeft = 0;
            else
            {
                var cooldownProgressLeft = 1.0f - (float)framesPassed / expectedCooldownFrames;
                durationLeft = Mathf.RoundToInt(refreshDuration * cooldownProgressLeft);
            }

            contentImage.sprite = rendering.AuraVisualSettingsById.TryGetValue(auraInfoId, out AuraVisualSettings settings)
                ? settings.AuraIcon
                : rendering.DefaultSpellIcon;
        }

        public void DoUpdate(float deltaTime)
        {
            if(auraInfoId == 0)
                return;

            if (maxDuration == 0)
            {
                cooldownText.SetCharArray(timerText, 0, 0);
                cooldownImage.fillAmount = 0.0f;
            }
            else
            {
                if (durationLeft > 0)
                {
                    int deltaInMilliseconds = (int) (deltaTime * 1000.0f);
                    if (durationLeft > deltaInMilliseconds)
                        durationLeft -= deltaInMilliseconds;
                    else
                        durationLeft = 0;
                }

                if (displayTimer)
                {
                    if (durationLeft < 1000)
                        cooldownText.SetCharArray(timerText.SetSpellTimerNonAlloc(durationLeft, out int length), 0, length);
                    else
                        cooldownText.SetCharArray(emptyTimerText, 0, 0);
                }

                cooldownImage.fillAmount = (float)durationLeft / maxDuration;
            }
        }
    }
}
