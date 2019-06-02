using UnityEngine;

namespace Common
{
    public abstract class SingletonBehaviour<T> : MonoBehaviour where T: MonoBehaviour
    {
        public static T Instance { get; private set; }

        protected virtual void Initialize()
        {
            if (Instance != null)
                Destroy(this);
            else
                Instance = GetComponent<T>();
        }

        protected virtual void Deinitialize()
        {
            Instance = null;
        }
    }
}
