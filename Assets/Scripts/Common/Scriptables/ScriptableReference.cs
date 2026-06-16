using UnityEngine;
using Zenject;

namespace Common
{
    /// <summary>
    /// Base class for game systems, that will be used to inject in behaviours.
    /// </summary>
    public abstract class ScriptableReference : MonoBehaviour
    {
        public void Register()
        {
            OnRegistered();
        }

        public void Unregister()
        {
            OnUnregister();
        }

        public void DoUpdate(float deltaTime)
        {
            OnUpdate(deltaTime);
        }

        public void DoUpdate(int deltaTime)
        {
            OnUpdate(deltaTime);
        }

        [Inject]
        protected virtual void QueueForInject(DiContainer container)
        {
        }

        protected virtual void OnRegistered() { }

        protected virtual void OnUnregister() { }

        protected virtual void OnUpdate(float deltaTime)
        {
        }

        protected virtual void OnUpdate(int deltaTime)
        {
        }
    }
}
