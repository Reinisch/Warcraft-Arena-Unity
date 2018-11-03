using System.Collections.Generic;

namespace Core
{
    public static class DataExtensions
    {
        public static TValue LookupEntry<TKey, TValue>(this Dictionary<TKey, TValue> dict, TKey key)
        {
            TValue val;
            dict.TryGetValue(key, out val);
            return val;
        }
    }
}