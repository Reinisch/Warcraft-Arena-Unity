using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Diagnostics;
using System.Text;
using ElleFramework.Database.MVC;
using ElleFramework.Utils;
using ElleFramework.Utils.Utility;
using JetBrains.Annotations;

namespace ElleFramework.Database
{
    public class ElleBaseDAO
    {
        //Do not remove or rename because from the outside it is possible to understand which configuration items can be set without having to go to open the code or remember them
        public const string APPSETTINGS_DBConnectionPoolSize = "DBConnectionPoolSize";
        public const string APPSETTINGS_DBSlowQueryLog = "DBSlowQueryLog";
        public const string APPSETTINGS_DBTraceQueries = "DBTraceQueries";
        public const string APPSETTINGS_DBTraceExceptions = "DBTraceExceptions";

        protected string													connectionString;
        protected DbProviderFactory                                         providerFactory;
        protected int														connPoolSize = 1;
        protected int														slowQueryLog;
        private int															cmdTimeout;
        private ConnectionQueue                                             connections;
        private readonly ConcurrentDictionary<DbDataReader, DbConnection>	connectionsToClose = new ConcurrentDictionary<DbDataReader, DbConnection>();
        [ThreadStatic]
        private static string												lastCommand;

        public static bool TraceQueries { get; set; }
        public static bool TraceExceptions { get; set; }

        private Config conf { get; set; }

        /*INIZIO*/
        public ElleBaseDAO() : this( null, null )
        {
        }

        public ElleBaseDAO( string connectionName )
        {
            SetConnectionString( connectionName );
        }

        public ElleBaseDAO( string providerFactory, string connectionString ) 
        {
            Setup( providerFactory, connectionString );
        }

        public ElleBaseDAO( string connectionString, int poolSize ) 
        {
            Setup( connectionString );

            connPoolSize = poolSize;
        }

        public void SetConnectionString( string connectionString )
        {
            Setup( connectionString );
        }

        private void Setup(string provider, string connString)
        {
            conf = ApplicationUtils.Configuration;
            connPoolSize = conf.Database.DbConnectionPoolSize;
            cmdTimeout = conf.Database.DBCmdTimeout;

            TraceQueries = conf.Database.DbTraceQueries == 1;
            TraceExceptions = conf.Logging.TraceExceptions == 1;

            if (!string.IsNullOrWhiteSpace(provider))
            {
                providerFactory = DbProviderFactories.GetFactory(provider);
            }

            if (!string.IsNullOrWhiteSpace(connString))
            {
                connectionString = connString;
                connections = ConnectionCache.GetConnectionQueue(connectionString, connPoolSize);
            }
        }

        private void Setup( string connString )
        {

            if( !string.IsNullOrWhiteSpace( connString )) {
                connectionString = connString;
                connections      = ConnectionCache.GetConnectionQueue( connectionString, connPoolSize );
            }
        }

        /// <summary>
        /// Allows subclasses to manage the connection in case of exceptions during queries.
        /// </summary>
        /// <param name="ex">exception</param>
        /// <param name="connection">connection in use at the moment of the exception</param>
        protected virtual void TraceException(Exception ex, DbConnection connection)
        {
            TraceException(ex);
        }

        public virtual void NewExceptionHandler(Exception ex)
        {
            throw ex;
        }

        protected void AddParam( DbCommand cmd, string column, DbType type, int size )
        {
            DbParameter param = providerFactory.CreateParameter();

            param.ParameterName = column;
            param.DbType        = type;
            param.Size          = size;
            param.SourceColumn  = column;

            cmd.Parameters.Add( param );
        }

        private void AddParam(DbCommand cmd, string name, object value)
        {
            DbParameter dbParam = providerFactory.CreateParameter();

            if (dbParam != null)
            {

                dbParam.ParameterName = name;
                dbParam.Value = value;

                cmd.Parameters.Add(dbParam);
            }
        }

        protected static bool isDbNull( DbDataReader dr, string columnName )
        {
            object val = dr[ columnName ];

            return ( val == DBNull.Value ) || ( val == null );
        }

