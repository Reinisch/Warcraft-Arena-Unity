using System.Collections.Generic;
using Client;
using JetBrains.Annotations;
using UnityEngine;

public class ActionBar : UIContainer
{
    [SerializeField, UsedImplicitly] List<ButtonSlot> buttonSlots;

    public override void Initialize()
    {
        buttonSlots.ForEach(slot => behaviours.Add(slot));

        base.Initialize();
    }

    public override void Deinitialize()
    {
        base.Deinitialize();

        buttonSlots.ForEach(slot => behaviours.Remove(slot));
    }

    public override void DoUpdate(int deltaTime)
    {
        base.DoUpdate(deltaTime);

        foreach (var slot in buttonSlots)
            if (slot.IsButtonPressed())
                slot.Click();
    }
}
