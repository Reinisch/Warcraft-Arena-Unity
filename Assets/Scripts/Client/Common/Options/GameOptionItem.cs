using Common;
using UnityEngine;

namespace Client
{
    public abstract class GameOptionItem : ScriptableUniqueInfo<GameOptionItem>
    {
        public abstract void Load();

        public abstract void Save();
    }
}