        public static bool HasColumn( DbDataReader dr, string columnName )
		{
			bool ret = false;

    		using( DataTable dt = dr.GetSchemaTable() )
    			if( dt != null ) {
    				int i = 0, num = dt.Rows.Count;

    				while( !ret && ( i < num )) {
    					DataRow row = dt.Rows[ i++ ];

    					ret = row[ 0 ].ToString().Equals( columnName, StringComparison.InvariantCultureIgnoreCase );
    				}
    			}

    		return ret;
		}

		public static bool ToBoolNullable( DbDataReader dr, string colName, bool nullValue )
		{
			bool ret;

			if( isDbNull( dr, colName ))
				ret = nullValue;
			else
				ret = Convert.ToBoolean( dr[ colName ] );

			return ret;
		}

		public static byte ToByteNullable( DbDataReader dr, string colName, byte nullValue )
		{
			byte ret;

			if( isDbNull( dr, colName ))
				ret = nullValue;
			else
				ret = Convert.ToByte( dr[ colName ] );

			return ret;
		}

		public static short ToShortNullable( DbDataReader dr, string colName, short nullValue )
		{
			short ret;

			if( isDbNull( dr, colName ))
				ret = nullValue;
			else
				ret = Convert.ToInt16( dr[ colName ] );

			return ret;
		}

		public static int ToIntNullable( DbDataReader dr, string colName, int nullValue )
		{
			int ret;

			if( isDbNull( dr, colName ))
				ret = nullValue;
			else
				ret = Convert.ToInt32( dr[ colName ] );

			return ret;
		}

		public static long ToLongNullable( DbDataReader dr, string colName, long nullValue )
		{
			long ret;

			if( isDbNull( dr, colName ))
				ret = nullValue;
			else
				ret = Convert.ToInt64( dr[ colName ] );

			return ret;
		}

		public static DateTime ToDateTimeNullable( DbDataReader dr, string colName, DateTime nullValue )
		{
			DateTime ret;

			if( isDbNull( dr, colName ))
				ret = nullValue;
			else
				ret = Convert.ToDateTime( dr[ colName ] );

			return ret;
		}

		public static string ToStringNullable( DbDataReader dr, string colName, string nullValue )
		{
			string ret;

			if( isDbNull( dr, colName ))
				ret = nullValue;
			else
				ret = Convert.ToString( dr[ colName ] );

			return ret;
		}

		public static decimal ToDecimalNullable( DbDataReader dr, string colName, decimal nullValue )
		{
			decimal ret;

			if( isDbNull( dr, colName ))
				ret = nullValue;
			else
				ret = Convert.ToDecimal( dr[ colName ] );

			return ret;
		}

		public static object ToObjNullable( DbDataReader dr, string colName, object nullValue )
		{
			object ret;

			if( isDbNull( dr, colName ))
				ret = nullValue;
			else
				ret = dr[ colName ];

			return ret;
		}


        public static object GetNullableValue( object value, object nullValue )
        {
            if(( value == null ) || value.Equals( nullValue ))
                value = DBNull.Value;
            else {
                try {
                    if( value.Equals( Convert.ChangeType( nullValue, value.GetType() )))
                        value = DBNull.Value;
                }
                catch {
                }
            }

            return value;
        }

        protected virtual DbConnection GetConnection()
        {
            DbConnection ret = null;
            int				maxIterations = connections.GetQueueCount() + 1;

            do {

                try {
                    ret = connections.Dequeue();
					
                    if( ret == null )
                    {

                        ret = providerFactory.CreateConnection();

                        if( ret == null )
                            throw new Exception( "I didn't get a database connection." );

                        ret.ConnectionString = connectionString;
                    }

                    if( ret.State == ConnectionState.Broken )
                        ret.Close();

                    if( ret.State != ConnectionState.Open )
                        ret.Open();
                }
                catch( Exception ex ) {

                    DisposeConnection( ret );

                    if( --maxIterations > 0 )
                        ret = null;
                    else {
                        TraceException( ex );
                        throw;
                    }
                }

            } while( ret == null );

            return ret;
        }

