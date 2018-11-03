using UnityEngine;

public class ButtonController : MonoBehaviour
{
    public GameObject toolTip;
    public DraggingItem draggingItem;

    [HideInInspector]
    public bool isDragging = false;
    [HideInInspector]
    public bool dragReplaced = false;

	void Update ()
    {
        if (isDragging)
        {
            Vector3 pos = (Input.mousePosition - new Vector3((float)Screen.width / 2, (float)Screen.height / 2, 0));
            draggingItem.GetComponent<RectTransform>().localPosition = pos;

            if (Input.GetMouseButtonUp(0))
            {
                if (dragReplaced)
                {
                    dragReplaced = false;
                }
                else
                {
                    isDragging = false;
                    draggingItem.gameObject.SetActive(false);
                }
            }
        }
	}

    public void ShowToolTip(RectTransform buttonRect)
    {
        toolTip.SetActive(true);
        if (buttonRect.position.y > (float)Screen.height / 2)
        {
            if(buttonRect.position.x > (float)Screen.width / 2)
            {
                toolTip.GetComponent<RectTransform>().pivot = new Vector2(1,1);
            }
            else
            {
                toolTip.GetComponent<RectTransform>().pivot = new Vector2(0,1);
            }
        }
        else
        {
            if (buttonRect.position.x > (float)Screen.width / 2)
            {
                toolTip.GetComponent<RectTransform>().pivot = new Vector2(1, 0);
            }
            else
            {
                toolTip.GetComponent<RectTransform>().pivot = new Vector2(0, 0);
            }
        }
        toolTip.GetComponent<RectTransform>().position = buttonRect.position;
    }

    public void HideToolTip()
    {
        toolTip.SetActive(false);
    }

    public void DragItem(ButtonContent buttonContent)
    {
        draggingItem.DragContent(buttonContent);
        draggingItem.RectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal,
            buttonContent.ButtonSlot.RectTransform.rect.width);
        isDragging = true;
    }

    public void DropItem(ButtonContent buttonContent)
    {
        if (buttonContent.ContentType != ButtonSlot.ContentType.Empty)
        {
            draggingItem.DropReplaceContent(buttonContent);
            dragReplaced = true;
        }
        else
        {
            draggingItem.DropContent(buttonContent);
            isDragging = false;
        }
    }
}
