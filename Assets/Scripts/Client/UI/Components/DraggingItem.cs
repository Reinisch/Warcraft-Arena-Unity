using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;

public class DraggingItem : MonoBehaviour
{
    [SerializeField, UsedImplicitly] private ButtonContent draggingContent;

    public Image Image { get; set; }
    public RectTransform RectTransform { get; set; } 

    [UsedImplicitly]
    private void Awake()
    {
        Image = GetComponent<Image>();
        RectTransform = GetComponent<RectTransform>();
        draggingContent.Image = Image;
        draggingContent.enabled = false;
    }

    public void DragContent(ButtonContent buttonContent)
    {
        gameObject.SetActive(true);
        draggingContent.FromDrag(buttonContent);
    }

    public void DropContent(ButtonContent targetButtonContent)
    {
        targetButtonContent.FromDrop(draggingContent);
        gameObject.SetActive(false);
    }

    public void DropReplaceContent(ButtonContent targetButtonContent)
    {
        targetButtonContent.Replace(draggingContent);
    }
}
