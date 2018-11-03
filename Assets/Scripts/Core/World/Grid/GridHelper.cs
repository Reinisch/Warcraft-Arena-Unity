using System;
using UnityEngine;

namespace Core
{
    public class GridHelper
    {
        public const float SizeOfGrids = 533.3333f;
        public const float CenterGridOffset = SizeOfGrids / 2;

        public const int MinGridDelay = TimeHelper.Minute * TimeHelper.InMilliseconds;
        public const int MinMapUpdateDelay = 50;
        public const int DefaultVisibilityNotifyPeriod = 1000;
        public const int MinUnloadDelay = 1;                // immediate unload

        public const int MapIdInvalid = 0x7FFFFFFF;
        public const uint MapResolution = 128;

        public const float MaxHeight = 100000.0f;           // can be use for find ground height at surface
        public const float InvalidHeight = -100000.0f;      // for check, must be equal to VMAP_INVALID_HEIGHT, real value for unknown height is VMAP_INVALID_HEIGHT_VALUE
        public const float MaxFallDistance = 250000.0f;     // "unlimited fall" to find VMap ground if it is available, just larger than MAX_HEIGHT - INVALID_HEIGHT
        public const float DefaultHeightSearch = 50.0f;     // default search distance to find height at nearby locations
    }
}