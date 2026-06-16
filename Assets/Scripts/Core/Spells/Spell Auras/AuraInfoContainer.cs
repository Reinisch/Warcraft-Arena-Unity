using Common;
using JetBrains.Annotations;
using UnityEngine;

namespace Core
{
    [UsedImplicitly, CreateAssetMenu(fileName = "Aura Info Container", menuName = "Game Data/Containers/Aura Info")]
    internal class AuraInfoContainer : ScriptableUniqueInfoContainer<AuraInfo>
    {
    }
}
