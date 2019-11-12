using UnityEngine;

namespace Client
{
    public abstract class GameOptionItem : ScriptableObject
    {
        public abstract void Load();

        public abstract void Save();
    }
}
