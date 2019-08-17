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

        ClientSpellFailed,

        ServerPlayerSpeedChanged,
        ServerPlayerRootChanged,
        ServerSpellLaunch,
        ServerSpellHit,
        ServerSpellCooldown,
        ServerPlayerTeleport,
        ServerDamageDone,
        ServerLaunched,

        UnitAttributeChanged,
        UnitTargetChanged,
        UnitFactionChanged,

        HotkeyStateChanged,
        GameOptionChanged
    }
}
