namespace Common
{
    public enum GameEvents
    {
        GameMapLoaded,
        DisconnectedFromHost,
        DisconnectedFromMaster,
        SessionListUpdated,
        WorldStateChanged,

        ClientControlStateChanged,
        ClientSpellFailed,

        SpellLaunched,
        SpellHit,
        SpellDamageDone,
        SpellHealingDone,
        SpellMissDone,

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
        UnitDisplayPowerChanged,

        HotkeyStateChanged,
        HotkeyBindingChanged,
        LobbyClassChanged,
        GameOptionChanged,
        EntityPooled
    }
}
