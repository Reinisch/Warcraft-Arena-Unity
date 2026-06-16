using Client.Sound;
using Core;
using UnityEngine;
using Zenject;

namespace Client
{
    public class UnitSoundController : SoundController<UnitSoundKit, UnitSounds>
    {
        [Inject] private SoundReference soundModule;

        public void HandleModelChange(UnitModel newModel)
        {
            LastSound.Release();

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
            if (soundModule.UnitSoundByEmoteType.TryGetValue(emoteType, out UnitSounds soundType))
                PlayOneShot(soundType);
        }
    }
}
