using JetBrains.Annotations;
using UnityEngine;

namespace Core.Conditions
{
    [UsedImplicitly, CreateAssetMenu(fileName = "Never", menuName = "Game Data/Conditions/Constant/Never", order = 2)]
    public sealed class Never : Condition
    {
        protected override bool IsValid => false;
    }
}