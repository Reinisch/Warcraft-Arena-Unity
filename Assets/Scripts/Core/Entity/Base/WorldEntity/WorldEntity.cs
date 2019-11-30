using System;
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
        private CreateToken createToken;

        public Vector3 Position { get => transform.position; set => transform.position = value; }
        public Quaternion Rotation { get => transform.rotation; set => transform.rotation = value; }

        public abstract string Name { get; internal set; }
        public bool IsVisible { get; } = true;

        public Map Map { get; private set; }
        public GridCell CurrentCell { get; internal set; }
        public bool VisibilityChanged { get; internal set; }

        public override void Attached()
        {
            base.Attached();

            worldEntityState = entity.GetState<IWorldEntityState>();
            worldEntityState.SetTransforms(worldEntityState.Transform, transform);

            createToken = (CreateToken) entity.AttachToken;
            Position = createToken.Position;
            Rotation = createToken.Rotation;
        }

        public override void Detached()
        {
            worldEntityState.SetTransforms(worldEntityState.Transform, null);
            worldEntityState = null;
            createToken = null;

            base.Detached();
        }

        internal void PrepareForScoping()
        {
            if (IsOwner)
            {
                createToken.Position = Position;
                createToken.Rotation = Rotation;
            }
        }

        internal void UpdateSyncTransform(bool shouldSync)
        {
            worldEntityState.SetTransforms(worldEntityState.Transform, shouldSync ? transform : null);
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
            if (this == target)
                return true;

            return true;
        }

        public bool IsFacing(WorldEntity target, SpellTargetDirections direction, float angle, float backBuffer = StatUtils.DefaultCombatReach)
        {
            Vector3 facingDirection;
            switch (direction)
            {
                case SpellTargetDirections.Front:
                    facingDirection = transform.forward;
                    break;
                case SpellTargetDirections.Back:
                    facingDirection = -transform.forward;
                    break;
                case SpellTargetDirections.Right:
                    facingDirection = transform.right;
                    break;
                case SpellTargetDirections.Left:
                    facingDirection = -transform.right;
                    break;
                case SpellTargetDirections.FrontRight:
                    facingDirection = transform.forward + transform.right;
                    break;
                case SpellTargetDirections.BackRight:
                    facingDirection = -transform.forward + transform.right;
                    break;
                case SpellTargetDirections.BackLeft:
                    facingDirection = -transform.forward - transform.right;
                    break;
                case SpellTargetDirections.FrontLeft:
                    facingDirection = transform.forward - transform.right;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(direction), direction, "Unknown direction type!");
            }

            Vector3 projectedFacingDirection = Vector3.ProjectOnPlane(facingDirection, Vector3.up);
            Vector3 pointOfView = Position - projectedFacingDirection.normalized * backBuffer;
            Vector3 targetDirection = Vector3.ProjectOnPlane(target.Position - pointOfView, Vector3.up);
            return Vector3.Angle(targetDirection, projectedFacingDirection) < angle;
        }

        public float DistanceTo(Vector3 position)
        {
            return Vector3.Distance(Position, position);
        }

        public float DistanceSqrTo(Vector3 position)
        {
            return Vector3.SqrMagnitude(Position - position);
        }

        public float DistanceTo(WorldEntity target)
        {
            return DistanceTo(target.Position);
        }

        public float DistanceSqrTo(WorldEntity target)
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
    }
}