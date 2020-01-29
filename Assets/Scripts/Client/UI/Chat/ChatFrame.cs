using System.Collections.Generic;
using Client;
using Common;
using Core;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using UnityEngine.UI;

public class ChatFrame : MonoBehaviour
{
    [SerializeField, UsedImplicitly] private InputReference input;
    [SerializeField, UsedImplicitly] private ScrollRect scrollRect;
    [SerializeField, UsedImplicitly] private ChatFrameMessage messagePrototype;
    [SerializeField, UsedImplicitly] private TMP_InputField inputField;
    [SerializeField, UsedImplicitly] private Transform messageContainer;
    [SerializeField, UsedImplicitly] private HotkeyInputItem chatFocusHotkey;
    [SerializeField, UsedImplicitly] private int maxMessageCount = 100;

    private readonly List<ChatFrameMessage> chatMessages = new List<ChatFrameMessage>();
    private const float BottomSnapThreshold = 0.001f;

    [UsedImplicitly]
    public bool SnapToBottom { get; set; } = true;

    [UsedImplicitly]
    private void Awake()
    {
        EventHandler.RegisterEvent<Unit, string>(GameEvents.UnitChat, OnUnitChat);
        EventHandler.RegisterEvent<HotkeyState>(chatFocusHotkey, GameEvents.HotkeyStateChanged, OnHotkeyStateChanged);
        inputField.onSubmit.AddListener(OnSubmit);
        inputField.onDeselect.AddListener(OnDeselect);

        GameObjectPool.PreInstantiate(messagePrototype, maxMessageCount);
    }

    [UsedImplicitly]
    private void OnDestroy()
    {
        foreach (ChatFrameMessage message in chatMessages)
            GameObjectPool.Return(message, true);

        chatMessages.Clear();
        inputField.onSubmit.RemoveListener(OnSubmit);
        inputField.onDeselect.RemoveListener(OnDeselect);
        EventHandler.UnregisterEvent<Unit, string>(GameEvents.UnitChat, OnUnitChat);
        EventHandler.UnregisterEvent<HotkeyState>(chatFocusHotkey, GameEvents.HotkeyStateChanged, OnHotkeyStateChanged);
    }

    [UsedImplicitly]
    private void Update()
    {
        if (SnapToBottom && scrollRect.verticalNormalizedPosition > BottomSnapThreshold)
            scrollRect.normalizedPosition = Vector2.zero;
    }

    private void OnDeselect(string text)
    {
        inputField.text = string.Empty;
    }

    private void OnSubmit(string text)
    {
        inputField.text = string.Empty;

        if (!inputField.wasCanceled && !string.IsNullOrEmpty(text))
            input.Say(text);

        if (EventSystem.current.currentSelectedGameObject == inputField.gameObject)
            EventSystem.current.SetSelectedGameObject(null);
    }

    private void OnUnitChat(Unit unit, string text)
    {
        ChatFrameMessage chatFrameMessage;
        if (chatMessages.Count >= maxMessageCount)
        {
            chatFrameMessage = chatMessages[0];
            chatMessages.RemoveAt(0);
        }
        else
        {
            chatFrameMessage = GameObjectPool.Take(messagePrototype);
            chatFrameMessage.RectTransform.SetParent(messageContainer, false);
        }

        chatMessages.Add(chatFrameMessage);
        chatFrameMessage.Modify(unit, text);
        chatFrameMessage.MoveToBottom();

        if (scrollRect.verticalNormalizedPosition < BottomSnapThreshold)
            SnapToBottom = true;
    }

    private void OnHotkeyStateChanged(HotkeyState state)
    {
        if (enabled && state == HotkeyState.Pressed)
            if (EventSystem.current.currentSelectedGameObject != inputField.gameObject)
                EventSystem.current.SetSelectedGameObject(inputField.gameObject);
    }
}