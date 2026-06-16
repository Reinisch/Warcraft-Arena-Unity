using Common;
using JetBrains.Annotations;
using UnityEngine;

namespace Client
{
    [UsedImplicitly, CreateAssetMenu(fileName = "Effect Settings Container", menuName = "Game Data/Containers/Effect Settings", order = 1)]
    public class EffectSettingsContainer : ScriptableUniqueInfoContainer<EffectSettings> { }
}
