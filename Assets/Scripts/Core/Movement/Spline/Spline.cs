using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace Core
{
    public class Spline : SplineBase
    {
        protected List<int> Lengths;


        public Spline()
        {
            Lengths = new List<int>();
        }

        /// <summary>
        /// Caclulates the position for given segment idx, and percent of segment length t.
        /// </summary>
        /// <param name="t">Percent of segment length, assumes that u in range [0, 1].</param>
        /// <param name="c">Evaluated position.</param>
        public void EvaluatePercent(float t, ref Vector3 c) { throw new NotImplementedException(); }
        /// <summary>
        /// Caclulates derivation for given percent of segment length t.
        /// </summary>
        /// <param name="t">Percent of segment length, assumes that u in range [0, 1].</param>
        /// <param name="hermite">Evaluated derivation.</param>
        public void EvaluateDerivative(float t, ref Vector3 hermite) { throw new NotImplementedException(); }
        /// <summary>
        /// Caclulates the position for given segment idx, and percent of segment length u.
        /// </summary>
        /// <param name="idx">Spline segment index, should be in range [first, last).</param>
        /// <param name="t">Percent of segment length, assumes that u in range [0, 1].</param>
        /// <param name="c">Evaluated position.</param>
        public new void EvaluatePercent(int idx, float t, ref Vector3 c) { base.EvaluatePercent(idx, t, ref c); }
        /// <summary>
        /// Caclulates derivation in index idx, and percent of segment length t.
        /// </summary>
        /// <param name="idx">Spline segment index, should be in range [first, last).</param>
        /// <param name="t">Percent of segment length, assumes that u in range [0, 1].</param>
        /// <param name="hermite">Evaluated derivation.</param>
        public new void EvaluateDerivative(int idx, float t, ref Vector3 hermite) { base.EvaluateDerivative(idx, t, ref hermite); }

        // Assumes that t in range [0, 1].
        public int ComputeIndexInBounds(float t) { throw new NotImplementedException(); }
        public int ComputeIndexInBounds(int length) { throw new NotImplementedException(); }
        public void ComputeIndex(float t, ref int outIdx, ref float outT) { throw new NotImplementedException(); }

        // Initializes spline. Don't call other methods while spline not initialized.
        public new void init_spline(Vector3 controls, int count, EvaluationMode m) { base.init_spline(controls, count, m); }
        public new void init_cyclic_spline(Vector3 controls, int count, EvaluationMode m, int cyclicPoint) { base.init_cyclic_spline(controls, count, m, cyclicPoint); }

        // Initializes lengths with SplineBase::SegLength method.
        public void InitLengths() { throw new NotImplementedException(); }
        // Initializes lengths in some custom way.
        public void InitLengths(ISplineInitializer cacher)
        {
            int i = IndexLo;
            int prevLength = 0;

            while (i < IndexHi)
            {
                var newLength = cacher.ComputeSplineTime(this, i);
                // length overflowed, assign to max positive value
                if (newLength < 0)
                    newLength = int.MaxValue;
                Lengths[++i] = newLength;

                Assert.IsTrue(prevLength <= newLength);
                prevLength = newLength;
            }
        }

        // Returns length of the whole spline.
        public int Length() { return Lengths[IndexHi];}
        // Returns length between given nodes.
        public int Length(int first, int last) { return Lengths[last] - Lengths[first];}
        public int Length(int idx) { return Lengths[idx];}

        public void SetLength(int i, int length) { Lengths[i] = length; }
        public new void Clear() { throw new NotImplementedException(); }
    }
}