using JetBrains.Annotations;
using UnityEngine;

namespace Core
{
    public abstract class SingletonGameObject<T> : MonoBehaviour where T: MonoBehaviour
    {
        public static T Instance { get; private set; }

        [UsedImplicitly]
        protected void Awake()
        {
            if (Instance != null)
                Destroy(this);
            else
                Instance = GetComponent<T>();
        }
    }
}
