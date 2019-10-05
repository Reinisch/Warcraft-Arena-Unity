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
            else if (Sound.UnitSoundKitsById.TryGetValue(newModel.Settings.DefaultSoundKit, out UnitSoundKit soundKit))
                SoundKit = soundKit;
            else
                Debug.LogError($"Sound kit with id={newModel.Settings.DefaultSoundKit} for model {newModel.name} not found!");
        }

        public void HandleEmote(EmoteType emoteType)
        {
            if (Sound.UnitSoundByEmoteType.TryGetValue(emoteType, out UnitSounds soundType))
                PlayOneShot(soundType);
        }
    }
}
