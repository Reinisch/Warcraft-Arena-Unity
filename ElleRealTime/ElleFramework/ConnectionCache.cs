using System.Collections.Concurrent;

namespace ElleFramework.Database
{
    class ConnectionCache
    {
        private static readonly ConcurrentDictionary<string, ConnectionQueue> connections = new ConcurrentDictionary<string,ConnectionQueue>();

        internal static ConnectionQueue GetConnectionQueue( string connectionString, int poolSize )
        {
            ConnectionQueue ret;

            if( !connections.TryGetValue( connectionString, out ret )) {
                ret                             = new ConnectionQueue( poolSize );
                connections[ connectionString ] = ret;
            }

            return ret;
        }
    }
}
