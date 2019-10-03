using System.Collections.Generic;
using UnityEngine;

namespace Core
{
    public static class UnitUtils
    {
        public const ulong NoTargetId = 0u;
        public const int EmoteOneShotFrameThreshold = 120;
        public const int EmoteStateMovementFrameThreshold = 20;

        private static readonly Dictionary<EmoteType, bool> EmoteStateInfo = new Dictionary<EmoteType, bool>
        {
            {EmoteType.Flex, false}
        };

        public static bool IsOneShot(this EmoteType emoteType) => EmoteStateInfo.TryGetValue(emoteType, out bool isState) && !isState;

        public static bool IsState(this EmoteType emoteType) => EmoteStateInfo.TryGetValue(emoteType, out bool isState) && isState;

        public static bool IsDefined(this EmoteType emoteType) => EmoteStateInfo.ContainsKey(emoteType);

        public static bool ExistsIn(this Unit unit, WorldManager world)
        {
            if (unit == null || world == null || unit.Map == null)
                return false;

            return unit.Map.WorldManager == world;
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
    }
}