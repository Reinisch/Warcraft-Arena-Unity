using Common;
using JetBrains.Annotations;
using UnityEngine;

namespace Client
{
    [CreateAssetMenu(fileName = "Game Option", menuName = "Player Data/Game Options/Bool Option", order = 1)]
    public sealed class GameOptionBool : GameOptionItem
    {
        [SerializeField, UsedImplicitly] private bool defaultValue;
        [SerializeField, UsedImplicitly] private bool currentValue;

        public bool Value => currentValue;

        public override void Load()
        {
            currentValue = PlayerPrefs.GetInt(name, defaultValue ? 1 : 0) == 1;
        }

        public override void Save()
        {
            PlayerPrefs.SetInt(name, Value ? 1 : 0);
        }

        public void Toggle()
        {
            currentValue = !currentValue;

            Save();

            EventHandler.ExecuteEvent(this, GameEvents.GameOptionChanged);
        }
    }
}
