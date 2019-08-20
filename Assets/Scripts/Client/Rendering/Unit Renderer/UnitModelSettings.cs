using Common;
using Core;
using JetBrains.Annotations;
using UnityEngine;

namespace Client
{
    [UsedImplicitly, CreateAssetMenu(fileName = "Unit Model Settings", menuName = "Player Data/Rendering/Unit Model", order = 3)]
    public sealed class UnitModelSettings : ScriptableUniqueInfo<SpellInfo>
    {
        [SerializeField, UsedImplicitly] private UnitModel prototype;

        public new int Id => base.Id;
        public UnitModel Prototype => prototype;
    }
}
