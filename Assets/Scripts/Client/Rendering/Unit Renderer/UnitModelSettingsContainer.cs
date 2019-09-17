using System.Collections.Generic;
using Common;
using JetBrains.Annotations;
using UnityEngine;

namespace Client
{
    [UsedImplicitly, CreateAssetMenu(fileName = "Unit Model Settings Container", menuName = "Game Data/Containers/Unit Model Settings", order = 1)]
    public class UnitModelSettingsContainer : ScriptableUniqueInfoContainer<UnitModelSettings>
    {
        [SerializeField, UsedImplicitly] private List<UnitModelSettings> modelSettings;

        protected override List<UnitModelSettings> Items => modelSettings;
    }
}
