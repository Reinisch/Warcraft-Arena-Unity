using Common;
using JetBrains.Annotations;
using UnityEngine;

namespace Client
{
    [UsedImplicitly, CreateAssetMenu(fileName = "Game Options Container", menuName = "Game Data/Containers/Game Option", order = 1)]
    public class GameOptionItemContainer : ScriptableUniqueInfoContainer<GameOptionItem> { }
}
