using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Core
{
    public static class UnitUtils
    {
        public const ulong NoTargetId = 0u;
        public const string PreferredClassPrefName = nameof(PreferredClassPrefName);
        public const int EmoteOneShotFrameThreshold = 120;
        public const int EmoteStateMovementFrameThreshold = 20;
        public const int MaxUnitPowers = 6;

        private static readonly Dictionary<EmoteType, bool> EmoteStateInfo = new Dictionary<EmoteType, bool>
        {
            {EmoteType.Beg, false},
            {EmoteType.Bow, false},
            {EmoteType.Cheer, false},
            {EmoteType.Chicken, false},
            {EmoteType.Cry, false},
            {EmoteType.Dance, true},
            {EmoteType.Flex, false},
            {EmoteType.Kiss, false},
            {EmoteType.Laugh, false},
            {EmoteType.No, false},
            {EmoteType.Point, false},
            {EmoteType.Read, true},
            {EmoteType.Roar, false},
            {EmoteType.Rude, false},
            {EmoteType.Shout, false},
            {EmoteType.Shy, false},
            {EmoteType.Talk, true},
            {EmoteType.TalkExclamation, false},
            {EmoteType.TalkQuestion, false},
            {EmoteType.Train, false},
            {EmoteType.Wave, false},
            {EmoteType.Yes, false}
        };

        private static readonly HashSet<ClassType> PlayerClasses = new HashSet<ClassType>(Enum.GetValues(typeof(ClassType)).Cast<ClassType>());

        public static bool IsOneShot(this EmoteType emoteType) => EmoteStateInfo.TryGetValue(emoteType, out bool isState) && !isState;

        public static bool IsState(this EmoteType emoteType) => EmoteStateInfo.TryGetValue(emoteType, out bool isState) && isState;

        public static bool IsDefined(this EmoteType emoteType) => EmoteStateInfo.ContainsKey(emoteType);

        public static bool IsDefined(this ClassType classType) => PlayerClasses.Contains(classType);

        public static bool ExistsIn(this Unit unit, World world)
        {
            if (unit == null || world == null || unit.Map == null)
                return false;

            return unit.Map.World == world;
        }

        public static bool ExistsIn(this Unit unit, Map map)
        {
            if (unit == null || map == null)
                return false;

            return unit.Map == map;
        }

        public static bool InRangeSqr(this WorldEntity entity, WorldEntity target, float sqrRange)
        {
            return Vector3.SqrMagnitude(entity.Position - target.Position) <= sqrRange;
        }

        public static bool InRange(this WorldEntity entity, WorldEntity target, float range)
        {
            return Vector3.Magnitude(entity.Position - target.Position) <= range;
        }

        public static bool HasTargetFlag(this UnitFlags baseFlags, UnitFlags flag)
        {
            return (baseFlags & flag) == flag;
        }
    }
}