        internal static void DisposeConnection(DbConnection conn )
        {
            if( conn != null ) {
				
                try {
                    conn.Close();
                }
                catch {
                }

                try {
                    conn.Dispose();
                }
                catch {
                }
            }
        }

        protected virtual void CloseConnection( DbConnection conn )
        {
            if( conn != null ) {

                if( conn.State == ConnectionState.Open )
                    connections.Enqueue( conn );
                else
                    DisposeConnection( conn );
            }
        }

        protected void DiscardConnection(DbConnection conn )
        {
            connections.DiscardConnection( conn );
        }

        protected virtual DbCommand GetCommand( string query )
        {
            DbCommand cmd = providerFactory.CreateCommand();

            if( cmd != null ) {

                cmd.CommandText = query;

                if( cmdTimeout >= 0 )
                    cmd.CommandTimeout = cmdTimeout;
            }

            return cmd;
        }

        protected virtual DbCommand GetCommand(string query, Hashtable prms)
        {
            DbCommand cmd = GetCommand(query);

            foreach (DictionaryEntry prm in prms)
            {
                AddParam(cmd, prm.Key.ToString(), prm.Value);
            }

            return cmd;
        }

        protected virtual DbCommand GetCommand(string query, Dictionary<string, object> prms)
        {
            DbCommand cmd = GetCommand(query);

            foreach (var key in prms.Keys)
            {
                AddParam(cmd, key, prms[key]);
            }

            return cmd;
        }

        // For compatibility with the existing code that overrides GetCommand (string, Hashtable) I can't
        // call that method with the second null parameter as I do in this class 
        private DbCommand GetCommandPrivate(string query, Hashtable prms)
        {
            return (prms != null) ? GetCommand(query, prms) : GetCommand(query);
        }

        private DbCommand GetCommandPrivate(string query, Dictionary<string, object> prms)
        {
            return (prms != null) ? GetCommand(query, prms) : GetCommand(query);
        }

        protected virtual DbCommand GetCommand( string query, Array prms )
        {
            DbCommand cmd = GetCommand( query );
            int			count = 0;

            foreach( object prm in prms ) {
                AddParam( cmd, "P" + count++, prm );
            }

            return cmd;
        }

        public DbTransaction BeginTransaction()
		{
			return BeginTransaction( IsolationLevel.ReadCommitted );
		}

		public virtual DbTransaction BeginTransaction( IsolationLevel level )
		{
		    DbConnection conn = GetConnection();

			return conn.BeginTransaction( level );
		}

		public void CommitTransaction( ref DbTransaction trans )
		{
			CommitTransaction( ref trans, true );
		}

		public void CommitTransaction( ref DbTransaction trans, bool closeConnection )
		{
			if( trans != null ) {
				DbConnection conn = trans.Connection;

				trans.Commit();

				if( closeConnection )
					CloseConnection( conn );

                trans.Dispose();

				trans = null;
			}
		}

		public void RollbackTransaction( ref DbTransaction trans )
		{
			RollbackTransaction( ref trans, true );
		}

		public void RollbackTransaction( ref DbTransaction trans, bool closeConnection )
		{
			DbConnection	conn = null;

			try {
				if( trans != null ) {

					conn = trans.Connection;

					trans.Rollback();
                    trans.Dispose();

					trans = null;
				}
			}
			catch( Exception ex ) {
				TraceException( ex );
			}
			finally {
				if( closeConnection )
					CloseConnection( conn );
			}
		}

		private object ExecuteScalar( DbCommand cmd, DbTransaction trans )
		{
			bool	newConn = false;
			object	ret;

