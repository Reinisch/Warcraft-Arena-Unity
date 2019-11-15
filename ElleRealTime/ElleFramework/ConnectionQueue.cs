using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data.Common;
using ElleFramework.Database.Entities;
using ElleFramework.Utils;


namespace ElleFramework.Database
{
    class ConnectionQueue
    {
        private const int											CONNECTION_TEST_PERIOD = 60;

        private readonly Queue<QueuedConnection>					queue = new Queue<QueuedConnection>();
        private readonly int										poolSize;
        private readonly ConcurrentDictionary<DbConnection, byte>	connectionsToDispose = new ConcurrentDictionary<DbConnection, byte>();

        public ConnectionQueue( int poolSize )
        {
            this.poolSize = poolSize;
        }

        public int GetQueueCount()
        {
            int ret;

            lock( queue )
                ret = queue.Count;

            return ret;
        }

        public DbConnection Dequeue()
        {
            DbConnection ret = null;

            lock( queue ) {

                while(( queue.Count > 0 ) && ( ret == null )) {
                    QueuedConnection	conn = queue.Dequeue();

                    ret = conn.Connection;

                    if(( DateTime.UtcNow - conn.LastUsedTime ).TotalSeconds >= CONNECTION_TEST_PERIOD ) {

                        try {
                            ret.GetSchema( "Tables" ).Dispose();
                        }
                        catch( Exception ex ) {

                            ret = null;

                            Tracer.TraceAppLog( "I got an exception during aquiring a pool's connection.", ex );
                        }
                    }
                }
            }

            return( ret );
        }

        public void Enqueue(DbConnection conn )
        {
            byte	dummy;
            bool	dispose = connectionsToDispose.TryRemove( conn, out dummy );

            lock( queue ) {

                if( !dispose && ( queue.Count < poolSize )) {

                    queue.Enqueue( new QueuedConnection
                                   {
                                       Connection = conn,
                                       LastUsedTime = DateTime.UtcNow
                                   } );

                } else
                    dispose = true;
            }

            if( dispose )
                ElleBaseDAO.DisposeConnection( conn );
        }

        public void DiscardConnection(DbConnection conn )
        {
            if( conn != null )
                connectionsToDispose.TryAdd( conn, 1 );
        }
    }
}
