using System;
using System.Collections.Generic;
using Client.Localization;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Client
{
    [RequireComponent(typeof(CustomDropdown))]
    public class InputActionsDropdown : UIBehaviour
    {
        [Serializable]
        private class LocalizedInputActions
        {
            [UsedImplicitly] public LocalizedString InputString;
            [UsedImplicitly] public InputAction InputAction;
        }

        [SerializeField, UsedImplicitly] private CustomDropdown dropdown;
        [SerializeField, UsedImplicitly] private List<LocalizedInputActions> dropdownItems;

        [UsedImplicitly]
        protected override void Awake()
        {
            base.Awake();

            var localizedOptions = new List<string>();

            foreach (var item in dropdownItems)
                localizedOptions.Add(item.InputString.Value);

            dropdown.ClearOptions();
            dropdown.AddOptions(localizedOptions);

            dropdown.OnValueChanged.AddListener(OnDropdownChanged);
        }

        [UsedImplicitly]
        protected override void OnDestroy()
        {
            dropdown.OnValueChanged.RemoveListener(OnDropdownChanged);

            base.OnDestroy();
        }

        private void OnDropdownChanged(int index)
        {
            dropdownItems[index].InputAction.Execute();
        }
    }
}