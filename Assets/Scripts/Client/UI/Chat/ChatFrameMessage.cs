using Client.Localization;
using Core;
using JetBrains.Annotations;
using UnityEngine;
using TMPro;

public class ChatFrameMessage : MonoBehaviour
{
    [SerializeField, UsedImplicitly] private TMP_Text messageLabel;
    [SerializeField, UsedImplicitly] private RectTransform rectTransform;
    [SerializeField, UsedImplicitly] private LocalizedString chatGeneralString;

    public RectTransform RectTransform => rectTransform;

    public void Modify(Unit unit, string message)
    {
        messageLabel.text = $"[{chatGeneralString.Value}] [{unit.Name}]: {message}";
    }

    public void MoveToBottom()
    {
        rectTransform.SetAsLastSibling();
    }
}