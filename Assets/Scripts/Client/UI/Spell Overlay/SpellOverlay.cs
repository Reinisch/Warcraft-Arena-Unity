using System;
using System.Collections.Generic;
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

        [Serializable]
        private class OverlayPerCharge
        {
            [SerializeField, UsedImplicitly] private int minCharges;
            [SerializeField, UsedImplicitly] private CanvasGroup partialCanvasGroup;

            public int MinCharges => minCharges;
            public CanvasGroup PartialCanvasGroup => partialCanvasGroup;
        }

        [SerializeField, UsedImplicitly] private RectTransform rectTransform;
        [SerializeField, UsedImplicitly] private Animator overlayAnimator;
        [SerializeField, UsedImplicitly] private CanvasGroup canvasGroup;
        [SerializeField, UsedImplicitly] private List<OverlayPerCharge> perChargeSettings;

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

        public void HandleAuraCharges(int chargeAmount)
        {
            foreach (OverlayPerCharge perChargeEntry in perChargeSettings)
                perChargeEntry.PartialCanvasGroup.alpha = perChargeEntry.MinCharges <= chargeAmount ? 1.0f : 0.0f;
        }
    }
}