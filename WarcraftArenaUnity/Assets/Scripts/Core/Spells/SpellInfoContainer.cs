using System.Collections.Generic;
using Common;
using JetBrains.Annotations;
using UnityEngine;

namespace Core
{
    [UsedImplicitly, CreateAssetMenu(fileName = "Spell Info Container", menuName = "Game Data/Containers/Spell Info", order = 1)]
    public class SpellInfoContainer : ScriptableUniqueInfoContainer<SpellInfo>
    {
        [SerializeField, UsedImplicitly] private List<SpellInfo> spellInfo;

        protected override List<SpellInfo> Items => spellInfo;
    }
}
