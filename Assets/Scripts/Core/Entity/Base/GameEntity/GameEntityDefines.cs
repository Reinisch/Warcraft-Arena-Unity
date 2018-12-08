namespace Core
{
    public enum GoState
    {
        Active = 0,
        Ready = 1,
        ActiveAlternative = 2,
        TransportActive = 24,
        TransportStopped = 25
    }

    public enum LootState
    {
        NotReady = 0,
        Ready,
        Activated,
        JustDeactivated
    }

    public enum GameEntityFlags
    {
        InUse = 0x00000001,
        Locked = 0x00000002,
        InteractCondition = 0x00000004,
        Transport = 0x00000008,
        NotSelectable = 0x00000010,
        NoDespawn = 0x00000020,
        FreezeAnimation = 0x00000080,
        Damaged = 0x00000200,
        Destroyed = 0x00000400,
    }

    public enum GameEntityDestructibleState
    {
        Intact = 0,
        Damaged = 1,
        Destroyed = 2,
        Rebuilding = 3
    }
}