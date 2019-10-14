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
        [SerializeField, UsedImplicitly] private List<InputActionGlobal> blockedByActions;
        [SerializeField, UsedImplicitly] private List<Condition> blockInactiveWhen;
        [SerializeField, UsedImplicitly] private List<Condition> hotkeyInactiveWhen;

        private bool IsBlockApplicable
        {
            get
            {
                foreach (Condition condition in blockInactiveWhen)
                    if (condition.IsApplicableAndValid(input.Player))
                        return false;

                return true;
            }
        }

        private bool IsHotkeyApplicable
        {
            get
            {
                foreach (Condition condition in hotkeyInactiveWhen)
                    if (condition.IsApplicableAndValid(input.Player))
                        return false;

                return true;
            }
        }

        public void Register()
        {
            EventHandler.RegisterEvent<HotkeyState>(hotkey, GameEvents.HotkeyStateChanged, OnHotkeyStateChanged);
        }

        public void Unregister()
        {
            EventHandler.UnregisterEvent<HotkeyState>(hotkey, GameEvents.HotkeyStateChanged, OnHotkeyStateChanged);
        }

        private void OnHotkeyStateChanged(HotkeyState state)
        {
            if (state == HotkeyState.Released || blockedByActions.Exists(blocker => blocker.IsBlockApplicable))
                return;

            if (IsHotkeyApplicable)
                action.Execute();
        }
    }
}
