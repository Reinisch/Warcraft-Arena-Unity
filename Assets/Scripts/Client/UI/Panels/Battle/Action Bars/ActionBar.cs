using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

public class ActionBar : MonoBehaviour
{
    [SerializeField, UsedImplicitly] List<ButtonSlot> buttonSlots;

    public void Initialize()
    {
        buttonSlots.ForEach(buttonSlot => buttonSlot.Initialize());
    }

    public void Denitialize()
    {
        buttonSlots.ForEach(buttonSlot => buttonSlot.Denitialize());
    }

    public void DoUpdate(float deltaTime)
    {
        foreach (var slot in buttonSlots)
            slot.DoUpdate(deltaTime);
    }
}
