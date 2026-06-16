using Common;
using JetBrains.Annotations;
using UnityEngine;

namespace Client
{
    [UsedImplicitly, CreateAssetMenu(fileName = "Action Bar Settings Container", menuName = "Game Data/Containers/Action Bar Settings", order = 1)]
    public class ActionBarSettingsContainer : ScriptableUniqueInfoContainer<ActionBarSettings>
    {
        // No prebuilt (class, slot) lookup: the only consumer (ActionBar) scans ItemList on class change.
        // Lookup dictionaries on a ScriptableObject leak runtime state between editor/MPPM play sessions.

        public override void Unregister()
        {
            base.Unregister();

            // save prefs one time after updating all action bars
            PlayerPrefs.Save();
        }
    }
}
