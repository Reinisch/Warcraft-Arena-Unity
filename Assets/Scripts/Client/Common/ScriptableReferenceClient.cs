using Common;
using Core;

namespace Client
{
    public abstract class ScriptableReferenceClient : ScriptableReference
    {
        protected World World { get; private set; }
        public Player Player { get; private set; }

        protected override void OnRegistered()
        {
            EventHandler.RegisterEvent<World, bool>(EventHandler.GlobalDispatcher, GameEvents.WorldStateChanged, OnWorldStateChanged);
            EventHandler.RegisterEvent<Player, bool>(EventHandler.GlobalDispatcher, GameEvents.ClientControlStateChanged, OnControlStateChanged);
        }

        protected override void OnUnregister()
        {
            EventHandler.UnregisterEvent<Player, bool>(EventHandler.GlobalDispatcher, GameEvents.ClientControlStateChanged, OnControlStateChanged);
            EventHandler.UnregisterEvent<World, bool>(EventHandler.GlobalDispatcher, GameEvents.WorldStateChanged, OnWorldStateChanged);
        }

        protected virtual void OnWorldStateChanged(World world, bool created)
        {
            World = created ? world : null;
        }

        protected virtual void OnControlStateChanged(Player player, bool underControl)
        {
            Player = underControl ? player : null;
        }
    }
}
