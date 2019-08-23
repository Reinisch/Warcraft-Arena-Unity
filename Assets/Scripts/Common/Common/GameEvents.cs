namespace Common
{
    public enum GameEvents
    {
        GameMapLoaded,
        DisconnectedFromHost,
        DisconnectedFromMaster,
        DisconnectHandled,
        SessionListUpdated,

        WorldInitialized,
        WorldDeinitializing,

        PlayerControlGained,
        PlayerControlLost,

        SpellLaunched,
        SpellHit,
        SpellDamageDone,
        SpellHealingDone,

        ClientSpellFailed,

        ServerPlayerSpeedChanged,
        ServerPlayerRootChanged,
        ServerSpellLaunch,
        ServerSpellHit,
        ServerSpellCooldown,
        ServerPlayerTeleport,
        ServerDamageDone,
        ServerHealingDone,
        ServerLaunched,

        UnitAttributeChanged,
        UnitTargetChanged,
        UnitFactionChanged,
        UnitModelChanged,

        HotkeyStateChanged,
        GameOptionChanged
    }
}
