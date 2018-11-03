// client side game entity show states

namespace Core
{
    public enum GoState
    {
        Active = 0,                     // show in world as used and not reset (closed door open)
        Ready = 1,                      // show in world as ready (closed door close)
        ActiveAlternative = 2,          // show in world as used in alt way and not reset (closed door open by cannon fire)
        TransportActive = 24,
        TransportStopped = 25
    }

    public enum LootState
    {
        NotReady = 0,
        Ready,                          // can be ready but despawned, and then not possible activate until spawn
        Activated,
        JustDeactivated
    }

    public enum GameEntityFlags
    {
        InUse = 0x00000001,                             // disables interaction while animated
        Locked = 0x00000002,                            // require key, spell, event, etc to be opened. Makes "Locked" appear in tooltip
        InteractCond = 0x00000004,                      // cannot interact (condition to interact - requires GO_DYNFLAG_LO_ACTIVATE to enable interaction clientside)
        Transport = 0x00000008,                         // any kind of transport? Object can transport (elevator, boat, car)
        NotSelectable = 0x00000010,                     // not selectable even in GM mode
        NoDespawn = 0x00000020,                         // never despawn, typically for doors, they just change state
        AIObstacle = 0x00000040,                        // makes the client register the object in something called AIObstacleMgr, unknown what it does
        FreezeAnimation = 0x00000080,
        Damaged = 0x00000200,
        Destroyed = 0x00000400,
        InteractDistanceUsesTemplateModel = 0x00080000, // client checks interaction distance from model sent in SMSG_QUERY_GAMEOBJECT_RESPONSE instead of GAMEOBJECT_DISPLAYID
        MapEntity = 0x00100000                          // pre-7.0 model loading used to be controlled by file extension (wmo vs m2)
    }

    public enum GameEntityDynamicLowFlags
    {
        HideModel = 0x01,               // object model is not shown with this flag
        Activate = 0x02,                // enables interaction with GO
        Animate = 0x04,                 // possibly more distinct animation of GO
        NoInteract = 0x08,              // appears to disable interaction (not fully verified)
        Sparkle = 0x10,                 // makes GO sparkle
        Stopped = 0x20                  // transport is stopped
    }

    public enum GameEntityDestructibleState
    {
        Intact = 0,
        Damaged = 1,
        Destroyed = 2,
        Rebuilding = 3
    }
}