			try {
				DateTime start;

				if( trans != null ) {
					cmd.Connection  = trans.Connection;
					cmd.Transaction = trans;
				} else if( cmd.Connection == null ) {
					cmd.Connection = GetConnection();
					newConn        = true;
				}

				SetLastCommand( cmd );

				start = DateTime.UtcNow;
				ret   = cmd.ExecuteScalar();

				CheckSlowQuery( cmd, start );
			}
			catch( Exception ex ) {
				TraceException( ex, cmd.Connection );
				throw;
			}
			finally {
				if( newConn ) {
					CloseConnection( cmd.Connection );
					cmd.Connection = null;
				}
			}

			return ret;
		}

		public object ExecuteScalar( string query )
		{
			return ExecuteScalar( query, (DbTransaction) null );
		}

		public object ExecuteScalar( string query, DbTransaction trans )
		{
			return ExecuteScalar( query, (Hashtable)null, trans );
		}

		public object ExecuteScalar( string query, Hashtable prms )
		{
			return ExecuteScalar( query, prms, null );
		}

        public object ExecuteScalar(string query, Hashtable prms, DbTransaction trans)
        {
            object rval = null;

            try
            {
                using (DbCommand cmd = GetCommandPrivate(query, prms))
                    rval = ExecuteScalar(cmd, trans);
            }
            catch (Exception e)
            {
                NewExceptionHandler(e);
            }

            return rval;
        }

        public object ExecuteScalar(string query, Dictionary<string, object> prms, DbTransaction trans)
        {
            object rval = null;

            try
            {
                using (DbCommand cmd = GetCommandPrivate(query, prms))
                    rval = ExecuteScalar(cmd, trans);
            }
            catch (Exception e)
            {
                NewExceptionHandler(e);
            }

            return rval;
        }

        public object ExecuteScalar( string query, View view )
		{
			return ExecuteScalar( query, view, null );
		}

		public object ExecuteScalar( string query, View view, DbTransaction trans )
		{
			return ExecuteScalar( query, new Hashtable(), view, trans );
		}

		public object ExecuteScalar( string query, Hashtable prms, View view, DbTransaction trans )
		{
			view.SetQueryParams( query, prms );

			return ExecuteScalar( query, prms, trans );
		}

		public object ExecuteScalar( string query, Array prms )
		{
			return ExecuteScalar( query, prms, null );
		}

		public object ExecuteScalar( string query, Array prms, DbTransaction trans )
		{
			object rval = null;

			try {
				using( DbCommand cmd = GetCommand( query, prms ))
					rval = ExecuteScalar( cmd, trans );
			}
			catch( Exception e ) {
				NewExceptionHandler( e );
			}

			return rval;
		}

		public T ExecuteScalar<T>( string query, DbTransaction trans = null )
		{
			return ExecuteScalar<T>( query, new Hashtable(), trans );
		}

		public T ExecuteScalar<T>( string query, Hashtable prms )
		{
			return ExecuteScalar<T>( query, prms, null );
		}

        public T ExecuteScalar<T>(string query, Hashtable prms, DbTransaction trans)
        {
            object obj = ExecuteScalar(query, prms, trans);
            Type type = typeof(T);
            T ret;

            if (type.IsEnum)
                type = Enum.GetUnderlyingType(type);
            else if (type.IsGenericType && (type.GetGenericTypeDefinition() == typeof(Nullable<>)))
                type = Nullable.GetUnderlyingType(type);

            if ((obj != null) && (obj != DBNull.Value))
                ret = (T)Convert.ChangeType(obj, type);
            else
                ret = default(T);

            return ret;
        }

        public T ExecuteScalar<T>(string query, Dictionary<string, object> prms, DbTransaction trans)
        {
            object obj = ExecuteScalar(query, prms, trans);
            Type type = typeof(T);
            T ret;

            if (type.IsEnum)
                type = Enum.GetUnderlyingType(type);
            else if (type.IsGenericType && (type.GetGenericTypeDefinition() == typeof(Nullable<>)))
                type = Nullable.GetUnderlyingType(type);

            if ((obj != null) && (obj != DBNull.Value))
                ret = (T)Convert.ChangeType(obj, type);
            else
                ret = default(T);

            return ret;
        }

