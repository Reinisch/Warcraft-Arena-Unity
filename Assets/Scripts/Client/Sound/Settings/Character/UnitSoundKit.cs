using Common;
using JetBrains.Annotations;
using UnityEngine;

namespace Client
{
    [UsedImplicitly, CreateAssetMenu(fileName = "Unit Sound Kit", menuName = "Game Data/Sound/Unit Sound Kit", order = 1)]
    public sealed class UnitSoundKit : SoundKit<UnitSoundKit, UnitSounds>
    {
        [SerializeField, UsedImplicitly] private UnitSoundKitContainer continer;
        [SerializeField, UsedImplicitly] private SoundEntryCharacterSoundDictionary soundItems;

        protected override ScriptableUniqueInfoContainer<UnitSoundKit> Container => continer;
        protected override UnitSoundKit Data => this;

        public new int Id => base.Id;

        public override void Populate() => soundItems.Populate();

        public override void Clear() => soundItems.Clear();

        public override SoundEntry FindSound(UnitSounds soundType) => soundItems.Value(soundType);
    }
}
