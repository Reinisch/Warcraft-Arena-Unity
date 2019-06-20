namespace Common
{
    public enum GameEvents
    {
        GameMapLoaded,
        DisconnectedFromHost,
        DisconnectedFromMaster,
        SessionListUpdated,

        WorldInitialized,
        WorldDeinitializing,

        PlayerControlGained,
        PlayerControlLost,

        SpellCasted,
        SpellHit,
        SpellDamageDone,

        ServerSpellCast,

        EntityAttributeChanged,

        HotkeyPressed
    }
}
