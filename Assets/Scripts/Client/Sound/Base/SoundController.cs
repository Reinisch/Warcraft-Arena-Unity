using UnityEngine;
using Zenject;

namespace Client.Sound
{
    public abstract class SoundController<TSoundKit, TKey> : MonoBehaviour where TSoundKit : SoundKit<TSoundKit, TKey>
    {
        [Inject] private SoundModule sound;

        protected SoundModule Sound => sound;
        protected SoundPlayHandle LastSound { get; private set; }
        protected TSoundKit SoundKit { get; set; }

        protected TKey LastSoundType { get; private set; }

        public virtual void PlayOneShot(TKey soundType)
        {
            if (SoundKit == null)
                return;

            SoundEntry entry = SoundKit.FindSound(soundType, false);
            if (entry == null)
                return;

            LastSoundType = soundType;

            PlayOneShot(entry);
        }

        public void PlayOneShot(SoundEntry soundEntry)
        {
            LastSound.Release();
            LastSound = soundEntry.Play(transform.position);
        }
    }
}
