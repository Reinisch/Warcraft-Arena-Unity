using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.EventSystems;

[UsedImplicitly]
public class AnimatorPointerNotifier : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField, UsedImplicitly] private Animator animator;
    [SerializeField, UsedImplicitly] private string parameter;

    [UsedImplicitly]
    private void OnDisable() => animator.SetBool(parameter, false);

    public void OnPointerEnter(PointerEventData eventData) => animator.SetBool(parameter, true);

    public void OnPointerExit(PointerEventData eventData) => animator.SetBool(parameter, false);
}