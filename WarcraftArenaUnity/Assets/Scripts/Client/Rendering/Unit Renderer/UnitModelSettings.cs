using Common;
using JetBrains.Annotations;
using UnityEngine;

namespace Client
{
    [UsedImplicitly, CreateAssetMenu(fileName = "Unit Model Settings", menuName = "Player Data/Rendering/Unit Model", order = 3)]
    public sealed class UnitModelSettings : ScriptableUniqueInfo<UnitModelSettings>
    {
        [SerializeField, UsedImplicitly] private UnitModelSettingsContainer container;
        [SerializeField, UsedImplicitly] private UnitModel prototype;
        [SerializeField, UsedImplicitly] private int defaultSoundKit;

        protected override ScriptableUniqueInfoContainer<UnitModelSettings> Container => container;
        protected override UnitModelSettings Data => this;

        public new int Id => base.Id;
        public int DefaultSoundKit => defaultSoundKit;
        public UnitModel Prototype => prototype;
    }
}
