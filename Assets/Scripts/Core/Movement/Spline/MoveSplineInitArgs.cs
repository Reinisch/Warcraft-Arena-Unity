using System;
using System.Collections.Generic;
using UnityEngine;

namespace Core
{
    public class MoveSplineInitArgs
    {
        public List<Vector3> Path { get; set; }
        public FacingInfo Facing { get; set; }
        public MoveSplineFlags Flags { get; set; }
        public int PathIdxOffset { get; set; }
        public float Velocity { get; set; }
        public float ParabolicAmplitude { get; set; }
        public float TimePerc { get; set; }
        public uint SplineId { get; set; }
        public float InitialOrientation { get; set; }
        public bool HasVelocity { get; set; }
        public bool TransformForTransport { get; set; }


        public MoveSplineInitArgs(int pathCapacity = 16)
        {
            PathIdxOffset = 0;
            Velocity = 0.0f;
            ParabolicAmplitude = 0.0f;
            TimePerc = 0.0f;
            SplineId = 0;
            InitialOrientation = 0.0f;
            HasVelocity = false;
            TransformForTransport = true;

            Path = new List<Vector3>(pathCapacity);
        }

        // Returns true to show that the arguments were configured correctly and MoveSpline initialization will succeed.
        public bool Validate(Unit unit) { throw new NotImplementedException(); }

        private bool CheckPathLengths() { throw new NotImplementedException(); }
    }
}