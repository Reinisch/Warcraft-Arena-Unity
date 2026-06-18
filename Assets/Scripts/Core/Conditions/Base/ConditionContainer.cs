using Common;
using JetBrains.Annotations;
using UnityEngine;

namespace Core.Conditions
{
    [UsedImplicitly, CreateAssetMenu(fileName = "Condition Container", menuName = "Game Data/Containers/Condition", order = 1)]
    public class ConditionContainer : ScriptableUniqueInfoContainer<Condition>
    {
    }
}
