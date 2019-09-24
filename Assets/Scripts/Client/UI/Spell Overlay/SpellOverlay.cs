using System;
using JetBrains.Annotations;
using UnityEngine;

namespace Client
{
    public class SpellOverlay : MonoBehaviour
    {
        public enum State
        {
            Active,
            Idle,
            Disabled
        }

        [SerializeField, UsedImplicitly] private RectTransform rectTransform;
        [SerializeField, UsedImplicitly] private Animator overlayAnimator;
        [SerializeField, UsedImplicitly] private CanvasGroup canvasGroup;

        private const string ActiveStateParamName = "IsPlaying";

        public RectTransform RectTransform => rectTransform;

        public void ModifyState(State state)
        {
            switch (state)
            {
                case State.Active:
                    overlayAnimator.enabled = true;
                    canvasGroup.alpha = 1;
                    overlayAnimator.SetBool(ActiveStateParamName, true);
                    break;
                case State.Idle:
                    overlayAnimator.enabled = true;
                    canvasGroup.alpha = 1;
                    overlayAnimator.SetBool(ActiveStateParamName, false);
                    break;
                case State.Disabled:
                    overlayAnimator.enabled = false;
                    canvasGroup.alpha = 0;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(state), state, "Unknown spell overlay state!");
            }
        }
    }
}