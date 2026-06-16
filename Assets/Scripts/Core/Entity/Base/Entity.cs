using UnityEngine;
using Zenject;

namespace Core
{
    public abstract class Entity : MonoBehaviour
    {
        public abstract class CreateToken
        {
            public ulong Id { get; set; }
        }

        public bool IsValid { get; private set; }

        [Inject]
        internal World World { get; private set; }

        [Inject]
        internal BalanceReference Balance { get; private set; }

        public bool IsOwner => true;
        public bool IsController { get; private set; } = true;
        public ulong Id { get; private set; }

        public virtual void Attached(CreateToken token)
        {
            Id = token.Id;
            IsValid = true;

#if UNITY_EDITOR
            name = $"{GetType().Name} ({Id})";
#endif
        }

        public virtual void Detached()
        {
            IsValid = false;
        }

        internal abstract void DoUpdate(int deltaTime);
    }
}