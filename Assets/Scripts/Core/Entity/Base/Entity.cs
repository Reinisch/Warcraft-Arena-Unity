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

        [SerializeField, UsedImplicitly, Header("Entity")] private BalanceReference balance;

        protected BalanceReference Balance => balance;
        internal WorldManager WorldManager { get; private set; }
        internal abstract bool AutoScoped { get; }

        public BoltEntity BoltEntity => entity;
        public bool IsOwner => entity.isOwner;
        public bool IsController => entity.hasControl;
        public bool IsValid { get; private set; }
        public ulong NetworkId => entity.networkId.PackedValue;

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

        public abstract void Accept(IVisitor visitor);

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