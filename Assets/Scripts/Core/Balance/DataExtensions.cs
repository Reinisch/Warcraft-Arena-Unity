using System.Collections.Generic;

namespace Core
{
    public static class DataExtensions
    {
        public static TValue LookupEntry<TKey, TValue>(this IDictionary<TKey, TValue> dict, TKey key)
        {
            dict.TryGetValue(key, out var val);
            return val; 
        }
    }
}