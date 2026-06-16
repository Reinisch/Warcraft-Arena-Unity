using Common;
using UnityEngine;

namespace Client.Sound
{
    internal sealed class SoundHandleComponent : MonoBehaviour
    {
        private SoundPlayHandle handle;
        private bool isReleased;

        [field: SerializeField]
        public AudioSource Source { get; private set; }

        internal SoundHandleComponent Create(AudioSource source)
        {
            Source = source;
            return this;
        }

        internal void Play(SoundPlayHandle handle)
        {
            this.handle = handle;
            isReleased = false;
        }

        internal void Release()
        {
            if (isReleased)
                return;

            isReleased = true;
            Source.Stop();
            Source.clip = null;
            GameObjectPool.Return(gameObject, false);
        }

        private void OnDisable() =>  handle.Release();
    }
}
