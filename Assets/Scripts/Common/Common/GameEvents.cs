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
        ServerMapLoaded,
        ServerLaunched,

        UnitChat,
        UnitAttributeChanged,
        UnitTargetChanged,
        UnitFactionChanged,
        UnitClassChanged,
        UnitModelChanged,
        UnitScaleChanged,

        HotkeyStateChanged,
        HotkeyBindingChanged,
        GameOptionChanged
    }
}
