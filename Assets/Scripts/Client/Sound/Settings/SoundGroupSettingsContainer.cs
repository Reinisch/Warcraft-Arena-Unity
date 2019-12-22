using System.Collections.Generic;
using Common;
using JetBrains.Annotations;
using UnityEngine;

namespace Client
{
    [UsedImplicitly, CreateAssetMenu(fileName = "Sound Group Settings Container", menuName = "Game Data/Containers/Sound Group Settings", order = 1)]
    public class SoundGroupSettingsContainer : ScriptableUniqueInfoContainer<SoundGroupSettings>
    {
        [SerializeField, UsedImplicitly] private SoundReference sound;
        [SerializeField, UsedImplicitly] private List<SoundGroupSettings> groups;

        private readonly Dictionary<SoundGroupSettings, AudioSource> sourcesBySettings = new Dictionary<SoundGroupSettings, AudioSource>();

        protected override List<SoundGroupSettings> Items => groups;

        public override void Register()
        {
            base.Register();

            foreach (var soundSetting in groups)
                sourcesBySettings[soundSetting] = ApplySettings(new GameObject(soundSetting.name).AddComponent<AudioSource>(), soundSetting);
        }

        public override void Unregister()
        {
            foreach (var soundSourceEntry in sourcesBySettings)
                Destroy(soundSourceEntry.Value);

            sourcesBySettings.Clear();

            base.Unregister();
        }

        public bool TryGetSource(SoundGroupSettings groupSettings, out AudioSource audioSource)
        {
            return sourcesBySettings.TryGetValue(groupSettings, out audioSource);
        }

        private AudioSource ApplySettings(AudioSource source, SoundGroupSettings settings)
        {
            source.transform.parent = sound.Container;
            return settings.Apply(source);
        }
    }
}