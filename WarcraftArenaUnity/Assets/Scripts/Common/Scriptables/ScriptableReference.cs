using UnityEngine;

namespace Common
{
    /// <summary>
    /// Base class for game systems, that will be used to inject in behaviours.
    /// </summary>
    public abstract class ScriptableReference : ScriptableObject
    {
        internal void Register()
        {
            OnRegistered();
        }

        internal void Unregister()
        {
            OnUnregister();
        }

        internal void DoUpdate(float deltaTime)
        {
            OnUpdate(deltaTime);
        }

        internal void DoUpdate(int deltaTime)
        {
            OnUpdate(deltaTime);
        }

        protected abstract void OnRegistered();

        protected abstract void OnUnregister();

        protected virtual void OnUpdate(float deltaTime)
        {
        }

        protected virtual void OnUpdate(int deltaTime)
        {
        }
    }
}
