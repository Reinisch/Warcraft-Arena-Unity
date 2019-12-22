using JetBrains.Annotations;
using UnityEngine;

namespace Client
{
    [CreateAssetMenu(fileName = "Sound Entry", menuName = "Game Data/Sound/Sound Entry", order = 1)]
    public class SoundEntry : ScriptableObject
    {
        [SerializeField, UsedImplicitly] private SoundReference soundReference;
        [SerializeField, UsedImplicitly] private SoundGroupSettings settings;
        [SerializeField, UsedImplicitly] private AudioClip audioClip;
        [SerializeField, UsedImplicitly, Range(0.0f, 1.0f)] private float volumeModifier = 1.0f;

        public void Play() => soundReference.Play(audioClip, settings, volumeModifier);

        public void PlayAtPoint(Vector3 point) => soundReference.PlayAtPoint(audioClip, settings, point, volumeModifier);

        public void PlayAtSource(AudioSource source)
        {
            settings.Apply(source);
            source.PlayOneShot(audioClip, volumeModifier);
        }
    }
}
