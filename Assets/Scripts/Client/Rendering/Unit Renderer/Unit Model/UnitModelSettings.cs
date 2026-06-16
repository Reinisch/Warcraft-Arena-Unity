using Common;
using JetBrains.Annotations;
using UnityEngine;

namespace Client
{
    [UsedImplicitly, CreateAssetMenu(fileName = "Unit Model Settings", menuName = "Player Data/Rendering/Unit Model", order = 3)]
    public sealed class UnitModelSettings : ScriptableUniqueInfo<UnitModelSettings>
    {
        [SerializeField, UsedImplicitly] private UnitModel prototype;
        [SerializeField, UsedImplicitly] private UnitSoundKit soundKit;

        public new int Id => base.Id;
        public UnitSoundKit SoundKit => soundKit;
        public UnitModel Prototype => prototype;
    }
}