        public T ExecuteScalar<T>( string query, View view, DbTransaction trans = null )
		{
			return ExecuteScalar<T>( query, new Hashtable(), view, trans );
		}

		public T ExecuteScalar<T>( string query, Hashtable prms, View view, DbTransaction trans )
		{
			view.SetQueryParams( query, prms );

			return ExecuteScalar<T>( query, prms, trans );
		}

		private void ExecuteDR(ref DbDataReader ret, DbCommand cmd, DbTransaction trans )
    	{
    		bool			newConn = false;

        	try {
				DateTime	start;

        		if( trans != null ) {
        			
					cmd.Connection  = trans.Connection;
        			cmd.Transaction = trans;

        		} else if( cmd.Connection == null ) {

        			cmd.Connection = GetConnection();
        			newConn        = true;
        		}

				SetLastCommand( cmd );

				start = DateTime.UtcNow;
                ret   = cmd.ExecuteReader();

				CheckSlowQuery( cmd, start );

        		if( newConn )
					connectionsToClose[ ret ] = cmd.Connection;
            }
            catch( Exception ex ) {

				TraceException( ex, cmd.Connection );

            	if( newConn ) {
            		CloseConnection( cmd.Connection );
					cmd.Connection = null;
				}

				throw;
            }
        }

        private void CheckSlowQuery( DbCommand cmd, DateTime start )
        {
            if( slowQueryLog > 0 ) {
                DateTime	end = DateTime.UtcNow;
                double		len = ( end - start ).TotalSeconds;

                if( len > slowQueryLog )
                    try {
                        StringBuilder sb = new StringBuilder( "Slow Query (" );
					
                        sb.Append( len ).Append( "s):" );
                        sb.AppendLine().AppendLine();
                        sb.Append( CmdToString( cmd ));
	
                        Tracer.TraceAppLog( sb.ToString(), EventLogEntryType.Warning );
                    }
                    catch {
                    }
            }
        }

        protected static string CmdToString( DbCommand cmd )
        {
            StringBuilder sb = new StringBuilder( cmd.CommandText );
			
            sb.AppendLine().AppendLine();

            sb.Append( "Parameters:" ).AppendLine().AppendLine();

            foreach(DbParameter prm in cmd.Parameters ) {

                sb.Append( prm.ParameterName ).Append( " = " );
				
                if( prm.Value == DBNull.Value )
                    sb.Append( "(DB NULL)" );
                else if( prm.Value != null ) {

                    sb.Append( prm.Value );
				
                    if( prm.Value.GetType().IsEnum )
                        sb.Append( " (" ).Append( Convert.ToInt64( prm.Value )).Append( ")" );
					
                } else
                    sb.Append( "(null obj)" );

                sb.AppendLine();
            }
			
            return sb.ToString();
        }

        protected static string GetLastCommand()
        {
            return( lastCommand );
        }

        private static void SetLastCommand( DbCommand cmd )
        {
            lastCommand = CmdToString( cmd );

            Trace.WriteLineIf( TraceQueries, lastCommand );
        }

        protected static void RemoveLastCommand()
        {
            lastCommand = null;
        }

        protected void TraceException(Exception ex)
        {
            try
            {
                if (TraceExceptions)
                    Tracer.TraceAppLog("An exception has been raised." +
                                       "\n\n" +
                                       "Last query:\n\n" +
                                       (GetLastCommand() ?? ""),
                        ex);

                RemoveLastCommand();
            }
            catch
            {
            }
        }

        public T ExecuteView<T>( string query ) where T : View, new()
		{
			return ExecuteView<T>( query, (DbTransaction)null );
		}

		public T ExecuteView<T>( string query, DbTransaction trans ) where T : View, new()
		{
			return ExecuteView<T>( query, (Hashtable)null, trans );
		}

		public T ExecuteView<T>( string query, Hashtable prms ) where T : View, new()
		{
			return ExecuteView<T>( query, prms, null );
		}

