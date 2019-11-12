using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using JetBrains.Annotations;

namespace Client
{
    public class TooltipSlot : MonoBehaviour
    {
        [UsedImplicitly, SerializeField]
        private RectTransform bounds;
        [UsedImplicitly, SerializeField]
        private LayoutElement layoutElement;
        [UsedImplicitly, SerializeField]
        private RectTransform rectTransform;
        [UsedImplicitly, SerializeField]
        private VerticalLayoutGroup layoutGroup;
        [UsedImplicitly, SerializeField]
        private TooltipItem item;
        [UsedImplicitly, SerializeField]
        private float showingDelay;

        private IEnumerator tooltipRoutine;

        public TooltipItem Item => item;
        public LayoutElement LayoutElement => layoutElement;
        public VerticalLayoutGroup LayoutGroup => layoutGroup;
        public RectTransform SelfRect => rectTransform;
        public RectTransform TargetRect { get; private set; }

        public void Show(RectTransform newTargetRect)
        {
            if (bounds.gameObject.activeSelf && TargetRect == newTargetRect)
                return;

            TargetRect = newTargetRect;

            if (tooltipRoutine != null)
                StopCoroutine(tooltipRoutine);
            else
                gameObject.SetActive(true);

            tooltipRoutine = TooltipRoutine(showingDelay);
            StartCoroutine(tooltipRoutine);
        }

        public void Hide()
        {
            if (tooltipRoutine != null)
                StopCoroutine(tooltipRoutine);

            bounds.gameObject.SetActive(false);
            gameObject.SetActive(false);
            tooltipRoutine = null;
        }

        private IEnumerator TooltipRoutine(float delay)
        {
            bounds.gameObject.SetActive(false);
            yield return new WaitForSeconds(delay);
            yield return new WaitForEndOfFrame();
            bounds.gameObject.SetActive(true);
            tooltipRoutine = null;
        }
    }
}