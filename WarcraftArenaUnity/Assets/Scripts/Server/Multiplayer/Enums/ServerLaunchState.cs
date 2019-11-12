using System;

namespace Server
{
    [Flags]
    public enum ServerLaunchState
    {
        SessionCreated = 1 << 0,
        MapLoaded = 1 << 1,

        Complete = SessionCreated | MapLoaded
    }
}
