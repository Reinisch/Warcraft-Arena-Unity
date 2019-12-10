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

        private readonly Dictionary<int, UnitModelSettings> modelSettingsById = new Dictionary<int, UnitModelSettings>();

        protected override List<UnitModelSettings> Items => modelSettings;

        public IReadOnlyDictionary<int, UnitModelSettings> ModelSettingsById => modelSettingsById;

        public override void Register()
        {
            base.Register();

            for (int i = 0; i < ItemList.Count; i++)
                modelSettingsById.Add(ItemList[i].Id, ItemList[i]);
        }

        public override void Unregister()
        {
            modelSettingsById.Clear();

            base.Unregister();
        }
    }
}
