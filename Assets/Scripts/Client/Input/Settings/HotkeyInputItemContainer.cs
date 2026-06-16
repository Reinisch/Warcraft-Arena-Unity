using Common;
using JetBrains.Annotations;
using UnityEngine;

namespace Client
{
    [UsedImplicitly, CreateAssetMenu(fileName = "Hotkey Input Item Container", menuName = "Game Data/Containers/Hotkey Input Item")]
    public class HotkeyInputItemContainer : ScriptableUniqueInfoContainer<HotkeyInputItem> { }
}
