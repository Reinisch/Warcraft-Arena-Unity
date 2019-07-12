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

        public static void Insert<TKey, TValue>(this IDictionary<TKey, List<TValue>> dictionary, TKey key, TValue value)
        {
            if (dictionary.TryGetValue(key, out List<TValue> list))
                list.Add(value);
            else
                dictionary[key] = new List<TValue> {value};
        }

        public static void Delete<TKey, TValue>(this IDictionary<TKey, List<TValue>> dictionary, TKey key, TValue value)
        {
            if (dictionary.TryGetValue(key, out List<TValue> list))
            {
                list.Remove(value);
                if (list.Count == 0)
                    dictionary.Remove(key);
            }
        }
    }
}
