using System;
using System.Collections.Concurrent;
using System.Data.Common;
using System.Reflection;
using ElleFramework.Utils;
using ElleFramework.Utils.Utility;

namespace ElleFramework.Database
{
    //DAO = Data Access Object
    public class DAOFactory
	{
		private static readonly ConcurrentDictionary<Type, object>	instances = new ConcurrentDictionary<Type, object>();

        public static string										AssemblyInstanceName { get; set; }
	    public static Config conf { get; set; }

		static DAOFactory()
		{
		}

        /// <summary>
        ///		<param>
        ///			Look for a DAO that implements the required T interface (or descends from the T class).
        ///		</param>
        ///		<param>
        ///			If a DAO of the same type has already been created, an existing instance is returned.
        ///			It is important that the objects created by this class are all thread-safe.
        ///		</param>
        /// </summary>
        /// <typeparam name="T">interface or class that the DAO must implement</typeparam>
        /// <returns>the DAO of the requested type, or null if it fails to create it</returns>
        public static T Create<T>()
		{
			Type	iface = typeof( T );
			object	ret;

            conf = ApplicationUtils.Configuration;

            if ( !instances.TryGetValue( iface, out ret )) {

				if( string.IsNullOrEmpty( AssemblyInstanceName ))
					AssemblyInstanceName = conf.Database.DAOAssembly;

				if( ret == null ) {
                    // if I don't find it "easily" and directly, try to find an object that implements the interface
                    Assembly asm = Assembly.Load( new AssemblyName( AssemblyInstanceName ));
					Type[]		types = asm.GetExportedTypes();

					foreach( Type type in types )
						if( type.IsClass && iface.IsAssignableFrom( type )) {
							
							ret = Activator.CreateInstance( type );

							break;
						}
				}

				instances[ iface ] = ret;
			}

			return (T)ret;
		}

        /// <summary>
        /// Clear the instance cache.
        /// 
        /// Useful if the application wants to switch from one DB to another.
        /// </summary>
        public static void Clear()
		{
			AssemblyInstanceName = null;

			instances.Clear();
		}
	}
}
