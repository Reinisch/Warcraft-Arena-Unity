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

        SpellLaunched,
        SpellHit,
        SpellDamageDone,

        ServerSpellLaunch,
        ServerSpellHit,
        ServerLaunched,

        UnitAttributeChanged,
        UnitTargetChanged,
        UnitFactionChanged,

        HotkeyPressed
    }
}
