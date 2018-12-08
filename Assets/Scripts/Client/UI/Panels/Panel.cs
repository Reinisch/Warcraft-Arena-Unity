using JetBrains.Annotations;
using UnityEngine;

namespace Client
{
    public abstract class Panel : MonoBehaviour
    {
        [SerializeField, UsedImplicitly] private CanvasGroup panelCanvasGroup;

        protected void SetInputState(bool interactable)
        {
            panelCanvasGroup.interactable = interactable;
        }
    }
}
