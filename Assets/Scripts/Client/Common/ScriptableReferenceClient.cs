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
            EventHandler.RegisterEvent<World, bool>(GameEvents.WorldStateChanged, OnWorldStateChanged);
            EventHandler.RegisterEvent<Player, bool>(GameEvents.ClientControlStateChanged, OnControlStateChanged);
        }

        protected override void OnUnregister()
        {
            EventHandler.UnregisterEvent<Player, bool>(GameEvents.ClientControlStateChanged, OnControlStateChanged);
            EventHandler.UnregisterEvent<World, bool>(GameEvents.WorldStateChanged, OnWorldStateChanged);
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
