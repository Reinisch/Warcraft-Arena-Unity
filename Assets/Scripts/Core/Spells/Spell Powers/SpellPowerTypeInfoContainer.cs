using System.Collections.Generic;
using Common;
using JetBrains.Annotations;
using UnityEngine;

namespace Core
{
    [UsedImplicitly, CreateAssetMenu(fileName = "Spell Power Type Info Container", menuName = "Game Data/Containers/Spell Power Type Info", order = 1)]
    public class SpellPowerTypeInfoContainer : ScriptableUniqueInfoContainer<SpellPowerTypeInfo>
    {
        [SerializeField, UsedImplicitly] private List<SpellPowerTypeInfo> spellPowerTypeInfos;

        protected override List<SpellPowerTypeInfo> Items => spellPowerTypeInfos;
    }
}
