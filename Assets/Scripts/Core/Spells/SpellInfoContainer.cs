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

        [Header("Unique Spells")]
        [SerializeField, UsedImplicitly] private SpellInfo controlVehicle;

        protected override List<SpellInfo> Items => spellInfo;

        public SpellInfo ControlVehicle => controlVehicle;
    }
}
