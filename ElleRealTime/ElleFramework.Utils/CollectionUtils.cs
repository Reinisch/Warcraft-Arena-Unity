using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace ElleFramework.Utils
{
    public class CollectionUtils
    {
        public static bool AreEquals( byte[] a1, byte[] a2 )
		{
			bool ret = true;

			if( a1.Length != a2.Length )
				ret = false;
			else {

				for( int i = 0; ( i < a1.Length ) && ret; i++ )
					ret = a1[ i ] == a2[ i ];
			}

			return ret;
		}

		public static Dictionary<TKey, List<TValue>> GroupListBy<TKey, TValue>( IEnumerable<TValue> items, Func<TValue, TKey> keyFunc )
		{
			Dictionary<TKey, List<TValue>>	ret = new Dictionary<TKey, List<TValue>>();

			if( items != null )
				foreach( TValue item in items ) {
					TKey			key = keyFunc( item );
					List<TValue>	list;

					if( !ret.TryGetValue( key, out list )) {
						list       = new List<TValue>();
						ret[ key ] = list;
					}

					list.Add( item );
				}

			return( ret );
		}

		public static Dictionary<TKey, TValue[]> GroupBy<TKey, TValue>( IEnumerable<TValue> items, Func<TValue, TKey> keyFunc )
		{
			Dictionary<TKey, TValue[]>	ret = new Dictionary<TKey, TValue[]>();

			foreach( KeyValuePair<TKey, List<TValue>> pair in GroupListBy( items, keyFunc ))
				ret[ pair.Key ] = pair.Value.ToArray();

			return( ret );
		}

		public static Dictionary<TKey, TValue> IndexBy<TKey, TValue>( IEnumerable<TValue> items, Func<TValue, TKey> keyFunc )
		{
			Dictionary<TKey, TValue>	ret = new Dictionary<TKey, TValue>();

			IndexBy( items, keyFunc, ret );

			return( ret );
		}

		public static void IndexBy<TKey, TValue>( IEnumerable<TValue> items, Func<TValue, TKey> keyFunc, Dictionary<TKey, TValue> result )
		{
			if( items != null )
				foreach( TValue item in items )
					result[ keyFunc( item ) ] = item;
		}

		public static Dictionary<K, V> Merge<K, V>( params Dictionary<K, V>[] dictionaries )
		{
			Dictionary<K, V>	ret = new Dictionary<K, V>();

			foreach( Dictionary<K, V> dict in dictionaries )
				foreach( KeyValuePair<K, V> pair in dict )
					ret[ pair.Key ] = pair.Value;

			return ret;
		}

		public static V GetOrSet<K, V>( IDictionary<K, V> dictionary, K key, Func<V> notFoundFunc )
		{
			V	ret;

			if( !dictionary.TryGetValue( key, out ret )) {
				ret               = notFoundFunc();
				dictionary[ key ] = ret;
			}

			return ret;
		}

        public static Dictionary<string, object> HashtableToDictionary(Hashtable prms)
        {
            Dictionary<string, object> ret = new Dictionary<string, object>();

            int currNum = -1;
            int numToReach = prms.Count;
            List<string> usedKeys = new List<string>();
            do
            {
                ArrayList list = new ArrayList(prms.Keys);
                string[] keys = (string[])list.ToArray(typeof(string));
                var foundKeys = keys.Where(x => x.StartsWith($"@{currNum + 1}_")).ToArray();
                if (foundKeys != null && foundKeys.Length > 0)
                {
                    string key = foundKeys[0];
                    ret.Add(key, prms[key]);
                    
                }
                currNum = currNum + 1;

            } while (currNum < numToReach);

            return ret;
        }

        private static string Between(string STR, string FirstString, string LastString)
        {
            string FinalString;
            int Pos1 = STR.IndexOf(FirstString) + FirstString.Length;
            int Pos2 = STR.IndexOf(LastString);
            FinalString = STR.Substring(Pos1, Pos2 - Pos1);
            return FinalString;
        }
    }
}
