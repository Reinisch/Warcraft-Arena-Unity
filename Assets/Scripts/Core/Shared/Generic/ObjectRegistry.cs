using System.Collections.Generic;
using System.Linq;

namespace Core
{
    public class ObjectRegistry<TItem, TKey> where TItem : class
    {
        private static ObjectRegistry<TItem, TKey> instanse = new ObjectRegistry<TItem, TKey>();
        private readonly Dictionary<TKey, TItem> registeredObjects = new Dictionary<TKey, TItem>();

        private ObjectRegistry()
        {
        
        }

        public static ObjectRegistry<TItem, TKey> Instanse => instanse ?? (instanse = new ObjectRegistry<TItem, TKey>());

        /// Returns a registry item
        public TItem GetRegistryItem(TKey key)
        {
            return registeredObjects.ContainsKey(key) ? registeredObjects[key] : null;
        }
        /// Inefficiently return a vector of registered items
        public List<TKey> GetRegisteredItems()
        {
            return registeredObjects.Keys.ToList();
        }

        /// Inserts a registry item
        public bool InsertItem(TItem obj, TKey key, bool replace = false)
        {
            if (registeredObjects.ContainsKey(key))
            {
                if (!replace)
                    return false;

                registeredObjects[key] = obj;
                return true;
            }

            registeredObjects.Add(key, obj);
            return true;
        }
        /// Removes a registry item
        public void RemoveItem(TKey key)
        {
            registeredObjects.Remove(key);
        }
        /// Returns true if registry contains an item
        public bool HasItem(TKey key)
        {
            return registeredObjects.ContainsKey(key);
        }
    }
}