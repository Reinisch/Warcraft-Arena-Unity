using Bolt;
using Common;
using Core;

namespace Server
{
    internal class GamePlayerListener : BaseGameListener
    {
        internal GamePlayerListener(WorldServerManager worldServerManager) : base(worldServerManager)
        {
            EventHandler.RegisterEvent<Player, UnitMoveType, float>(EventHandler.GlobalDispatcher, GameEvents.ServerPlayerSpeedChanged, OnPlayerSpeedChanged);
        }

        internal override void Dispose()
        {
            EventHandler.RegisterEvent<Player, UnitMoveType, float>(EventHandler.GlobalDispatcher, GameEvents.ServerPlayerSpeedChanged, OnPlayerSpeedChanged);
        }

        private void OnPlayerSpeedChanged(Player player, UnitMoveType moveType, float rate)
        {
            if (player.BoltEntity.Controller != null)
            {
                PlayerSpeedRateChangedEvent spellTeleportEvent = PlayerSpeedRateChangedEvent.Create(player.BoltEntity.Controller, ReliabilityModes.ReliableOrdered);
                spellTeleportEvent.MoveType = (int) moveType;
                spellTeleportEvent.SpeedRate = rate;
                spellTeleportEvent.Send();
            }
        }
    }
}
