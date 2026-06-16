namespace Client.UI
{
    public abstract class Presenter<TView>
    {
        protected TView View { get; private set; }

        public virtual void Initialize(TView view) => View = view;

        public virtual void Deinitialize() => View = default;

        public virtual void Tick(float deltaTime)
        {
        }
    }
}
