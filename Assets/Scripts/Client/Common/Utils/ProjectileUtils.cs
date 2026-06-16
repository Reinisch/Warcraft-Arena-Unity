using System.Collections.Generic;
using UnityEngine;

namespace Client
{
    public static class ProjectileUtils
    {
        public class RaycastHitDistanceComparer : IComparer<RaycastHit>
        {
            public int Compare(RaycastHit x, RaycastHit y)
            {
                return x.distance.CompareTo(y.distance);
            }
        }
    }
}
