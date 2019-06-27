using Bolt.Utils;
using UdpKit;
using UnityEngine;
using Common;

namespace Core
{
    public abstract class WorldEntity : Entity
    {
        public new abstract class CreateToken : Entity.CreateToken
        {
            public Vector3 Position { get; set; }
            public Quaternion Rotation { get; set; }

            public override void Read(UdpPacket packet)
            {
                Position = packet.ReadVector3();
                Rotation = packet.ReadQuaternion();
            }

            public override void Write(UdpPacket packet)
            {
                packet.WriteVector3(Position);
                packet.WriteQuaternion(Rotation);
            }
        }

        private IWorldEntityState worldEntityState;

        public MovementInfo MovementInfo { get; } = new MovementInfo();
        public Vector3 Position { get => transform.position; set => transform.position = value; }
        public Quaternion Rotation { get => transform.rotation; set => transform.rotation = value; }

        public abstract string Name { get; internal set; }
        public bool IsVisible { get; } = true;

        public Map Map { get; private set; }
        public GridCell CurrentCell { get; internal set; }

        public override void Attached()
        {
            worldEntityState = entity.GetState<IWorldEntityState>();

            base.Attached();

            worldEntityState.SetTransforms(worldEntityState.Transform, transform);

            if (entity.AttachToken is CreateToken createInfo)
            {
                Position = createInfo.Position;
                Rotation = createInfo.Rotation;
            }
        }

        public override void Detached()
        {
            worldEntityState = null;

            base.Detached();
        }

        internal void SetMap(Map map)
        {
            Assert.IsNotNull(map);
            Assert.IsTrue(IsValid);

            if (Map == map)
                return;

            Map = map;
            Map.AddWorldEntity(this);
        }

        internal void ResetMap()
        {
            Map.RemoveWorldEntity(this);

            Map = null;
        }

        public bool CanSeeOrDetect(WorldEntity target, bool ignoreStealth = false, bool distanceCheck = false, bool checkAlert = false)
        {
            return true;
        }

        public float DistanceTo(Vector3 position)
        {
            return Vector3.Distance(Position, position);
        }

        public float DistanceTo(WorldEntity target)
        {
            return DistanceTo(target.Position);
        }

        public void SetFacingTo(WorldEntity target)
        {
            SetFacingTo(target.Position);
        }

        public void SetFacingTo(Vector3 position)
        {
            Rotation = Quaternion.LookRotation(position - Position);
        }

        public int DistanceSortOrder(WorldEntity targetX, WorldEntity targetY)
        {
            return DistanceTo(targetX).CompareTo(DistanceTo(targetY));
        }
    }
}