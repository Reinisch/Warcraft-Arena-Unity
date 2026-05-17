using JetBrains.Annotations;
using System.Collections.Generic;
using UnityEngine;

namespace Client
{
    [CreateAssetMenu(fileName = "Sound Entry", menuName = "Game Data/Sound/Sound Entry", order = 1)]
    public class SoundEntry : ScriptableObject
    {
        [SerializeField, UsedImplicitly] private SoundReference soundReference;
        [SerializeField, UsedImplicitly] private SoundGroupSettings settings;
        [SerializeField, UsedImplicitly] private AudioClip audioClip;
        [SerializeField, UsedImplicitly] private float delay;
        [SerializeField, UsedImplicitly, Range(0.0f, 1.0f)] private float volumeModifier = 1.0f;
        [SerializeField, UsedImplicitly] private List<SoundEntry> extraEntries = new();

        public void Play()
        {
            soundReference.Play(audioClip, settings, volumeModifier, delay);

            foreach (var entry in extraEntries)
                entry.Play();
        }

        public void PlayAtPoint(Vector3 point)
        {
            soundReference.PlayAtPoint(audioClip, settings, point, volumeModifier, delay);

            foreach (var entry in extraEntries)
                entry.PlayAtPoint(point);
        }

        public void PlayAtSource(AudioSource source)
        {
            settings.Apply(source);

            if (delay > 0)
            {
                source.volume = volumeModifier;
                source.clip = audioClip;
                source.PlayDelayed(delay);
            }
            else
                source.PlayOneShot(audioClip, volumeModifier);

            foreach (var entry in extraEntries)
                entry.PlayAtSource(source);
        }
    }
}
