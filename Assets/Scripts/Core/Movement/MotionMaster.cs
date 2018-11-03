using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace Core
{
    public class MotionMaster
    {
        private Dictionary<MovementSlot, MovementGenerator> impl;
        private Dictionary<MovementSlot, bool> needInit;

        private MovementSlot top;
        private MotionCleanFlag cleanFlag;


        public MotionMaster()
        {
            impl = new Dictionary<MovementSlot, MovementGenerator>();
            needInit = new Dictionary<MovementSlot, bool>();

            top = (MovementSlot)(-1);
            cleanFlag = MotionCleanFlag.None;

            foreach (MovementSlot slot in Enum.GetValues(typeof(MovementSlot)))
            {
                impl.Add(slot, null);
                needInit.Add(slot, true);
            }
        }

        public void Initialize() { }
        public void InitDefault() { }

        public bool Empty() { return top < 0; }
        public int Size() { return (int)top + 1; }

        public MovementGenerator Top()
        {
            Assert.IsFalse(Empty(), "Motion Master has nothing on top!");
            return impl[top];
        }
        public MovementGenerator GetMotionSlot(MovementSlot slot)
        {
            Assert.IsTrue(slot >= 0);
            return impl[slot];
        }

        public void DirectDelete(MovementGenerator curr) { }
        public void DelayedDelete(MovementGenerator curr) { }

        public void UpdateMotion(uint diff) { }
        public void Clear(bool reset = true)
        {
            if ((cleanFlag & MotionCleanFlag.Update) > 0)
            {
                if (reset)
                    cleanFlag |= MotionCleanFlag.Reset;
                else
                    cleanFlag &= ~MotionCleanFlag.Reset;
                DelayedClean();
            }
            else
                DirectClean(reset);
        }
        public void MovementExpired(bool reset = true)
        {
            if ((cleanFlag & MotionCleanFlag.Update) > 0)
            {
                if (reset)
                    cleanFlag |= MotionCleanFlag.Reset;
                else
                    cleanFlag &= ~MotionCleanFlag.Reset;
                DelayedExpire();
            }
            else
                DirectExpire(reset);
        }

        public void MoveIdle() { }
        public void MoveTargetedHome() { }
        public void MoveRandom(float spawndist = 0.0f) { }
        public void MoveFollow(Unit target, float dist, float angle, MovementSlot slot = MovementSlot.Active) { }
        public void MoveChase(Unit target, float dist = 0.0f, float angle = 0.0f) { }
        public void MoveConfused() { }
        public void MoveFleeing(Unit enemy, uint time = 0) { }
        public void MovePoint(uint id, Position pos, bool generatePath = true)
        {
            MovePoint(id, pos.X, pos.Y, pos.Z, generatePath);
        }
        public void MovePoint(uint id, float x, float y, float z, bool generatePath = true) { }

        /// <summary>
        /// Makes the unit move toward the target until it is at a certain distance from it. The unit then stops.
        /// Only works in 2D. This method doesn't account for any movement done by the target. in other words, it only works if the target is stationary.
        /// </summary>
        public void MoveCloserAndStop(uint id, Unit target, float distance) { }
        /// <summary>
        /// These two movement types should only be used with creatures having landing/takeoff animations.
        /// </summary>
        public void MoveLand(uint id, Position pos) { throw new NotImplementedException(); }
        public void MoveTakeoff(uint id, Position pos) { throw new NotImplementedException(); }
        public void MoveCharge(float x, float y, float z, float speed = MovementHelper.SpeedCharge,
            EventId id = EventId.Charge, bool generatePath = false)
        {
            throw new NotImplementedException();
        }
        public void MoveCharge(PathGenerator path, float speed = MovementHelper.SpeedCharge)
        {
            throw new NotImplementedException();
        }
        public void MoveKnockbackFrom(float srcX, float srcY, float speedXy, float speedZ)
        {
            throw new NotImplementedException();
        }
        public void MoveJumpTo(float angle, float speedXy, float speedZ)
        {
            throw new NotImplementedException();
        }
        public void MoveJump(Position pos, float speedXy, float speedZ, EventId id = EventId.Jump, 
            bool hasOrientation = false, uint arrivalSpellId = 0, Guid arrivalSpellTargetGuid = default(Guid))
        {
            MoveJump(pos.X, pos.Y, pos.Z, pos.Orientation, speedXy, speedZ, id, hasOrientation, arrivalSpellId, arrivalSpellTargetGuid);
        }
        public void MoveJump(float x, float y, float z, float o, float speedXy, float speedZ, EventId id = EventId.Jump,
            bool hasOrientation = false, uint arrivalSpellId = 0, Guid arrivalSpellTargetGuid = default(Guid))
        {
            throw new NotImplementedException();
        }
        public void MoveCirclePath(float x, float y, float z, float radius, bool clockwise, byte stepCount)
        {
            throw new NotImplementedException();
        }
        public void MoveSmoothPath(uint pointId, List<Vector3> pathPoints, int pathSize, bool walk)
        {
            throw new NotImplementedException();
        }
        public void MoveFall(uint id = 0)
        {
            throw new NotImplementedException();
        }

        public void MoveSeekAssistance(float x, float y, float z) { throw new NotImplementedException(); }
        public void MoveSeekAssistanceDistract(uint timer) { throw new NotImplementedException(); }
        public void MoveTaxiFlight(uint path, uint pathnode) { throw new NotImplementedException(); }
        public void MoveDistract(int time) { throw new NotImplementedException(); }
        public void MovePath(uint pathID, bool repeatable) { throw new NotImplementedException(); }
        public void MoveRotate(uint time, RotateDirection direction) { throw new NotImplementedException(); }

        public MovementGeneratorType GetCurrentMovementGeneratorType() { throw new NotImplementedException(); }
        public MovementGeneratorType GetMotionSlotType(int slot) { throw new NotImplementedException(); }

        public void PropagateSpeedChange() { throw new NotImplementedException(); }
        public bool GetDestination(ref float x, ref float y, ref float z) { throw new NotImplementedException(); }

        private void Pop()
        {
            if (Empty())
                return;

            impl[top] = null;
            while (!Empty() && Top() != null)
                --top;
        }
        private void Push(MovementGenerator generator)
        {
            ++top;
            impl[top] = generator;
        }
        private bool NeedInitTop()
        {
            if (Empty())
                return false;
            return needInit[top];
        }
        private void InitTop() { }
        private void Mutate(MovementGenerator m, MovementSlot slot) { throw new NotImplementedException(); }

        private void DirectClean(bool reset) { throw new NotImplementedException(); }
        private void DelayedClean() { throw new NotImplementedException(); }
        private void DirectExpire(bool reset) { throw new NotImplementedException(); }
        private void DelayedExpire() { throw new NotImplementedException(); }
    }
}