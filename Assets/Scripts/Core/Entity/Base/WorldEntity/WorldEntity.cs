using System;
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
            public Map Map { get; set; }
        }

        private CreateToken createToken;

        internal MapGrid.Cell CurrentCell { get; set; }

        public Vector3 Position { get => transform.position; set => transform.position = value; }
        public Quaternion Rotation { get => transform.rotation; set => transform.rotation = value; }

        public abstract string Name { get; internal set; }
        public virtual float Size => StatUtils.DefaultEntitySize;

        public Map Map { get; private set; }

        public bool IsVisible { get; } = true;
        public bool IsVisibilityChanged { get; internal set; }
        public int StealthSubtlety { get; internal set; }
        public int StealthDetection { get; internal set; }
        public int InvisibilityPower { get; internal set; }
        public int InvisibilityDetection { get; internal set; }

        public event Action EventTeleported;

        public override void Attached(Entity.CreateToken token)
        {
            base.Attached(token);

            createToken = (CreateToken)token;
            Teleport(createToken.Position, notify: false);
            Rotation = createToken.Rotation;
        }

        internal virtual void UpdateVisibility(bool forced)
        {
            IsVisibilityChanged = true;
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

        public virtual void Teleport(Vector3 position, bool notify = true)
        {
            transform.position = position;

            if (notify)
                EventTeleported?.Invoke();
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

        public bool IsWithinDistance(WorldEntity target, float range, bool is3D)
        {
            float sizeDistance = Size + target.Size;
            float actualRange = range + sizeDistance;
            Vector3 position = Position;
            Vector3 targetPosition = target.Position;

            float dx = position.x - targetPosition.x;
            float dz = position.z - targetPosition.z;
            float sqrDistance = dx * dx + dz * dz;
            if (is3D)
            {
                float dy = position.y - targetPosition.y;
                sqrDistance += dy * dy;
            }

            return sqrDistance < actualRange * actualRange;
        }

        public float ExactDistanceTo(Vector3 position)
        {
            return Vector3.Distance(Position, position);
        }

        public float ExactDistanceTo(WorldEntity target)
        {
            return ExactDistanceTo(target.Position);
        }

        public float ExactDistanceSqrTo(WorldEntity target)
        {
            return ExactDistanceTo(target.Position);
        }
    }
}