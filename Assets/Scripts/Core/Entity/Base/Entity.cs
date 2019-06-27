using Bolt;
using JetBrains.Annotations;
using UdpKit;
using UnityEngine;

namespace Core
{
    public abstract class Entity : EntityBehaviour
    {
        public abstract class CreateToken : IProtocolToken
        {
            public abstract void Read(UdpPacket packet);

            public abstract void Write(UdpPacket packet);
        }

        [SerializeField, UsedImplicitly, Header(nameof(Entity))] private BalanceReference balance;

        protected BalanceReference Balance => balance;
        protected bool IsValid { get; private set; }

        internal WorldManager WorldManager { get; private set; }
        internal abstract bool AutoScoped { get; }

        public BoltEntity BoltEntity => entity;
        public bool IsOwner => entity.IsOwner;
        public bool IsController => entity.HasControl;
        public ulong Id => entity.NetworkId.PackedValue;

        [UsedImplicitly]
        protected virtual void Awake()
        {
        }

        public override void Attached()
        {
            base.Attached();

            IsValid = true;
        }

        public override void Detached()
        {
            IsValid = false;

            base.Detached();
        }

        internal virtual void DoUpdate(int deltaTime)
        {
        }

        internal void TakenFromPool(WorldManager worldManager)
        {
            WorldManager = worldManager;
        }

        internal void ReturnedToPool()
        {
            WorldManager = null;
        }
    }
}