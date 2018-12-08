using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Core
{
    public static class DataExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TValue LookupEntry<TKey, TValue>(this Dictionary<TKey, TValue> dict, TKey key)
        {
            TValue val;
            dict.TryGetValue(key, out val);
            return val;
        }
    }
}