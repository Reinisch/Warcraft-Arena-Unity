namespace Core
{
    /// <summary>
    /// Compressed to 3 bits in UnitState.
    /// </summary>
    public enum DeathState
    {
        Alive = 0,
        JustDied = 1,
        Corpse = 2,
        Dead = 3,
        JustRespawned = 4
    }
}
