using System.Collections.Generic;
using Client.Spells;
using Common;
using JetBrains.Annotations;
using UnityEngine;

namespace Core
{
    [UsedImplicitly, CreateAssetMenu(fileName = "Spell Visuals Settings Container", menuName = "Game Data/Containers/Spell Visuals Info", order = 1)]
    public class SpellVisualsInfoContainer : ScriptableUniqueInfoContainer<SpellVisualsInfo>
    {
        [SerializeField, UsedImplicitly] private List<SpellVisualsInfo> visualsInfos;

        protected override List<SpellVisualsInfo> Items => visualsInfos;

        private readonly Dictionary<int, SpellVisualsInfo> spellVisualsInfosById = new Dictionary<int, SpellVisualsInfo>();

        public IReadOnlyDictionary<int, SpellVisualsInfo> SpellVisualsInfosById => spellVisualsInfosById;

        public override void Register()
        {
            base.Register();

            visualsInfos.ForEach(visual => spellVisualsInfosById.Add(visual.SpellInfo.Id, visual));
            visualsInfos.ForEach(visual => visual.Initialize());
        }

        public override void Unregister()
        {
            visualsInfos.ForEach(visual => visual.Deinitialize());
            spellVisualsInfosById.Clear();

            base.Unregister();
        }
    }
}