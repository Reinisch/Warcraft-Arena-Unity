using System.Collections.Generic;
using Common;
using Core;
using JetBrains.Annotations;
using UnityEngine;

namespace Client
{
    [UsedImplicitly, CreateAssetMenu(fileName = "Spell Sound Info Container", menuName = "Game Data/Containers/Spell Sound Info", order = 1)]
    public class SpellSoundInfoContainer : ScriptableUniqueInfoContainer<SpellSoundInfo>
    {
        [SerializeField, UsedImplicitly] private List<SpellSoundInfo> soundInfos;

        protected override List<SpellSoundInfo> Items => soundInfos;

        private readonly Dictionary<SpellInfo, SpellSoundInfo> spellSoundInfos = new Dictionary<SpellInfo, SpellSoundInfo>();

        public IReadOnlyDictionary<SpellInfo, SpellSoundInfo> SoundInfos => spellSoundInfos;

        public override void Register()
        {
            base.Register();

            foreach (var spellSetting in soundInfos)
                spellSoundInfos[spellSetting.SpellInfo] = spellSetting;
        }

        public override void Unregister()
        {
            spellSoundInfos.Clear();

            base.Unregister();
        }
    }
}