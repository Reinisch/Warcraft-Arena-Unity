using System.Collections.Generic;
using Common;
using JetBrains.Annotations;
using UnityEngine;

namespace Client
{
    [UsedImplicitly, CreateAssetMenu(fileName = "Action Bar Settings Container", menuName = "Game Data/Containers/Action Bar Settings", order = 1)]
    public class ActionBarSettingsContainer : ScriptableUniqueInfoContainer<ActionBarSettings>
    {
        [SerializeField, UsedImplicitly] private List<ActionBarSettings> actionBars;

        protected override List<ActionBarSettings> Items => actionBars;

        public override void Unregister()
        {
            base.Unregister();

            PlayerPrefs.Save();
        }
    }
}
