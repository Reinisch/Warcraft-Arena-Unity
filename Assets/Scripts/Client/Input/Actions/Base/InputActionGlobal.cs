using JetBrains.Annotations;
using UnityEngine;
using Common;

namespace Client
{
    [UsedImplicitly, CreateAssetMenu(fileName = "Input Action Global", menuName = "Player Data/Input/Input Action Global", order = 1)]
    public class InputActionGlobal : ScriptableObject
    {
        [SerializeField, UsedImplicitly] private InputAction action;
        [SerializeField, UsedImplicitly] private HotkeyInputItem hotkey;

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
            action.Execute();
        }
    }
}
