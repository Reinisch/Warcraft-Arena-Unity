using System.Collections.Generic;
using Common;
using JetBrains.Annotations;
using UnityEngine;

namespace Client
{
    [UsedImplicitly, CreateAssetMenu(fileName = "Spell Overlay Settings Container", menuName = "Game Data/Containers/Spell Overlay Settings", order = 2)]
    public class SpellOverlaySettingsContainer : ScriptableUniqueInfoContainer<SpellOverlaySettings>
    {
    }
}
