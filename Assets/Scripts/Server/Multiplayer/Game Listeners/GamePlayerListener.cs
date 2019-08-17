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
            EventHandler.RegisterEvent<Player, bool>(EventHandler.GlobalDispatcher, GameEvents.ServerPlayerRootChanged, OnPlayerRootChanged);
        }

        internal override void Dispose()
        {
            EventHandler.UnregisterEvent<Player, UnitMoveType, float>(EventHandler.GlobalDispatcher, GameEvents.ServerPlayerSpeedChanged, OnPlayerSpeedChanged);
            EventHandler.UnregisterEvent<Player, bool>(EventHandler.GlobalDispatcher, GameEvents.ServerPlayerRootChanged, OnPlayerRootChanged);
        }

        private void OnPlayerSpeedChanged(Player player, UnitMoveType moveType, float rate)
        {
            if (player.BoltEntity.Controller != null)
            {
                PlayerSpeedRateChangedEvent speedChangeEvent = PlayerSpeedRateChangedEvent.Create(player.BoltEntity.Controller, ReliabilityModes.ReliableOrdered);
                speedChangeEvent.MoveType = (int) moveType;
                speedChangeEvent.SpeedRate = rate;
                speedChangeEvent.Send();
            }
        }

        private void OnPlayerRootChanged(Player player, bool applied)
        {
            if (player.BoltEntity.Controller != null)
            {
                PlayerRootChangedEvent rootChangedEvent = PlayerRootChangedEvent.Create(player.BoltEntity.Controller, ReliabilityModes.ReliableOrdered);
                rootChangedEvent.Applied = applied;
                rootChangedEvent.Send();
            }
        }
    }
}
