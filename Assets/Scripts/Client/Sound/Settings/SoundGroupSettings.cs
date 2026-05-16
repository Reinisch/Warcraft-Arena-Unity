using Common;
using JetBrains.Annotations;
using UnityEngine;

namespace Client
{
    [CreateAssetMenu(fileName = "Sound Group Settings", menuName = "Game Data/Sound/Sound Group Settings", order = 1)]
    public class SoundGroupSettings : ScriptableUniqueInfo<SoundGroupSettings>
    {
        [SerializeField, UsedImplicitly] private SoundGroupSettingsContainer container;
        [SerializeField, UsedImplicitly] private float volume;
        [SerializeField, UsedImplicitly] private float spatialBlend;
        [SerializeField, UsedImplicitly] private AudioRolloffMode rolloffMode = AudioRolloffMode.Logarithmic;
        [SerializeField, UsedImplicitly] private float maxDistance = 60.0f;

        protected override SoundGroupSettings Data => this;
        protected override ScriptableUniqueInfoContainer<SoundGroupSettings> Container => container;

        public AudioSource Apply(AudioSource source)
        {
            source.volume = volume;
            source.spatialBlend = spatialBlend;
            source.rolloffMode = rolloffMode;
            source.maxDistance = maxDistance;
            return source;
        }
    }
}
