using JetBrains.Annotations;
using UnityEngine;

namespace Client
{
    [CreateAssetMenu(fileName = "Sound Settings", menuName = "Game Data/Sound/Sound Settings", order = 1)]
    public class SoundSettings : ScriptableObject
    {
        [SerializeField, UsedImplicitly] private float volume;
        [SerializeField, UsedImplicitly] private float spatialBlend;

        public AudioSource Apply(AudioSource source)
        {
            source.volume = volume;
            source.spatialBlend = spatialBlend;
            return source;
        }
    }
}
