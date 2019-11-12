using Common;
using Core;

namespace Client
{
    public abstract class ScriptableReferenceClient : ScriptableReference
    {
        protected WorldManager World { get; private set; }
        public Player Player { get; private set; }

        protected override void OnRegistered()
        {
            EventHandler.RegisterEvent<WorldManager>(EventHandler.GlobalDispatcher, GameEvents.WorldInitialized, OnWorldInitialized);
            EventHandler.RegisterEvent<WorldManager>(EventHandler.GlobalDispatcher, GameEvents.WorldDeinitializing, OnWorldDeinitializing);
            EventHandler.RegisterEvent<Player>(EventHandler.GlobalDispatcher, GameEvents.PlayerControlGained, OnPlayerControlGained);
            EventHandler.RegisterEvent<Player>(EventHandler.GlobalDispatcher, GameEvents.PlayerControlLost, OnPlayerControlLost);
        }

        protected override void OnUnregister()
        {
            EventHandler.UnregisterEvent<Player>(EventHandler.GlobalDispatcher, GameEvents.PlayerControlGained, OnPlayerControlGained);
            EventHandler.UnregisterEvent<Player>(EventHandler.GlobalDispatcher, GameEvents.PlayerControlLost, OnPlayerControlLost);
            EventHandler.UnregisterEvent<WorldManager>(EventHandler.GlobalDispatcher, GameEvents.WorldInitialized, OnWorldInitialized);
            EventHandler.UnregisterEvent<WorldManager>(EventHandler.GlobalDispatcher, GameEvents.WorldDeinitializing, OnWorldDeinitializing);
        }

        protected virtual void OnWorldInitialized(WorldManager world)
        {
            World = world;
        }

        protected virtual void OnWorldDeinitializing(WorldManager world)
        {
            World = null;
        }

        protected virtual void OnPlayerControlGained(Player player)
        {
            Player = player;
        }

        protected virtual void OnPlayerControlLost(Player player)
        {
            Player = null;
        }
    }
}
