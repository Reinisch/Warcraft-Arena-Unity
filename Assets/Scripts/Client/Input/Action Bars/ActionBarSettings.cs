using System.Collections.Generic;
using Common;
using Core;
using JetBrains.Annotations;
using UnityEngine;

namespace Client
{
    [UsedImplicitly, CreateAssetMenu(fileName = "Action Bar Settings", menuName = "Player Data/Input/Action Bar", order = 1)]
    public class ActionBarSettings : ScriptableUniqueInfo<ActionBarSettings>
    {
        [SerializeField, UsedImplicitly] private ActionBarSettingsContainer container;
        [SerializeField, UsedImplicitly] private bool saveChanges;
        [SerializeField, UsedImplicitly] private int actionBarSlot;
        [SerializeField, UsedImplicitly] private ClassType classType;
        [SerializeField, UsedImplicitly] private List<SpellInfo> defaultPresets = new List<SpellInfo>();

        private readonly ActionBarData activePreset = new ActionBarData();
        private string PlayerPrefsString => $"{nameof(ActionBarSettings)}#{Id}";

        public new int Id => base.Id;
        public int SlotId => actionBarSlot;
        public ClassType ClassType => classType;
        public IReadOnlyList<ActionButtonData> ActiveButtonPresets => activePreset.Buttons;

        protected override void OnRegister()
        {
            base.OnRegister();

            bool loadedFromPrefs = false;
            if (saveChanges)
            {
                string activePresetJson = PlayerPrefs.GetString(PlayerPrefsString, string.Empty);
                if (activePresetJson != string.Empty)
                {
                    JsonUtility.FromJsonOverwrite(PlayerPrefs.GetString(PlayerPrefsString, string.Empty), activePreset);
                    loadedFromPrefs = true;
                }
            }

            for (int i = 0; i < InputUtils.ActionBarSlotCount; i++)
                if (i >= activePreset.Buttons.Count)
                    activePreset.Buttons.Add(new ActionButtonData());
            
            if (!loadedFromPrefs)
                for (int i = 0; i < InputUtils.ActionBarSlotCount && i < defaultPresets.Count; i++)
                    if (defaultPresets[i] != null)
                        activePreset.Buttons[i].Modify(defaultPresets[i]);
        }

        protected override void OnUnregister()
        {
            if (saveChanges)
                PlayerPrefs.SetString(PlayerPrefsString, JsonUtility.ToJson(activePreset));

            base.OnUnregister();
        }

        protected override ActionBarSettings Data => this;
        protected override ScriptableUniqueInfoContainer<ActionBarSettings> Container => container;
    }
}
