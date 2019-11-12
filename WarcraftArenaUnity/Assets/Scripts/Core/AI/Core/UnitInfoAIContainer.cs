using System.Collections.Generic;
using Common;
using JetBrains.Annotations;
using UnityEngine;

namespace Core
{
    [UsedImplicitly, CreateAssetMenu(fileName = "Unit Info AI Container", menuName = "Game Data/Containers/Unit Info AI", order = 1)]
    public class UnitInfoAIContainer : ScriptableUniqueInfoContainer<UnitInfoAI>
    {
        [SerializeField, UsedImplicitly] private List<UnitInfoAI> aiInfos;

        protected override List<UnitInfoAI> Items => aiInfos;
    }
}
