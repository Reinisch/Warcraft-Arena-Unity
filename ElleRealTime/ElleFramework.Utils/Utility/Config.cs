using Newtonsoft.Json;

namespace ElleFramework.Utils.Utility
{
    /// <summary>
    /// Supported databases.
    /// </summary>
    public enum DatabaseType
    {
        MySql = 0,
        MSSql = 1
    }

    /// <summary>
    /// Log configuration, used to write into eventviewer.
    /// </summary>
    public class Logging
    {
        public int TraceExceptions { get; set; }
        public string TraceSourceName { get; set; }
        public bool UseTrace { get; set; }
    }

    /// <summary>
    /// Database configuration.
    /// </summary>
    public class Database
    {
        public DatabaseType Type { get; set; }
        public string Host { get; set; }
        public string User { get; set; }
        public string Password { get; set; }
        public int Port { get; set; }
        public string Name { get; set; }
        public string DAOAssembly { get; set; }
        public int DbConnectionPoolSize { get; set; }
        public int DBCmdTimeout { get; set; }
        public int DbTraceQueries { get; set; }
        public int DbSlowQueryLog { get; set; }
        public string ProviderName { get; set; }
    }

    /// <summary>
    /// Class that contains Application-level variables.
    /// Example: Telegram ChatBot token.
    /// </summary>
    public class Application
    {
        
    }

    /// <summary>
    /// Represents json config file.
    /// </summary>
    public class Config
    {
        public Logging Logging { get; set; }
        public Database Database { get; set; }
        public Application Application { get; set; }
    }
}