        public T ExecuteView<T>(string query, Hashtable prms, DbTransaction trans) where T : View, new()
        {
            DbDataReader dr = null;
            T ret = null;

            try
            {
                ExecuteDR(ref dr, query, prms, trans);
                ret = ViewFactory<T>.Read(dr);
            }
            catch (Exception e)
            {
                NewExceptionHandler(e);
            }
            finally
            {
                CloseDataReader(dr);
            }

            return ret;
        }

        public T ExecuteView<T>(string query, Dictionary<string, object> dic, DbTransaction trans) where T : View, new()
        {
            DbDataReader dr = null;
            T ret = null;

            try
            {
                ExecuteDR(ref dr, query, dic, trans);
                ret = ViewFactory<T>.Read(dr);
            }
            catch (Exception e)
            {
                NewExceptionHandler(e);
            }
            finally
            {
                CloseDataReader(dr);
            }

            return ret;
        }

        public T ExecuteView<T>( string query, View view, DbTransaction trans ) where T : View, new()
		{
			return ExecuteView<T>( query, new Hashtable(), view, trans );
		}

		public T ExecuteView<T>( string query, Hashtable prms, View view, DbTransaction trans ) where T : View, new()
		{
			view.SetQueryParams( query, prms );

			return ExecuteView<T>( query, prms, trans );
		}

		[NotNull]
		public T[] ExecuteViewArray<T>( string query ) where T : View, new()
		{
			return ExecuteViewArray<T>( query, (DbTransaction)null );
		}

		[NotNull]
		public T[] ExecuteViewArray<T>( string query, DbTransaction trans ) where T : View, new()
		{
			return ExecuteViewArray<T>( query, (Hashtable)null, trans );
		}

		[NotNull]
		public T[] ExecuteViewArray<T>( string query, Hashtable prms ) where T : View, new()
		{
			return ExecuteViewArray<T>( query, prms, null );
		}

        [NotNull]
        public T[] ExecuteViewArray<T>(string query, Dictionary<string, object> prms, DbTransaction trans) where T : View, new()
        {
            DbDataReader dr = null;
            T[] ret = null;

            try
            {
                ExecuteDR(ref dr, query, prms, trans);
                ret = ViewFactory<T>.GetArray(dr);
            }
            catch (Exception e)
            {
                NewExceptionHandler(e);
            }
            finally
            {
                CloseDataReader(dr);
            }

            return ret;
        }

        [NotNull]
		public T[] ExecuteViewArray<T>( string query, Hashtable prms, DbTransaction trans ) where T : View, new()
		{
			DbDataReader	dr = null;
			T[]				ret = null;

			try {
				ExecuteDR(ref dr, query, prms, trans );
				ret = ViewFactory<T>.GetArray( dr );
			}
			catch( Exception e ) {
				NewExceptionHandler( e );
			}
			finally {
				CloseDataReader( dr );
			}

			return ret;
		}

		[NotNull]
		public T[] ExecuteViewArray<T>( string query, View view, DbTransaction trans ) where T : View, new()
		{
			return ExecuteViewArray<T>( query, new Hashtable(), view, trans );
		}

		[NotNull]
		public T[] ExecuteViewArray<T>( string query, Hashtable prms, View view, DbTransaction trans ) where T : View, new()
		{
			view.SetQueryParams( query, prms );

			return ExecuteViewArray<T>( query, prms, trans );
		}

		[NotNull]
		private T[] ExecuteScalarArray<T>( DbCommand cmd, DbTransaction trans )
    	{
    		List<T>			ret = new List<T>();
    		DbDataReader	dr = null;

        	try {
				Type type = typeof( T );

				if( type.IsEnum )
					type = Enum.GetUnderlyingType( type );

				ExecuteDR(ref dr, cmd, trans );

        		while( dr.Read() )
					ret.Add((T)Convert.ChangeType( dr.GetValue( 0 ), type ));
            }
            finally {
				CloseDataReader( dr );
            }

            return ret.ToArray();
        }

		[NotNull]
		public T[] ExecuteScalarArray<T>( string query )
		{
			return ExecuteScalarArray<T>( query, (DbTransaction)null );
		}

