using System.Collections.Generic;
using Common;
using JetBrains.Annotations;
using UnityEngine;

namespace Client
{
    [UsedImplicitly, CreateAssetMenu(fileName = "Action Bar Settings", menuName = "Player Data/Input/Action Bar", order = 1)]
    public class ActionBarSettings : ScriptableUniqueInfo<ActionBarSettings>
    {
        [SerializeField, UsedImplicitly] private ActionBarSettingsContainer container;
        [SerializeField, UsedImplicitly] private int actionBarSlot;
        [SerializeField, UsedImplicitly] private List<ActionButtonData> buttons = new List<ActionButtonData>();

        private readonly ActionBarData activePreset = new ActionBarData();
        private string PlayerPrefsString => $"{nameof(ActionBarSettings)}#{Id}";

        public IReadOnlyList<ActionButtonData> ActiveButtonPresets => activePreset.Buttons;

        protected override void OnRegister()
        {
            base.OnRegister();

            JsonUtility.FromJsonOverwrite(PlayerPrefs.GetString(PlayerPrefsString, string.Empty), activePreset);

            for (int i = 0; i < InputUtils.ActionBarSlotCount; i++)
                if (i >= activePreset.Buttons.Count)
                    activePreset.Buttons.Add(i < buttons.Count ? buttons[i] : new ActionButtonData());
        }

        protected override void OnUnregister()
        {
            PlayerPrefs.SetString(PlayerPrefsString, JsonUtility.ToJson(activePreset));

            base.OnUnregister();
        }

        protected override ActionBarSettings Data => this;
        protected override ScriptableUniqueInfoContainer<ActionBarSettings> Container => container;
    }
}
