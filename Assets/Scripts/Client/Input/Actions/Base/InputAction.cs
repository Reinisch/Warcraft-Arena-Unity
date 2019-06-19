using UnityEngine;

namespace Client
{
    public abstract class InputAction : ScriptableObject
    {
        public abstract void Execute();
    }
}