		[NotNull]
		public T[] ExecuteScalarArray<T>( string query, DbTransaction trans )
		{
			return ExecuteScalarArray<T>( query, (Hashtable)null, trans );
		}

		[NotNull]
		public T[] ExecuteScalarArray<T>( string query, Hashtable prms )
		{
			return ExecuteScalarArray<T>( query, prms, null );
		}

		[NotNull]
		public T[] ExecuteScalarArray<T>( string query, Hashtable prms, DbTransaction trans )
		{
			T[] ret = null;

			try {
				using( DbCommand cmd = GetCommandPrivate( query, prms ))
					ret = ExecuteScalarArray<T>( cmd, trans );
			}
			catch( Exception e ) {
				NewExceptionHandler( e );
			}

			return ret;
		}

		[NotNull]
		public T[] ExecuteScalarArray<T>( string query, Array prms )
		{
			return ExecuteScalarArray<T>( query, prms, null );
		}

		[NotNull]
		public T[] ExecuteScalarArray<T>( string query, Array prms, DbTransaction trans )
		{
			T[] ret = null;

			try {
				using( DbCommand cmd = GetCommand( query, prms ))
					ret = ExecuteScalarArray<T>( cmd, trans );
			}
			catch( Exception e ) {
				NewExceptionHandler( e );
			}

			return ret;
		}

		[NotNull]
		public T[] ExecuteScalarArray<T>( string query, View view, DbTransaction trans ) where T : View, new()
		{
			return ExecuteScalarArray<T>( query, new Hashtable(), view, trans );
		}

		[NotNull]
		public T[] ExecuteScalarArray<T>( string query, Hashtable prms, View view, DbTransaction trans ) where T : View, new()
		{
			view.SetQueryParams( query, prms );

			return ExecuteScalarArray<T>( query, prms, trans );
		}

		public int ExecuteNonQuery( DbCommand cmd )
		{
			return ExecuteNonQuery( cmd, null );
		}

		public int ExecuteNonQuery( DbCommand cmd, DbTransaction trn )
		{
			bool newConn = false;

			try {
				int			ret;
				DateTime	start;

				if( trn != null ) {
					cmd.Connection  = trn.Connection;
					cmd.Transaction = trn;
				} else if( cmd.Connection == null ) {
					cmd.Connection = GetConnection();
					newConn        = true;
				}

				SetLastCommand( cmd );

				start = DateTime.UtcNow;
				ret   = cmd.ExecuteNonQuery();

				CheckSlowQuery( cmd, start );

				return( ret );
			}
			catch( Exception ex ) {
				TraceException( ex, cmd.Connection );
				throw;
			}
			finally {
				if( newConn ) {
					CloseConnection( cmd.Connection );
					cmd.Connection = null;
				}
			}
		}

		public int ExecuteNonQuery( string query )
		{
			return ExecuteNonQuery( query, (DbTransaction) null );
		}

		public int ExecuteNonQuery( string query, DbTransaction trn )
		{
			int rval = -1;

			try {
				using( DbCommand cmd = GetCommand( query ))
					rval = ExecuteNonQuery( cmd, trn );
			}
			catch( Exception e ) {
				NewExceptionHandler( e );
			}

			return rval;
		}

		public int ExecuteNonQuery( string query, Hashtable prms )
		{
			return ExecuteNonQuery( query, prms, null );
		}

        public int ExecuteNonQuery(string query, Hashtable prms, DbTransaction trn)
        {
            int ret = -1;

            try
            {
                using (DbCommand cmd = GetCommandPrivate(query, prms))
                    ret = ExecuteNonQuery(cmd, trn);
            }
            catch (Exception e)
            {
                NewExceptionHandler(e);
            }

            return ret;
        }

        public int ExecuteNonQuery(string query, Dictionary<string, object> prms, DbTransaction trn)
        {
            int ret = -1;

            try
            {
                using (DbCommand cmd = GetCommandPrivate(query, prms))
                    ret = ExecuteNonQuery(cmd, trn);
            }
            catch (Exception e)
            {
                NewExceptionHandler(e);
            }

            return ret;
        }

