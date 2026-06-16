using Common;
using UnityEngine;

namespace Client
{
    public abstract class InputAction : ScriptableUniqueInfo<InputAction>
    {
        public abstract void Execute();
    }
}
