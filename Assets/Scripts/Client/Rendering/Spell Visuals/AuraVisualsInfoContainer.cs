using System.Collections.Generic;
using Client.Spells;
using Common;
using JetBrains.Annotations;
using UnityEngine;

namespace Client
{
    [UsedImplicitly, CreateAssetMenu(fileName = "Aura Visual Info Container", menuName = "Game Data/Containers/Aura Visual Info", order = 1)]
    public class AuraVisualsInfoContainer : ScriptableUniqueInfoContainer<AuraVisualsInfo>
    {
        [SerializeField, UsedImplicitly] private List<AuraVisualsInfo> visualsInfos;

        protected override List<AuraVisualsInfo> Items => visualsInfos;

        private readonly Dictionary<int, AuraVisualsInfo> auraVisualsInfosById = new Dictionary<int, AuraVisualsInfo>();

        public IReadOnlyDictionary<int, AuraVisualsInfo> AuraVisualsInfosById => auraVisualsInfosById;

        public override void Register()
        {
            base.Register();

            visualsInfos.ForEach(visual => auraVisualsInfosById.Add(visual.AuraInfo.Id, visual));
        }

        public override void Unregister()
        {
            auraVisualsInfosById.Clear();

            base.Unregister();
        }
    }
}