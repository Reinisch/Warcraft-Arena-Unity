using System.Collections.Generic;
using Core;
using JetBrains.Annotations;
using UnityEngine;

namespace Client
{
    public class ActionBar : MonoBehaviour
    {
        [SerializeField, UsedImplicitly] private ActionBarSettingsContainer container;
        [SerializeField, UsedImplicitly] private List<ButtonSlot> buttonSlots;
        [SerializeField, UsedImplicitly] private ActionBarSettings actionBarSettings;
        [SerializeField, UsedImplicitly] private MovementMode movementMode;

        public MovementMode MovementMode => movementMode;

        public bool IsActive => gameObject.activeSelf;

        public void SetActive(bool active)
        {
            gameObject.SetActive(active);
        }

        public void Initialize()
        {
            for (int i = 0; i < buttonSlots.Count; i++)
                buttonSlots[i].Initialize();
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

        public void ModifyContent(ClassType classType)
        {
            ActionBarSettings appliedSettings = actionBarSettings;
            foreach (ActionBarSettings settings in container.ItemList)
                if (settings.ClassType == classType && settings.SlotId == actionBarSettings.SlotId)
                {
                    appliedSettings = settings;
                    break;
                }

            for (int i = 0; i < buttonSlots.Count; i++)
                buttonSlots[i].ButtonContent.UpdateContent(appliedSettings.ActiveButtonPresets[i]);
        }
    }
}