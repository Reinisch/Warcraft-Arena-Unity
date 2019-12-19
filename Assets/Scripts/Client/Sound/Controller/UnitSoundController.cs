using Core;
using UnityEngine;

namespace Client
{
    public class UnitSoundController : SoundController<UnitSoundKit, UnitSounds>
    {
        public override void PlayOneShot(UnitSounds soundType)
        {
            if (Source.isPlaying)
                Source.Stop();

            base.PlayOneShot(soundType);
        }

        public void HandleModelChange(UnitModel newModel)
        {
            if (Source.isPlaying)
                Source.Stop();

            if (newModel == null)
                SoundKit = null;
            else
            {
                SoundKit = newModel.Settings.SoundKit;

                if (SoundKit == null)
                    Debug.LogError($"Sound kit for model {newModel.name} not found!");
            }
        }

        public void HandleEmote(EmoteType emoteType)
        {
            if (Sound.UnitSoundByEmoteType.TryGetValue(emoteType, out UnitSounds soundType))
                PlayOneShot(soundType);
        }
    }
}
