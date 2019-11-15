using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data.Common;
using JetBrains.Annotations;

namespace ElleFramework.Database.MVC
{
    public class ViewFactory<T> where T : View, new()
    {
        [CanBeNull]
        public static T Read( DbDataReader dr )
        {
            return( dr.Read() ? Create( dr ) : null );
        }

        [NotNull]
        public static T Create( DbDataReader dr )
        {
            T ret = new T();

            ret.Load( dr );

            return ret;
        }

        [NotNull]
        public static T Create( NameValueCollection values )
        {
            T ret = new T();

            ret.Load( values );

            return ret;
        }

        [NotNull]
        public static T Create( Hashtable values )
        {
            T ret = new T();

            ret.Load( values );

            return ret;
        }

        [NotNull]
        public static T Create( IDictionary<string, object> values )
        {
            T ret = new T();

            ret.Load( values );

            return ret;
        }

        [NotNull]
        public static T[] GetArray( DbDataReader dr )
        {
            List<T> ret = new List<T>();

            while( dr.Read() )
                ret.Add( Create( dr ));

            return ret.ToArray();
        }
    }
}
