using System.Collections.Generic;
using Client;
using JetBrains.Annotations;
using UnityEngine;

public class ActionBar : UIContainer
{
    [SerializeField, UsedImplicitly] List<ButtonSlot> buttonSlots;

    public override void Initialize()
    {
        buttonSlots.ForEach(slot => Behaviours.Add(slot));

        base.Initialize();
    }

    public override void Deinitialize()
    {
        buttonSlots.ForEach(slot => Behaviours.Remove(slot));

        base.Deinitialize();
    }

    public override void DoUpdate(int deltaTime)
    {
        base.DoUpdate(deltaTime);

        foreach (var slot in buttonSlots)
            if (slot.IsButtonPressed())
                slot.Click();
    }
}
