using JetBrains.Annotations;
using UnityEngine;

namespace Client
{
    public abstract class SoundController<TSoundKit, TKey> : MonoBehaviour where TSoundKit : SoundKit<TSoundKit, TKey>
    {
        [SerializeField, UsedImplicitly] private SoundReference sound;
        [SerializeField, UsedImplicitly] private AudioSource source;

        protected SoundReference Sound => sound;
        protected TSoundKit SoundKit { get; set; }

        public void PlayOneShot(TKey soundType) => SoundKit?.FindSound(soundType).PlayAtPoint(source.transform.position);
    }
}
