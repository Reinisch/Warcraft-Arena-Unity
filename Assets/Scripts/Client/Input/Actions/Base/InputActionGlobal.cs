using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using Common;
using Core.Conditions;
using Zenject;

namespace Client
{
    [UsedImplicitly, CreateAssetMenu(fileName = "Input Action Global", menuName = "Player Data/Input/Input Action Global")]
    public class InputActionGlobal : ScriptableUniqueInfo<InputActionGlobal>
    {
        [Inject] private InputReference input;
        [Inject] private EventBus eventBus;

        [SerializeField] private InputAction action;
        [SerializeField] private HotkeyInputItem hotkey;
        [SerializeField] private List<InputActionGlobal> blockedByActions;
        [SerializeField] private List<Condition> blockInactiveWhen;
        [SerializeField] private List<Condition> hotkeyInactiveWhen;

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

        protected override void OnRegister()
        {
            eventBus.RegisterEvent<HotkeyState>(hotkey, GameEvents.HotkeyStateChanged, OnHotkeyStateChanged);
        }

        protected override void OnUnregister()
        {
            eventBus.UnregisterEvent<HotkeyState>(hotkey, GameEvents.HotkeyStateChanged, OnHotkeyStateChanged);
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
