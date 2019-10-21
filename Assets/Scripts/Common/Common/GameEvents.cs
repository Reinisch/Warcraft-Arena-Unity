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
        SpellHealingDone,
        SpellMissDone,

        ClientSpellFailed,

        ServerPlayerSpeedChanged,
        ServerPlayerRootChanged,
        ServerPlayerMovementControlChanged,
        ServerSpellLaunch,
        ServerSpellHit,
        ServerSpellCooldown,
        ServerPlayerTeleport,
        ServerDamageDone,
        ServerHealingDone,
        ServerLaunched,

        UnitChat,
        UnitAttributeChanged,
        UnitTargetChanged,
        UnitFactionChanged,
        UnitModelChanged,
        UnitScaleChanged,

        HotkeyStateChanged,
        HotkeyBindingChanged,
        GameOptionChanged
    }
}
