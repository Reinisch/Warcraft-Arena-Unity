using JetBrains.Annotations;
using UnityEngine;

namespace Core
{
    public interface ISingletonGameObject
    {
        void Initialize();
        void Deinitialize();
        void DoUpdate(int deltaTime);
    }

    public abstract class SingletonGameObject : MonoBehaviour, ISingletonGameObject
    {
        public virtual void Initialize() { }

        public virtual void Deinitialize() { }

        public virtual void DoUpdate(int deltaTime) { }
    }

    public abstract class SingletonGameObject<T> : SingletonGameObject where T: MonoBehaviour
    {
        public static T Instance { get; private set; }

        [UsedImplicitly]
        protected virtual void Awake()
        {
            if (Instance != null)
                Destroy(this);
            else
                Instance = GetComponent<T>();
        }
    }
}
