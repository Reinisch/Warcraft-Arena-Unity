using Common;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Audio;

namespace Client.Sound
{
    [CreateAssetMenu(fileName = "Sound Group Settings", menuName = "Game Data/Sound/Sound Group Settings", order = 1)]
    internal class SoundGroupSettings : ScriptableUniqueInfo<SoundGroupSettings>
    {
        [SerializeField, UsedImplicitly] private AudioMixerGroup mixerGroup;
        [SerializeField, UsedImplicitly] private float spatialBlend;
        [SerializeField, UsedImplicitly] private AudioRolloffMode rolloffMode = AudioRolloffMode.Logarithmic;
        [SerializeField, UsedImplicitly] private float maxDistance = 60.0f;
        [SerializeField, UsedImplicitly] private bool looping;

        public bool ShouldScheduleDestroy => !looping;

        public AudioSource Apply(AudioSource source)
        {
            source.loop = looping;
            source.spatialBlend = spatialBlend;
            source.rolloffMode = rolloffMode;
            source.maxDistance = maxDistance;
            source.outputAudioMixerGroup = mixerGroup;
            return source;
        }
    }
}
