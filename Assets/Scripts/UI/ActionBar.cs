using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.EventSystems;
using System;

public class ActionBar : MonoBehaviour
{
    public PlayerInterface playerInterface;
    public Sprite emptySlotImage;

    ButtonSlot[] buttonSlots;

    public void Initialize()
    {
        buttonSlots = transform.GetComponentsInChildren<ButtonSlot>();
        for (int i = 0; i < buttonSlots.Length; i++)
            buttonSlots[i].Initialize(this);
    }
    public void Update()
    {
        for (int i = 0; i < buttonSlots.Length; i++)
            buttonSlots[i].UpdateSlot();
    }

    public void CheckButtons(ArenaManager world)
    {
        for (int i = 0; i < buttonSlots.Length; i++)
            if (buttonSlots[i].IsButtonPressed())
            {
                buttonSlots[i].Click();
            }
    }
}
