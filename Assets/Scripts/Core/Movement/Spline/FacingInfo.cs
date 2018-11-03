using System;
using UnityEngine;

namespace Core
{
    public class FacingInfo
    {
        public Vector3 Facing { get; set; }
        public Guid Target { get; set; }
        public float Angle { get; set; }
        public MonsterMoveType Type { get; set; }

        public FacingInfo()
        {
            Angle = 0.0f;
            Type = MonsterMoveType.Normal;
        }
    }
}