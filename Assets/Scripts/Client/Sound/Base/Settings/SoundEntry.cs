using Common;
using JetBrains.Annotations;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;
using Zenject;

namespace Client.Sound
{
    [CreateAssetMenu(fileName = "Sound Entry", menuName = "Game Data/Sound/Sound Entry", order = 1)]
    public class SoundEntry : ScriptableUniqueInfo<SoundEntry>
    {
        [Inject] private SoundPlayController controller;

        [SerializeField, UsedImplicitly] private SoundGroupSettings settings;
        [SerializeField, UsedImplicitly] private AudioClip audioClip;
        [SerializeField, UsedImplicitly] private LocalizedAudioClip localizedAudioClip;
        [SerializeField, UsedImplicitly] private float delay;
        [SerializeField, UsedImplicitly] private float speed = 1.0f;
        [SerializeField, UsedImplicitly] private float startTime;

        [SerializeField, UsedImplicitly, Range(0.0f, 1.0f)]
        private float volumeModifier = 1.0f;

        [SerializeField, UsedImplicitly] private List<SoundEntry> extraEntries = new();
        [SerializeField, UsedImplicitly] private List<SoundEntry> alternativeEntries = new();

        internal SoundGroupSettings Settings => settings;

        public AudioClip AudioClip => audioClip;
        public LocalizedAudioClip LocalizedAudioClip => localizedAudioClip;
        public float Delay => delay;
        public float Speed => speed;
        public float StartTime => startTime;
        public float VolumeModifier => volumeModifier;
        public IReadOnlyList<SoundEntry> ExtraEntries => extraEntries;
        public IReadOnlyList<SoundEntry> AlternativeEntries => alternativeEntries;

        public SoundPlayHandle Play(Vector3 point, Transform parent = null) => controller.Play(this, point, parent);

        public SoundLoadHandle PreloadSingle() => controller.Preload(this, true);

        public SoundLoadHandle PreloadAll() => controller.Preload(this, false);
    }
}