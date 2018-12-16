using UdpKit;
using UnityEngine;
using UnityEngine.Assertions;

namespace Core
{
    public abstract class WorldEntity : Entity
    {
        public new class CreateInfo : Entity.CreateInfo
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

        private Map currMap;
        protected IWorldEntityState worldEntityState;

        public MovementInfo MovementInfo { get; } = new MovementInfo();
        public Vector3 Position { get { return transform.position; } set { transform.position = value; } }
        public Quaternion Rotation { get { return transform.rotation; } set { transform.rotation = value; } }

        public bool IsVisible { get; } = true;
        public string Name => name;
        public Map Map => currMap;

        public override void Attached()
        {
            base.Attached();

            worldEntityState = entity.GetState<IWorldEntityState>();

            var createInfo = entity.attachToken as CreateInfo;
            if (createInfo != null)
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

        public virtual void DoUpdate(int timeDelta)
        {

        }

        public virtual void SetMap(Map map)
        {
            Assert.IsNotNull(map);
            Assert.IsTrue(IsValid);

            if (currMap == map)
                return;

            currMap = map;
            currMap.AddWorldObject(this);
        }

        public virtual void ResetMap()
        {
            currMap = null;
        }
       
        public bool CanSeeOrDetect(WorldEntity target, bool ignoreStealth = false, bool distanceCheck = false, bool checkAlert = false)
        {
            return false;
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