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
        ServerSpellLaunch,
        ServerSpellHit,
        ServerSpellCooldown,
        ServerPlayerTeleport,
        ServerLaunched,

        UnitAttributeChanged,
        UnitTargetChanged,
        UnitFactionChanged,

        HotkeyStateChanged,
        GameOptionChanged
    }
}
