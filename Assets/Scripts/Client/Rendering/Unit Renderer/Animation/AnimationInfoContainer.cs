using System.Collections.Generic;
using Common;
using JetBrains.Annotations;
using UnityEngine;

namespace Client
{
    [UsedImplicitly, CreateAssetMenu(fileName = "Animation Info Container", menuName = "Game Data/Containers/Animation Info", order = 1)]
    public class AnimationInfoContainer : ScriptableUniqueInfoContainer<AnimationInfo>
    {
    }
}