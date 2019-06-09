using System.Collections.Generic;

namespace Common
{
    public static class CommonUtils
    {
        public static TValue LookupEntry<TKey, TValue>(this IDictionary<TKey, TValue> dict, TKey key)
        {
            dict.TryGetValue(key, out var val);
            return val;
        }
    }
}
