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

        ServerMapLoaded,
        ServerVisibilityChanged,
        ServerPlayerSpeedChanged,
        ServerUnitTeleported,
        ServerPlayerMovementControlChanged,

        UnitChat,
        SystemMessage,
        UnitAttributeChanged,
        UnitTargetChanged,
        UnitFactionChanged,
        UnitClassChanged,
        UnitVisualsChanged,
        UnitModelChanged,
        UnitModelAttached,
        UnitScaleChanged,
        UnitDisplayPowerChanged,

        HotkeyStateChanged,
        HotkeyBindingChanged,
        LobbyClassChanged,
        GameOptionChanged,
        EntityPooled,

        ServerArenaStateChanged,
        ServerArenaMatchEnded,

        SessionLeaveRequested
    }
}
