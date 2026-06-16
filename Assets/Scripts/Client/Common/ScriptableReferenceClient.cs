using Common;
using Core;
using Zenject;

namespace Client
{
    public abstract class ScriptableReferenceClient : ScriptableReference
    {
        [Inject]
        protected World World { get; private set; }

        public Player Player => World.PlayerManager.Player;

        protected override void OnRegistered()
        {
            World.PlayerManager.EventPlayerChanged += OnControlStateChanged;
        }

        protected override void OnUnregister()
        {
            World.PlayerManager.EventPlayerChanged -= OnControlStateChanged;
        }

        public virtual void OnWorldStateChanged(bool created)
        {
        }

        public virtual void OnControlStateChanged(bool underControl)
        {
        }
    }
}
