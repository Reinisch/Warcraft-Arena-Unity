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

        internal WorldManager World { get; private set; }
        internal abstract bool AutoScoped { get; }

        public BoltEntity BoltEntity => entity;
        public bool IsOwner => entity.IsOwner;
        public bool IsController => entity.HasControl;
        public ulong Id { get; private set; }

        public override void Attached()
        {
            base.Attached();

            Id = entity.NetworkId.PackedValue;
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
            World = worldManager;
        }

        internal void ReturnedToPool()
        {
            World = null;
        }
    }
}