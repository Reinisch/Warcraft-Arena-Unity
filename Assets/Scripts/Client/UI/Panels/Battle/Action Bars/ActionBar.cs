using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

namespace Client
{
    public class ActionBar : MonoBehaviour
    {
        [SerializeField, UsedImplicitly] List<ButtonSlot> buttonSlots;
        [SerializeField, UsedImplicitly] ActionBarSettings actionBarSettings;

        public void Initialize()
        {
            for (int i = 0; i < buttonSlots.Count; i++)
                buttonSlots[i].Initialize(actionBarSettings.ActiveButtonPresets[i]);
        }

        public void Denitialize()
        {
            buttonSlots.ForEach(buttonSlot => buttonSlot.Denitialize());
        }

        public void DoUpdate(float deltaTime)
        {
            foreach (var slot in buttonSlots)
                slot.DoUpdate();
        }
    }
}