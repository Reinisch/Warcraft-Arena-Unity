using System;
using UnityEngine;

namespace Core
{
    public class DynamicMapTree
    {
        public int Size { get { throw new NotImplementedException(); } }


        public DynamicMapTree() { throw new NotImplementedException(); }

        public bool IsInLineOfSight(float x1, float y1, float z1, float x2, float y2, float z2, uint phasemask) { throw new NotImplementedException(); }
        public bool GetIntersectionTime(uint phasemask, ref Ray ray, ref Vector3 endPos, ref float maxDist) { throw new NotImplementedException(); }
        public bool GetObjectHitPos(uint phasemask, ref Vector3 pPos1, ref Vector3 pPos2, ref Vector3 pResultHitPos, float pModifyDist) { throw new NotImplementedException(); }
        public float GetHeight(float x, float y, float z, float maxSearchDist, uint phasemask) { throw new NotImplementedException(); }

        public void Insert(GameEntityModel model) { throw new NotImplementedException(); }
        public void Remove(GameEntityModel model) { throw new NotImplementedException(); }
        public bool Contains(GameEntityModel model) { throw new NotImplementedException(); }

        public void Balance() { throw new NotImplementedException(); }
        public void DoUpdate(int timeDiff) { throw new NotImplementedException(); }
    }
}