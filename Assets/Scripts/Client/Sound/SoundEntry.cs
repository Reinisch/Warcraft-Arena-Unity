using JetBrains.Annotations;
using UnityEngine;

namespace Client
{
    [CreateAssetMenu(fileName = "Sound Entry", menuName = "Game Data/Sound/Sound Entry", order = 1)]
    public class SoundEntry : ScriptableObject
    {
        [SerializeField, UsedImplicitly] private SoundReference soundReference;
        [SerializeField, UsedImplicitly] private SoundSettings settings;
        [SerializeField, UsedImplicitly] private AudioClip audioClip;

        public void Play() => soundReference.Play(audioClip, settings);

        public void PlayAtPoint(Vector3 point) => soundReference.PlayAtPoint(audioClip, settings, point);
    }
}