        public int ExecuteNonQuery( string query, View view )
		{
			return ExecuteNonQuery( query, view, null );
		}

        public int ExecuteNonQuery( string query, View view, DbTransaction trn )
		{
			return ExecuteNonQuery( query, new Hashtable(), view, trn );
        }

        public int ExecuteNonQuery( string query, Hashtable prms, View view, DbTransaction trn )
		{
			view.SetQueryParams( query, prms );

			return ExecuteNonQuery( query, prms, trn );
        }

		public int ExecuteNonQuery( string query, Array prms )
		{
			return ExecuteNonQuery( query, prms, null );
		}

        public int ExecuteNonQuery( string query, Array prms, DbTransaction trn )
		{
            int ret = -1;

            try {
            	using( DbCommand cmd = GetCommand( query, prms ))
            		ret = ExecuteNonQuery( cmd, trn );
            }
            catch( Exception e ) {
            	NewExceptionHandler( e );
            }

        	return ret;
		}

        private DbDataReader ExecuteDR(DbCommand cmd, DbTransaction trans)
        {
            bool newConn = false;
            DbDataReader ret;

            try
            {
                DateTime start;

                if (trans != null)
                {

                    cmd.Connection = trans.Connection;
                    cmd.Transaction = trans;

                }
                else if (cmd.Connection == null)
                {

                    cmd.Connection = GetConnection();
                    newConn = true;
                }

                SetLastCommand(cmd);

                start = DateTime.UtcNow;
                ret = cmd.ExecuteReader();

                CheckSlowQuery(cmd, start);

                if (newConn)
                    connectionsToClose[ret] = cmd.Connection;
            }
            catch (Exception ex)
            {

                TraceException(ex, cmd.Connection);

                if (newConn)
                {
                    CloseConnection(cmd.Connection);
                    cmd.Connection = null;
                }

                throw;
            }

            return ret;
        }

        public DbDataReader ExecuteDR( string query )
        {
            DbDataReader dr = null;
            ExecuteDR(ref dr, query, (Hashtable)null, null );
            return dr;
        }

        public DbDataReader ExecuteDR( string query, DbTransaction trans )
        {
            DbDataReader dr = null;
            ExecuteDR(ref dr, query, (Hashtable)null, trans );
            return dr;
        }

        public DbDataReader ExecuteDR( string query, Hashtable prms )
        {
            DbDataReader dr = null;
            ExecuteDR(ref dr, query, prms, null );
            return dr;
        }

        public void ExecuteDR(ref DbDataReader ret, string query, Hashtable prms, DbTransaction trans)
        {
            try
            {
                DbCommand cmd = GetCommandPrivate(query, prms);
                ExecuteDR(ref ret, cmd, trans);
            }
            catch (Exception e)
            {
                NewExceptionHandler(e);
            }
        }

        public void ExecuteDR(ref DbDataReader ret, string query, Dictionary<string, object> prms, DbTransaction trans)
        {
            try
            {
                DbCommand cmd = GetCommandPrivate(query, prms);
                ExecuteDR(ref ret, cmd, trans);
            }
            catch (Exception e)
            {
                NewExceptionHandler(e);
            }
        }

        public DbDataReader ExecuteDR( string query, Array prms )
        {
            return ExecuteDR( query, prms, null );
        }

        public DbDataReader ExecuteDR( string query, Array prms, DbTransaction trans )
        {
            DbDataReader rval = null;

            try {
                using( DbCommand cmd = GetCommand( query, prms ))
                    ExecuteDR(ref rval, cmd, trans );
            }
            catch( Exception e ) {
                NewExceptionHandler( e );
            }

            return rval;
        }

        public void CloseDataReader( DbDataReader dr )
        {
            if( dr != null ) {
                DbConnection	conn;

                dr.Close();

                if( connectionsToClose.TryRemove( dr, out conn ))
                    CloseConnection( conn );
            }
        }
    }
}
