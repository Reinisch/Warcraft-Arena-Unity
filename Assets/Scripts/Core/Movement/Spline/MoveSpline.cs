using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace Core
{
    public class MoveSpline
    {
        [Flags]
        public enum UpdateResult
        {
            None = 0x01,
            Arrived = 0x02,
            NextCycle = 0x04,
            NextSegment = 0x08,
        }


        public Spline Spline { get; protected set; }
        public bool Initialized => !Spline.Empty();
        public bool OnTransport { get; set; }
        public bool SplineIsFacingOnly { get; set; }

        public int Duration => Spline.Length();
        public int CurrentSplineIdx() { return PointIdx; }


        protected uint Id { get; set; }
        protected FacingInfo Facing { get; set; }
        protected MoveSplineFlags Splineflags { get; set; }

        protected int TimePassed { get; set; }
        protected int TimeElapsed => Duration - TimePassed;
        protected int NextTimestamp => Spline.Length(PointIdx + 1);
        protected int SegmentTimeElapsed => NextTimestamp - TimePassed;
        protected int EffectStartTime { get; set; }
        protected int PointIdx { get; set; }
        protected int PointIdxOffset { get; set; }

        protected float VerticalAcceleration { get; set; }
        protected float InitialOrientation { get; set; }
   

        public MoveSpline()
        {
            Id = 0;
            TimePassed = 0;
            VerticalAcceleration = 0.0f;
            InitialOrientation = 0.0f;
            EffectStartTime = 0;
            PointIdx = 0;
            PointIdxOffset = 0;
            OnTransport = false;
            SplineIsFacingOnly = false;

            Splineflags = Splineflags.SetFlag(MoveSplineFlags.Done);
        }

        public void Initialize(MoveSplineInitArgs args) { throw new NotImplementedException(); }
        public void Deinitialize() { throw new NotImplementedException(); }
        public void Interrupt() { Splineflags = Splineflags.SetFlag(MoveSplineFlags.Done); }
        public void UpdateState(int difftime)
        {
            Assert.IsTrue(Initialized);

            do ProcessSpline(ref difftime);
            while (difftime > 0);
        }


        public uint GetId() { return Id; }
        public bool Finalized() { return Splineflags.HasFlag(MoveSplineFlags.Done); }
        public bool IsCyclic() { return Splineflags.HasFlag(MoveSplineFlags.Cyclic); }
        public bool IsFalling() { return Splineflags.HasFlag(MoveSplineFlags.Falling); }
        public Position ComputePosition() { throw new NotImplementedException(); }
        public Vector3 FinalDestination() { return Initialized ? Spline.GetPoint(Spline.Last()) : Vector3.zero; }
        public Vector3 CurrentDestination() { return Initialized ? Spline.GetPoint(PointIdx + 1) : Vector3.zero; }
        public int CurrentPathIdx() { throw new NotImplementedException(); }
        public override string ToString() { throw new NotImplementedException(); }


        protected void InitSpline(MoveSplineInitArgs args) { throw new NotImplementedException(); }

        protected List<Vector3> GetPath() { return Spline.GetPoints(); }
        protected void ComputeParabolicElevation(ref float el) { throw new NotImplementedException(); }
        protected void ComputeFallElevation(ref float el) { throw new NotImplementedException(); }
        protected UpdateResult ProcessSpline(ref int msTimeDiff) { throw new NotImplementedException(); }
    }
}