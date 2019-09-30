using UnityEngine;

namespace Client
{
    public class UnitSoundController : SoundController<UnitSoundKit, UnitSounds>
    {
        public void HandleModelChange(UnitModel newModel)
        {
            if (newModel == null)
                SoundKit = null;
            else if (Sound.UnitSoundKitsById.TryGetValue(newModel.Settings.DefaultSoundKit, out UnitSoundKit soundKit))
                SoundKit = soundKit;
            else
                Debug.LogError($"Sound kit with id={newModel.Settings.DefaultSoundKit} for model {newModel.name} not found!");
        }
    }
}
