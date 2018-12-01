using System;
using System.Collections.Generic;
using UnityEngine;

namespace Core
{
    public abstract class SplineBase
    {
        public delegate void CustomSplineInitializer(out EvaluationMode mode, out bool cyclic, List<Vector3> points, out int lo, out int hi);

        public enum EvaluationMode : byte
        {
            ModeLinear,
            ModeCatmullrom,
            ModeBezier3Unused,
            UninitializedMode,
        }

        private readonly List<Vector3> points;

        protected int indexLo;
        protected int indexHi;

        private EvaluationMode splineMode;
        private bool cyclic;
    
        protected SplineBase()
        {
            splineMode = EvaluationMode.UninitializedMode;

            points = new List<Vector3>();
        }

        protected void EvaluateLinear(int segment, float length, ref Vector3 result) { throw new NotImplementedException(); }
        protected void EvaluateCatmullRom(int segment, float length, ref Vector3 result) { throw new NotImplementedException(); }
        protected void EvaluateBezier3(int segment, float length, ref Vector3 result) { throw new NotImplementedException(); }

        protected void EvaluateDerivativeLinear(int segment, float length, ref Vector3 result) { throw new NotImplementedException(); }
        protected void EvaluateDerivativeCatmullRom(int segment, float length, ref Vector3 result) { throw new NotImplementedException(); }
        protected void EvaluateDerivativeBezier3(int segment, float length, ref Vector3 result) { throw new NotImplementedException(); }

        protected float SegLengthLinear(int segment) { throw new NotImplementedException(); }
        protected float SegLengthCatmullRom(int segment) { throw new NotImplementedException(); }
        protected float SegLengthBezier3(int segment) { throw new NotImplementedException(); }

        protected void InitLinear(Vector3 controls, int count, int point) { throw new NotImplementedException(); }
        protected void InitCatmullRom(Vector3 controls, int count, int point) { throw new NotImplementedException(); }
        protected void InitBezier3(Vector3 controls, int count, int point) { throw new NotImplementedException(); }

        /// <summary>
        /// Caclulates the position for given segment idx, and percent of segment length u.
        /// </summary>
        /// <param name="idx">Spline segment index, should be in range [first, last).</param>
        /// <param name="u">Percent of segment length, assumes that u in range [0, 1].</param>
        /// <param name="c">Evaluated position.</param>
        protected void EvaluatePercent(int idx, float u, ref Vector3 c)
        {
            switch (splineMode)
            {
                case EvaluationMode.ModeLinear:
                    EvaluateLinear(idx, u, ref c);
                    break;
                case EvaluationMode.ModeCatmullrom:
                    EvaluateCatmullRom(idx, u, ref c);
                    break;
                case EvaluationMode.ModeBezier3Unused:
                    EvaluateBezier3(idx, u, ref c);
                    break;
                case EvaluationMode.UninitializedMode:
                    break;
            }
        
        }
        /// <summary>
        /// Caclulates derivation in index idx, and percent of segment length u.
        /// </summary>
        /// <param name="idx">Spline segment index, should be in range [first, last).</param>
        /// <param name="u">Percent of segment length, assumes that u in range [0, 1].</param>
        /// <param name="hermite">Evaluated derivation.</param>
        protected void EvaluateDerivative(int idx, float u, ref Vector3 hermite)
        {
            switch (splineMode)
            {
                case EvaluationMode.ModeLinear:
                    EvaluateDerivativeLinear(idx, u, ref hermite);
                    break;
                case EvaluationMode.ModeCatmullrom:
                    EvaluateDerivativeCatmullRom(idx, u, ref hermite);
                    break;
                case EvaluationMode.ModeBezier3Unused:
                    EvaluateDerivativeBezier3(idx, u, ref hermite);
                    break;
                case EvaluationMode.UninitializedMode:
                    break;
            }
        }

        // bounds for spline indexes, all indexes should be in range [first, last).
        public int First() { return indexLo;}
        public int Last() { return indexHi;}

        public bool Empty() { return indexLo == indexHi;}
        public EvaluationMode Mode() { return splineMode;}
        public bool IsCyclic() { return cyclic;}

        public List<Vector3> GetPoints() { return points; }
        public int GetPointCount() { return points.Count; }
        public Vector3 GetPoint(int i) { return points[i]; }

        // initializes spline, don't call other methods while spline not initialized.
        public void init_spline(Vector3 controls, int count, EvaluationMode m) { throw new NotImplementedException(); }
        public void init_cyclic_spline(Vector3 controls, int count, EvaluationMode m, int cyclicPoint) { throw new NotImplementedException(); }
        public void init_spline_custom(CustomSplineInitializer initializer)
        {
            initializer(out splineMode, out cyclic, points, out indexLo, out indexHi);
        }

        public void Clear() { throw new NotImplementedException(); }
        // calculates distance between [i; i+1] points, assumes that index i is in bounds.
        public float SegLength(int i)
        {
            switch (splineMode)
            {
                case EvaluationMode.ModeLinear:
                    return SegLengthLinear(i);
                case EvaluationMode.ModeCatmullrom:
                    return SegLengthCatmullRom(i);
                case EvaluationMode.ModeBezier3Unused:
                    return SegLengthBezier3(i);
                default:
                    return 0.0f;
            }
        }
        public override string ToString() { throw new NotImplementedException(); }
    }
}