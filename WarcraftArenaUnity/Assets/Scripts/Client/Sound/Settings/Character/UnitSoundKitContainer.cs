using System.Collections.Generic;
using Common;
using JetBrains.Annotations;
using UnityEngine;

namespace Client
{
    [UsedImplicitly, CreateAssetMenu(fileName = "Unit Sound Kit Container", menuName = "Game Data/Containers/Unit Sound Kit", order = 1)]
    public class UnitSoundKitContainer : ScriptableUniqueInfoContainer<UnitSoundKit>
    {
        [SerializeField, UsedImplicitly] private List<UnitSoundKit> soundKits;

        private readonly Dictionary<int, UnitSoundKit> soundKitsById = new Dictionary<int, UnitSoundKit>();

        protected override List<UnitSoundKit> Items => soundKits;

        public IReadOnlyDictionary<int, UnitSoundKit> SoundKitsById => soundKitsById;

        public override void Register()
        {
            base.Register();

            foreach(var soundKit in soundKits)
                soundKitsById.Add(soundKit.Id, soundKit);
        }

        public override void Unregister()
        {
            soundKitsById.Clear();

            base.Unregister();
        }
    }
}
