namespace Core
{
    public abstract class FactoryHolder<TObj, TKey>
    {
        public TKey Key { get; private set; }

        protected FactoryHolder(TKey k)
        {
            Key = k;
        }

        public void RegisterSelf()
        {
            ObjectRegistry<FactoryHolder<TObj, TKey>, TKey>.Instanse.InsertItem(this, Key);
        }
        public void DeregisterSelf()
        {
            ObjectRegistry<FactoryHolder<TObj, TKey>, TKey>.Instanse.RemoveItem(Key);
        }
        /// Abstract Factory create method
        public abstract TObj Create(object data = null);
    }
}