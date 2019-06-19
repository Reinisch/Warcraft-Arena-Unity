using JetBrains.Annotations;
using UnityEngine;

namespace Client
{
    public class InputManager : MonoBehaviour
    {
        [SerializeField, UsedImplicitly] private InputReference inputReference;

        public void Initialize()
        {
            inputReference.Initialize();
        }

        public void Deinitialize()
        {
            inputReference.Deinitialize();
        }

        public void DoUpdate(float deltaTime)
        {
            inputReference.DoUpdate(deltaTime);
        }
    }
}
