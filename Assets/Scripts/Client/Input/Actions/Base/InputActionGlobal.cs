using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using Common;
using Core.Conditions;

namespace Client
{
    [UsedImplicitly, CreateAssetMenu(fileName = "Input Action Global", menuName = "Player Data/Input/Input Action Global", order = 1)]
    public class InputActionGlobal : ScriptableObject
    {
        [SerializeField, UsedImplicitly] private InputReference input;
        [SerializeField, UsedImplicitly] private InputAction action;
        [SerializeField, UsedImplicitly] private HotkeyInputItem hotkey;
        [SerializeField, UsedImplicitly] private List<InputActionGlobal> blockedBy;
        [SerializeField, UsedImplicitly] private List<Condition> inactiveWhen;

        private bool IsApplicable
        {
            get
            {
                foreach (Condition contition in inactiveWhen)
                    if (contition.WithSource(input.Player).IsValid)
                        return false;

                return true;
            }
        }

        public void Register()
        {
            EventHandler.RegisterEvent(hotkey, GameEvents.HotkeyPressed, OnHotkeyPressed);
        }

        public void Unregister()
        {
            EventHandler.UnregisterEvent(hotkey, GameEvents.HotkeyPressed, OnHotkeyPressed);
        }

        private void OnHotkeyPressed()
        {
            if (blockedBy.Exists(blocker => blocker.IsApplicable))
                return;

            action.Execute();
        }
    }
}
